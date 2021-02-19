using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.AspNetCore.Mvc;
using Richter.Kafka.POC.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Richter.Kafka.POC.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class KakfaAdminController : ControllerBase
    {
        static string ToString(int[] array) => $"[{string.Join(", ", array)}]";

        [HttpPost("CreateTopic")]
        public async Task<IActionResult> Post([FromBody] TopicViewModel topic)
        {
            using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = "localhost:9092" }).Build())
            {
                try
                {
                    await adminClient.CreateTopicsAsync(
                        new TopicSpecification[] { 
                            new TopicSpecification { Name = topic.Name, ReplicationFactor = topic.ReplicationFactor, NumPartitions = topic.NumPartitions 
                            } 
                        });
                
                }
                catch (CreateTopicsException e)
                {
                    Exception ex = new Exception($"An error occured creating topic { e.Results[0].Topic}: {e.Results[0].Error.Reason}", e);
                    return BadRequest(ex);
                }
                catch(KafkaException ex)
                {
                    return BadRequest(ex);
                }
            }

            return Ok();
        }

        [HttpGet("ListConsumerGroups")]
        public IActionResult ListConsumerGroups(string bootstrapServers)
        {
            StringBuilder sb = new StringBuilder();

            using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = bootstrapServers }).Build())
            {
                var groups = adminClient.ListGroups(TimeSpan.FromSeconds(10));
                
                sb.Append($"Consumer Groups:");

                foreach (var g in groups)
                {
                    sb.Append($"Group: {g.Group} {g.Error} {g.State}");
                    sb.Append($"Broker: {g.Broker.BrokerId} {g.Broker.Host}:{g.Broker.Port}");
                    sb.Append($"Protocol: {g.ProtocolType} {g.Protocol}");
                    sb.Append($"Members:");
                    foreach (var m in g.Members)
                    {
                        sb.Append($"{m.MemberId} {m.ClientId} {m.ClientHost}");
                        sb.Append($"Metadata: {m.MemberMetadata.Length} bytes");
                        sb.Append($"Assignment: {m.MemberAssignment.Length} bytes");
                    }
                }
            }

            return Ok(sb.ToString());
        }

        [HttpGet("PrintMetadata")]
        public IActionResult PrintMetadata(string bootstrapServers)
        {
            StringBuilder sb = new StringBuilder();

            using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = bootstrapServers }).Build())
            {
                var meta = adminClient.GetMetadata(TimeSpan.FromSeconds(20));

                sb.Append($"{meta.OriginatingBrokerId} {meta.OriginatingBrokerName}");

                meta.Brokers.ForEach(broker =>
                    sb.Append($"Broker: {broker.BrokerId} {broker.Host}:{broker.Port}"));

                meta.Topics.ForEach(topic =>
                {
                    sb.Append($"Topic: {topic.Topic} {topic.Error}");
                    topic.Partitions.ForEach(partition =>
                    {
                        sb.Append($"Partition: {partition.PartitionId}");
                        sb.Append($"Replicas: {ToString(partition.Replicas)}");
                        sb.Append($"InSyncReplicas: {ToString(partition.InSyncReplicas)}");
                    });
                });
            }

            return Ok(sb.ToString());
        }
    }
}
