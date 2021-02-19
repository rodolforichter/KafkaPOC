using Richter.Kafka.Core.Topic;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Richter.Kafka.Core.Builders
{
    public class AdminClientBuilder
    {
        public Handle Handle
            => handle;
        internal protected IEnumerable<KeyValuePair<string, string>> Config { get; set; }
        public AdminClientBuilder(IEnumerable<KeyValuePair<string, string>> config)
        {
            this.Config = config;
        }

        public Task CreateTopicsAsync(IEnumerable<TopicSpecification> topics, CreateTopicsOptions options = null)
        {
            // TODO: To support results that may complete at different times, we may also want to implement:
            // public List<Task<CreateTopicResult>> CreateTopicsConcurrent(IEnumerable<TopicSpecification> topics, CreateTopicsOptions options = null)

            var completionSource = new TaskCompletionSource<List<CreateTopicReport>>();
            var gch = GCHandle.Alloc(completionSource);
            Handle.LibrdkafkaHandle.CreateTopics(
                topics, options, resultQueue,
                GCHandle.ToIntPtr(gch));
            return completionSource.Task;
        }
    }
}
