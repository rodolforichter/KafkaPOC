using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Richter.Kafka.Core.Product;
using System.Threading.Tasks;

namespace Richter.Kafka.POC.Controllers
{
    public class KafkaProducerController : ControllerBase
    {
        [HttpPost("Send")]
        public async Task<IActionResult> Send([FromBody] GpsLocalizationViewModel message, string broker, string topicName)
        {
            var config = new ProducerConfig { BootstrapServers = broker };

            using (var producer = new ProducerBuilder<string, GpsLocalizationViewModel>(config)
                .SetValueSerializer(new SerializerGpsLocalization())
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
            }
        }
    }
}
