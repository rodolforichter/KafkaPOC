using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Richter.Kafka.POC.ViewModels
{
    public class TopicViewModel
    {
        public string Name { get; set; }
        public short ReplicationFactor { get; set; }
        public int NumPartitions { get; set; }
    }
}
