using System;
using System.Collections.Generic;
using System.Text;

namespace Richter.Kafka.Core
{
    public static class ConfigPropertyNames
    {
        public static class Producer
        {
            public const string EnableBackgroundPoll = "dotnet.producer.enable.background.poll";
            public const string EnableDeliveryReports = "dotnet.producer.enable.delivery.reports";
            public const string DeliveryReportFields = "dotnet.producer.delivery.report.fields";
        }

        public static class Consumer
        {
            public const string ConsumeResultFields = "dotnet.consumer.consume.result.fields";
        }
      
        public const string CancellationDelayMaxMs = "dotnet.cancellation.delay.max.ms";
    }
}
