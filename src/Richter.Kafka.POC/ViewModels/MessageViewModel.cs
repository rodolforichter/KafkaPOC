using Richter.Kafka.Core.Product;

namespace Richter.Kafka.POC.ViewModels
{
    public class MessageViewModel
    {
        public string MessageKey { get; set; }
        public GpsLocalizationViewModel Localization { get; set; }
    }
}
