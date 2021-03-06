﻿using Avro;
using Avro.Generic;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.AspNetCore.Mvc;
using Richter.Kafka.Core.Product;
using System;
using System.Threading.Tasks;

namespace Richter.Kafka.POC.Controllers
{
    public class KafkaProducerController : ControllerBase
    {
        public static RecordSchema _schemaViewModel = (RecordSchema)RecordSchema.Parse(
                @"{
                    ""type"": ""record"",
                    ""name"": ""GpsLocalizationViewModel"",
                    ""namespace"":""Richter.Kafka.Core.Product"",
                    ""fields"": [
                        {""name"": ""MessageKey"", ""type"": ""string""},
                        {""name"": ""Latitude"", ""type"": ""string""},
                        {""name"": ""Longitude"", ""type"": ""string""},
                        {""name"": ""VehicleId"", ""type"": ""int""},
                    ]
                  }"
            );

        public static RecordSchema _schemaRecordGeneric = (RecordSchema)RecordSchema.Parse(
                @"{
                    ""type"": ""record"",
                    ""name"": ""GpsLocalizationViewModel"",
                    ""namespace"":""Richter.Kafka.Core.Product"",
                    ""fields"": [
                        {""name"": ""MessageKey"", ""type"": ""string""},
                        {""name"": ""Latitude"", ""type"": ""string""},
                        {""name"": ""Longitude"", ""type"": ""string""},
                        {""name"": ""VehicleId"", ""type"": ""int""},
                    ]
                  }"
            );

        [HttpPost("SendGenericRecord")]
        public async Task<IActionResult> SendGenericRecord([FromBody] GpsLocalizationViewModel message, string topicName = "Gps02", string broker = "localhost:9092", string schemaRegistryUrl = "localhost:8081")
        {
            GenericRecord genericMsg = new GenericRecord(_schemaRecordGeneric);
            genericMsg.Add("MessageKey", message.MessageKey);
            genericMsg.Add("Latitude", message.Latitude);
            genericMsg.Add("Longitude", message.Longitude);
            genericMsg.Add("VehicleId", message.VehicleId);

            var config = new ProducerConfig { BootstrapServers = broker };
            using (var schemaRegistry = new CachedSchemaRegistryClient(new SchemaRegistryConfig { Url = schemaRegistryUrl }))
            {
                using (var producer = new ProducerBuilder<string, GenericRecord>(config)
                    .SetKeySerializer(new AvroSerializer<string>(schemaRegistry))
                    .SetValueSerializer(new AvroSerializer<GenericRecord>(schemaRegistry))
                    .Build())
                {
                    try
                    {
                        var deliveryReport = await producer.ProduceAsync(
                            topicName, new Message<string, GenericRecord> { Key = message.MessageKey, Value = genericMsg });

                        return Ok($"delivered to: {deliveryReport.TopicPartitionOffset}");
                    }
                    catch (ProduceException<string, string> e)
                    {
                        return BadRequest($"failed to deliver message: {e.Message} [{e.Error.Code}]");
                    }
                }
            }
        }

        [HttpPost("SendViewModel")]
        public async Task<IActionResult> SendViewModel([FromBody] GpsLocalizationViewModel message, string topicName = "GpsVm", string broker = "localhost:9092", string schemaRegistryUrl = "localhost:8081"){
            
            var config = new ProducerConfig { BootstrapServers = broker };
            using (var schemaRegistry = new CachedSchemaRegistryClient(new SchemaRegistryConfig { Url = schemaRegistryUrl }))
            {
                using (var producer = new ProducerBuilder<string, GpsLocalizationViewModel>(config)
                    .SetKeySerializer(new AvroSerializer<string>(schemaRegistry))
                    .SetValueSerializer(new AvroSerializer<GpsLocalizationViewModel>(schemaRegistry))
                    .Build())
                {
                    try
                    {
                        var deliveryReport = await producer.ProduceAsync(
                            topicName, new Message<string, GpsLocalizationViewModel> { Key = message.MessageKey, Value = message });

                        return Ok($"delivered to: {deliveryReport.TopicPartitionOffset}");
                    }
                    catch (ProduceException<string, string> e)
                    {
                        return BadRequest($"failed to deliver message: {e.Message} [{e.Error.Code}]");
                    }
                    catch(Exception e){
                        return BadRequest($"failed to deliver message: {e.Message}");
                    }
                }
            }
        }
    }
}
