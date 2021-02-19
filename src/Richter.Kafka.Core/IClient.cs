using System;

namespace Richter.Kafka.Core
{
    public interface IClient : IDisposable
    {
        
        Handle Handle { get; }
        string Name { get; }
        int AddBrokers(string brokers);
    }
}
