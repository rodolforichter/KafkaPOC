using Avro;
using Avro.Specific;
using System;
using System.Runtime.Serialization;

namespace Richter.Kafka.Core.Product
{
    [Serializable]
    public partial class GpsLocalizationViewModel : ISpecificRecord
    {
        public static Schema _SCHEMA = Avro.Schema.Parse(@"{""type"":""record"",""name"":""GpsLocalizationViewModel"",""namespace"":""Richter.Kafka.Core.Product"",""fields"":[{""name"":""MessageKey"",""type"":""string""},{""name"":""Latitude"",""type"":""string""},{""name"":""Longitude"",""type"":""string""},{""name"":""VehicleId"",""type"":""int""}]}");

        public string MessageKey { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public int VehicleId { get; set; }
        public virtual Schema Schema => _SCHEMA;
        public object Get(int fieldPos)
        {
            switch (fieldPos)
            {
                case 0: return this.MessageKey;
                case 1: return this.Latitude;
                case 2: return this.Longitude;
                case 3: return this.VehicleId;
                default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
            };
        }

        public void Put(int fieldPos, object fieldValue)
        {
            switch (fieldPos)
            {
                case 0: this.MessageKey = (System.String)fieldValue; break;
                case 1: this.Latitude = (System.String)fieldValue; break;
                case 2: this.Longitude = (System.String)fieldValue; break;
                case 3: this.VehicleId = (System.Int32)fieldValue; break;
                default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
            };
        }
    }
}
