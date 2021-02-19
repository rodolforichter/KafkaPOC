using System;
using System.Collections.Generic;
using System.Text;

namespace Richter.Kafka.Core.Topic
{
    public class CreateTopicReport
    {
        public string Topic { get; set; }
        public Error Error { get; set; }
    }
}
