﻿using Avro;
using Avro.Generic;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
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
        [HttpPost("GetMessagesToGenericRecord")]
        public IActionResult GetMessagesToGenericRecord(string brokerList, IList<string> topics, string consumerGroup)
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

            string schemaRegistryUrl = "localhost:8081";

            //var schema = (RecordSchema)RecordSchema.Parse(
            //    @"{
            //        ""type"": ""record"",
            //        ""name"": ""GpsLocalizationViewModel"",
            //        ""namespace"":""Richter.Kafka.Core.Product"",
            //        ""fields"": [
            //            {""name"": ""MessageKey"", ""type"": ""string""},
            //            {""name"": ""Latitude"", ""type"": ""string""},
            //            {""name"": ""Longitude"", ""type"": ""string""},
            //            {""name"": ""VehicleId"", ""type"": ""int""},
            //        ]
            //      }"
            //);

            using (var schemaRegistry = new CachedSchemaRegistryClient(new SchemaRegistryConfig
            {
                Url = schemaRegistryUrl
            }))
            {
                using (var consumer = new ConsumerBuilder<string, GenericRecord>(config)
                    .SetErrorHandler((_, e) => sbStatistics.Append($"Error: {e.Reason} {Environment.NewLine}"))
                    .SetStatisticsHandler((_, json) => sbStatistics.Append($"Statistics: {json} {Environment.NewLine}"))
                    .SetPartitionsAssignedHandler((c, partitions) =>
                    { })
                    .SetPartitionsRevokedHandler((c, partitions) =>
                    { })
                    .SetKeyDeserializer(new AvroDeserializer<string>(schemaRegistry).AsSyncOverAsync())
                    .SetValueDeserializer(new AvroDeserializer<GenericRecord>(schemaRegistry).AsSyncOverAsync())
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

                                messages.Add(new KafkaObjectResult<GpsLocalizationViewModel>() { Key = consumeResult.Message.Key, OffSet = consumeResult.TopicPartitionOffset.Partition.Value, MessageResult = GetViewModelByConsumerResult(consumeResult) });

                                if (consumeResult.Offset % commitPeriod == 0)
                                {
                                    try
                                    {
                                        consumer.Commit(consumeResult);
                                    }
                                    catch (KafkaException e)
                                    {
                                        return BadRequest(e.Message);
                                    }
                                }
                            }
                            catch (ConsumeException e)
                            {
                                return BadRequest(e.Message);
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        consumer.Close();
                    }

                    return Ok(messages);
                }
            }
        }

        [HttpPost("GetMessagesSerializedToViewModel")]
        public IActionResult GetMessagesSerializedToViewModel(string brokerList = "localhost:9092", string topic = "GpsVm", string consumerGroup = "richter-consumer", string schemaRegistryUrl = "localhost:8081")
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
           
            //var schema = (RecordSchema)RecordSchema.Parse(
            //    @"{
            //        ""type"": ""GpsLocalizationViewModel"",
            //        ""name"": ""GpsLocalizationViewModel"",
            //        ""namespace"":""Richter.Kafka.Core.Product"",
            //        ""fields"": [
            //            {""name"": ""MessageKey"", ""type"": ""string""},
            //            {""name"": ""Latitude"", ""type"": ""string""},
            //            {""name"": ""Longitude"", ""type"": ""string""},
            //            {""name"": ""VehicleId"", ""type"": ""int""},
            //        ]
            //      }"
            //);

            using (var schemaRegistry = new CachedSchemaRegistryClient(new SchemaRegistryConfig
            {
                Url = schemaRegistryUrl
            }))
            {
                using (var consumer = new ConsumerBuilder<string, GpsLocalizationViewModel>(config)
                    .SetErrorHandler((_, e) => sbStatistics.Append($"Error: {e.Reason} {Environment.NewLine}"))
                    .SetStatisticsHandler((_, json) => sbStatistics.Append($"Statistics: {json} {Environment.NewLine}"))
                    .SetPartitionsAssignedHandler((c, partitions) =>
                    {
                    })
                    .SetPartitionsRevokedHandler((c, partitions) =>
                    {
                    })
                    .SetKeyDeserializer(new AvroDeserializer<string>(schemaRegistry).AsSyncOverAsync())
                    .SetValueDeserializer(new AvroDeserializer<GpsLocalizationViewModel>(schemaRegistry).AsSyncOverAsync())
                    .Build())
                {
                    var topics = new List<string> { topic };
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
                                        return BadRequest(e.Message);
                                    }
                                }
                            }
                            catch (ConsumeException e)
                            {
                                return BadRequest(e.Message);
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        consumer.Close();
                    }

                    return Ok(messages);
                }
            }
        }

        private static GpsLocalizationViewModel GetViewModelByConsumerResult(ConsumeResult<string, GenericRecord> consumeResult)
        {
            GpsLocalizationViewModel vm = new GpsLocalizationViewModel();
            vm.Latitude = consumeResult.Message.Value["Latitude"].ToString();
            vm.Longitude = consumeResult.Message.Value["Longitude"].ToString();
            vm.VehicleId = int.Parse(consumeResult.Message.Value["VehicleId"].ToString());
            vm.MessageKey = consumeResult.Message.Key;
            return vm;
        }
    }
}
