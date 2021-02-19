using System.Runtime.Serialization;

namespace Richter.Kafka.Core.Consumer
{
    public class KafkaObjectResult<T> where T : class
    {
        public T MessageResult { get; set; }
        public string Partition { get; set; }
        public int OffSet { get; set; }
        public string Key { get; set; }
        public string Timestamp { get; set; }
    }
}
