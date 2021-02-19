namespace Richter.Kafka.Core
{
    public class AdminConfig : Config
    {
        public string BootstrapServers { get { return Get("bootstrap.servers"); } set { this.SetObject("bootstrap.servers", value); } }
    }
}
