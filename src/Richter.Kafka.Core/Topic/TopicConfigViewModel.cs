namespace Richter.Kafka.Core.Topic
{
    public class TopicConfigViewModel
    {
        public string Name { get; set; }
        public short ReplicationFactor { get; set; }
        public uint PartitionQuantity { get; set; }
    }
}
