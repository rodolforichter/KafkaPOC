using System;
using System.Collections.Generic;
using System.Text;

namespace Richter.Kafka.Core
{
    public class Handle
    {
        internal IClient Owner { get; set; }

        internal SafeKafkaHandle LibrdkafkaHandle { get; set; }
    }
}
