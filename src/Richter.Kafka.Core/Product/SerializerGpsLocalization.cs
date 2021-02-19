using Confluent.Kafka;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Richter.Kafka.Core.Product
{
    public class SerializerGpsLocalization : ISerializer<GpsLocalizationViewModel>
    {
        public byte[] Serialize(GpsLocalizationViewModel data, SerializationContext context)
        {
            if (data == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, data);
                return ms.ToArray();
            }
        }
    }
}
