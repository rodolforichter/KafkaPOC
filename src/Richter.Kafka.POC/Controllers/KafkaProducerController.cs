using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Richter.Kafka.POC.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Richter.Kafka.POC.Controllers
{
    public class KafkaProducerController : ControllerBase
    {
        [HttpPost("Send")]
        public async Task<IActionResult> Send([FromBody] MessageViewModel message, string broker, string topicName)
        {
            var config = new ProducerConfig { BootstrapServers = broker };

            using (var producer = new ProducerBuilder<string, string>(config).Build())
            {
                try
                {
                    var deliveryReport = await producer.ProduceAsync(
                        topicName, new Message<string, string> { Key = message.MessageKey, Value = message.MessageValue });

                   return Ok($"delivered to: {deliveryReport.TopicPartitionOffset}");
                }
                catch (ProduceException<string, string> e)
                {
                    return BadRequest($"failed to deliver message: {e.Message} [{e.Error.Code}]");
                }
            }
        }
    }
}
