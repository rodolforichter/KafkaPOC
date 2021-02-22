using Confluent.SchemaRegistry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Richter.Kafka.Core.Product
{
    public class GpsLocalizationSchemaRegistry : ISchemaRegistryClient
    {
        public int MaxCachedSchemas => throw new NotImplementedException();

        public string ConstructKeySubjectName(string topic, string recordType = null)
        {
            throw new NotImplementedException();
        }

        public string ConstructValueSubjectName(string topic, string recordType = null)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetAllSubjectsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<RegisteredSchema> GetLatestSchemaAsync(string subject)
        {
            throw new NotImplementedException();
        }

        public Task<RegisteredSchema> GetRegisteredSchemaAsync(string subject, int version)
        {
            throw new NotImplementedException();
        }

        public Task<Schema> GetSchemaAsync(int id, string format = null)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetSchemaAsync(string subject, int version)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetSchemaIdAsync(string subject, string avroSchema)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetSchemaIdAsync(string subject, Schema schema)
        {
            throw new NotImplementedException();
        }

        public Task<List<int>> GetSubjectVersionsAsync(string subject)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsCompatibleAsync(string subject, string avroSchema)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsCompatibleAsync(string subject, Schema schema)
        {
            throw new NotImplementedException();
        }

        public Task<RegisteredSchema> LookupSchemaAsync(string subject, Schema schema, bool ignoreDeletedSchemas)
        {
            throw new NotImplementedException();
        }

        public Task<int> RegisterSchemaAsync(string subject, string avroSchema)
        {
            throw new NotImplementedException();
        }

        public Task<int> RegisterSchemaAsync(string subject, Schema schema)
        {
            throw new NotImplementedException();
        }
    }
}
