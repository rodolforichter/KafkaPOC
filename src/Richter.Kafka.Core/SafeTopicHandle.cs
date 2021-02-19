using System;
using System.Collections.Generic;
using System.Text;

namespace Richter.Kafka.Core
{
    internal sealed class SafeTopicHandle : SafeHandleZeroIsInvalid
    {
        const int RD_KAFKA_PARTITION_UA = -1;

        internal SafeKafkaHandle kafkaHandle;

        private SafeTopicHandle() : base("kafka topic") { }

        protected override bool ReleaseHandle()
        {
            Librdkafka.topic_destroy(handle);
            // This corresponds to the DangerousAddRef call when
            // the TopicHandle was created.
            kafkaHandle.DangerousRelease();
            return true;
        }

        internal string GetName()
            => Util.Marshal.PtrToStringUTF8(Librdkafka.topic_name(handle));

        internal bool PartitionAvailable(int partition)
        {
            return Librdkafka.topic_partition_available(handle, partition);
        }
    }
}
