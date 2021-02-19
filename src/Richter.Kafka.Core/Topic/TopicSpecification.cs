using System.Collections.Generic;

namespace Richter.Kafka.Core.Topic
{
    public class TopicSpecification : ValueObject
    {
        private const short NameMinLength = 2;

        public string Name { get; private set; }    
        internal Partition Partition { get; private set; }
        public short ReplicationFactor { get; private set; } = -1;
        public IDictionary<string, string> Configs { get; private set; }
        public IDictionary<int, List<int>> ReplicasAssignments { get; private set; } = null;

        public TopicSpecification(TopicConfigViewModel topicConfig)
        {
            Name = topicConfig.Name;
            Partition = new Partition(topicConfig.PartitionQuantity);
            ReplicationFactor = topicConfig.ReplicationFactor;
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(Name) && Name.Length > NameMinLength && Partition.IsValid();
        }
    }
}
