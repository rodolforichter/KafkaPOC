using System;

namespace Richter.Kafka.Core.Topic
{
    public class CreateTopicsOptions
    {
        public bool ValidateOnly { get; set; } = false;
        public TimeSpan? RequestTimeout { get; set; }
        public TimeSpan? OperationTimeout { get; set; }
    }
}
