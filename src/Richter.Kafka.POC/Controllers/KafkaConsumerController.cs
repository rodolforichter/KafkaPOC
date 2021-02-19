using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Richter.Kafka.POC.Controllers
{
    public class KafkaConsumerController : ControllerBase
    {
        [HttpPost("GetMessages")]
        public IActionResult GetMessages(string brokerList, IList<string> topics)
        {
            List<string> mensagens = new List<string>();
            StringBuilder sbStatistics = new StringBuilder();
            CancellationTokenSource cts = new CancellationTokenSource();
            var config = new ConsumerConfig
            {
                BootstrapServers = brokerList,
                GroupId = "richter-consumer",
                EnableAutoCommit = false,
                StatisticsIntervalMs = 5000,
                SessionTimeoutMs = 6000,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnablePartitionEof = true
            };

            const int commitPeriod = 5;

          
            using (var consumer = new ConsumerBuilder<string, string>(config)                
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
                .Build())
            {
                consumer.Subscribe(topics);
                

                try
                {
                    var offSet = new TopicPartitionOffset("Bandas", 0, new Offset(0));
                    foreach (var t in topics)
                    {
                        consumer.Assign(offSet);
                    }
                    while (true)
                    {
                        try
                        {
                            var consumeResult = consumer.Consume(cts.Token);

                            if (consumeResult.IsPartitionEOF)
                            {
                                mensagens.Add($"Fim do Topic {consumeResult.Topic}, offset {consumeResult.Offset}.");
                                break;
                            }

                            mensagens.Add($"Recebida mensagem da partição de offSet { consumeResult.TopicPartitionOffset.Partition.Value } ,Mensagem: {consumeResult.Message.Value}, Chave: { consumeResult.Message.Key } {Environment.NewLine}");

                            if (consumeResult.Offset % commitPeriod == 0)
                            {
                                try
                                {
                                    consumer.Commit(consumeResult);
                                }
                                catch (KafkaException e)
                                {
                                    mensagens.Add($"Commit error: {e.Error.Reason}");
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
                    mensagens.Add("Closing consumer.");
                    consumer.Close();
                }

                return Ok(mensagens);
            }
        }
    }
}
