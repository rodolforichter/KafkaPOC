using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Richter.Kafka.Core.Consumer;
using Richter.Kafka.Core.Product;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Richter.Kafka.POC.Controllers
{
    public class KafkaConsumerController : ControllerBase
    {
        [HttpPost("GetMessages")]
        public IActionResult GetMessages(string brokerList, IList<string> topics, string consumerGroup)
        {
            List<KafkaObjectResult<GpsLocalizationViewModel>> messages = new List<KafkaObjectResult<GpsLocalizationViewModel>>();

            StringBuilder sbStatistics = new StringBuilder();

            CancellationTokenSource cts = new CancellationTokenSource();

            var config = new ConsumerConfig
            {
                BootstrapServers = brokerList,
                GroupId = consumerGroup,
                EnableAutoCommit = false,
                StatisticsIntervalMs = 5000,
                SessionTimeoutMs = 6000,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnablePartitionEof = true
            };

            const int commitPeriod = 20;


            using (var consumer = new ConsumerBuilder<string, GpsLocalizationViewModel>(config)
                .SetErrorHandler((_, e) => sbStatistics.Append($"Error: {e.Reason} {Environment.NewLine}"))
                .SetStatisticsHandler((_, json) => sbStatistics.Append($"Statistics: {json} {Environment.NewLine}"))
                .SetPartitionsAssignedHandler((c, partitions) =>
                {
                    //sb.Append($"Assigned partitions: [{string.Join(", ", partitions)}] {Environment.NewLine}");
                })
                .SetPartitionsRevokedHandler((c, partitions) =>
                {
                    //sb.Append($"Revoking assignment: [{string.Join(", ", partitions)}] {Environment.NewLine}");
                })
                .SetValueDeserializer(new DeserializerGpsLocalization())
                .Build())
            {
                consumer.Subscribe(topics);
                

                try
                {
                   
                    foreach (var t in topics)
                    {
                        var offSet = new TopicPartitionOffset(t, 0, new Offset(0));
                        consumer.Assign(offSet);
                    }
                    while (true)
                    {
                        try
                        {
                            var consumeResult = consumer.Consume(cts.Token);

                            if (consumeResult.IsPartitionEOF)
                            {
                                break;
                            }

                            messages.Add(new KafkaObjectResult<GpsLocalizationViewModel>() { Key = consumeResult.Message.Key, OffSet = consumeResult.TopicPartitionOffset.Partition.Value, MessageResult = consumeResult.Message.Value });

                            if (consumeResult.Offset % commitPeriod == 0)
                            {
                                try
                                {
                                    consumer.Commit(consumeResult);
                                }
                                catch (KafkaException e)
                                {
                                    //mensagens.Add($"Commit error: {e.Error.Reason}");
                                }
                            }
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Consume error: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    //mensagens.Add("Closing consumer.");
                    consumer.Close();
                }

                return Ok(messages);
            }
        }
    }
}
