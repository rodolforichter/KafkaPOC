using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Richter.Kafka.Core
{
    public class Config : IEnumerable<KeyValuePair<string, string>>
    {
        private static Dictionary<string, string> EnumNameToConfigValueSubstitutes = new Dictionary<string, string>
        {
            { "saslplaintext", "sasl_plaintext" },
            { "saslssl", "sasl_ssl" },
            { "consistentrandom", "consistent_random" },
            { "murmur2random", "murmur2_random" },
            { "readcommitted", "read_committed" },
            { "readuncommitted", "read_uncommitted" }
        };

        public Config() { this.properties = new Dictionary<string, string>(); }      
        public Config(Config config) { this.properties = config.properties; }
        public Config(IDictionary<string, string> config) { this.properties = config; }
        public void Set(string key, string val)
        {
            this.properties[key] = val;
        }

        public string Get(string key)
        {
            if (this.properties.TryGetValue(key, out string val))
            {
                return val;
            }
            return null;
        }

        
        protected int? GetInt(string key)
        {
            var result = Get(key);
            if (result == null) { return null; }
            return int.Parse(result);
        }

      
        protected bool? GetBool(string key)
        {
            var result = Get(key);
            if (result == null) { return null; }
            return bool.Parse(result);
        }

        protected double? GetDouble(string key)
        {
            var result = Get(key);
            if (result == null) { return null; }
            return double.Parse(result);
        }

        protected object GetEnum(Type type, string key)
        {
            var result = Get(key);
            if (result == null) { return null; }
            if (EnumNameToConfigValueSubstitutes.Values.Count(v => v == result) > 0)
            {
                return Enum.Parse(type, EnumNameToConfigValueSubstitutes.First(v => v.Value == result).Key, ignoreCase: true);
            }
            return Enum.Parse(type, result, ignoreCase: true);
        }

        protected void SetObject(string name, object val)
        {
            if (val == null)
            {
                this.properties.Remove(name);
                return;
            }

            if (val is Enum)
            {
                var stringVal = val.ToString().ToLowerInvariant();
                if (EnumNameToConfigValueSubstitutes.TryGetValue(stringVal, out string substitute))
                {
                    this.properties[name] = substitute;
                }
                else
                {
                    this.properties[name] = stringVal;
                }
            }
            else
            {
                this.properties[name] = val.ToString();
            }
        }

       
        protected IDictionary<string, string> properties;
        
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => this.properties.GetEnumerator();
        
        public int CancellationDelayMaxMs { set { this.SetObject(ConfigPropertyNames.CancellationDelayMaxMs, value); } }

        private const int DefaultCancellationDelayMaxMs = 100;

        internal static IEnumerable<KeyValuePair<string, string>> ExtractCancellationDelayMaxMs(
            IEnumerable<KeyValuePair<string, string>> config, out int cancellationDelayMaxMs)
        {
            var cancellationDelayMaxString = config
                .Where(prop => prop.Key == ConfigPropertyNames.CancellationDelayMaxMs)
                .Select(a => a.Value)
                .FirstOrDefault();

            if (cancellationDelayMaxString != null)
            {
                if (!int.TryParse(cancellationDelayMaxString, out cancellationDelayMaxMs))
                {
                    throw new ArgumentException(
                        $"{ConfigPropertyNames.CancellationDelayMaxMs} must be a valid integer value.");
                }
                if (cancellationDelayMaxMs < 1 || cancellationDelayMaxMs > 10000)
                {
                    throw new ArgumentOutOfRangeException(
                        $"{ConfigPropertyNames.CancellationDelayMaxMs} must be in the range 1 <= {ConfigPropertyNames.CancellationDelayMaxMs} <= 10000");
                }
            }
            else
            {
                cancellationDelayMaxMs = DefaultCancellationDelayMaxMs;
            }

            return config.Where(prop => prop.Key != ConfigPropertyNames.CancellationDelayMaxMs);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
