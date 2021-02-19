namespace Richter.Kafka.Core.Topic
{
    internal class Partition : ValueObject
    {
        public uint PartitionQuantity { get; private set; }

        internal Partition(uint partitionQuantity)
        {
            PartitionQuantity = partitionQuantity;
        }

        public override bool IsValid()
        {
            return PartitionQuantity > 0;
        }
    }
}
