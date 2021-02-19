using Confluent.Kafka;
using Confluent.SchemaRegistry.Serdes;
using NJsonSchema.Generation;
using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Richter.Kafka.Core.Product
{
    //public class DeserializerGpsLocalization : Confluent.Kafka.IDeserializer<GpsLocalizationViewModel>
    //{
    //    private readonly int headerSize = sizeof(int) + sizeof(byte);
    //    private readonly JsonSchemaGeneratorSettings jsonSchemaGeneratorSettings;
    //    public GpsLocalizationViewModel Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    //    {
    //        //string text =  Encoding.UTF32.GetString(data.ToArray());
    //        //return JsonSerializer.Deserialize<GpsLocalizationViewModel>(text);

    //        try
    //        {
    //            var array = data.ToArray();

    //            using (var stream = new MemoryStream(array, headerSize, array.Length - 9))
    //            using (var sr = new System.IO.StreamReader(stream, Encoding.UTF8))
    //            {
    //                object obj = Newtonsoft.Json.JsonConvert.DeserializeObject<GpsLocalizationViewModel>(sr.ReadToEnd(), this.jsonSchemaGeneratorSettings?.ActualSerializerSettings);
    //                //return Newtonsoft.Json.JsonConvert.DeserializeObject<GpsLocalizationViewModel>(sr.ReadToEnd(), this.jsonSchemaGeneratorSettings?.ActualSerializerSettings);
    //                return null;
    //            }
    //        }
    //        catch (AggregateException e)
    //        {
    //            throw e.InnerException;
    //        }
    //    }
    //}

    public class DeserializerGpsLocalization : Confluent.Kafka.IDeserializer<GpsLocalizationViewModel>
    {
        private readonly int headerSize = sizeof(int) + sizeof(byte);
        private readonly JsonSchemaGeneratorSettings jsonSchemaGeneratorSettings;
        public GpsLocalizationViewModel Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            //string text =  Encoding.UTF32.GetString(data.ToArray());
            //return JsonSerializer.Deserialize<GpsLocalizationViewModel>(text);

            try
            {
                var array = data.ToArray();

                using (var stream = new MemoryStream(array, headerSize, array.Length - 9))
                using (var sr = new System.IO.StreamReader(stream, Encoding.UTF8))
                {
                    object obj = Newtonsoft.Json.JsonConvert.DeserializeObject<GpsLocalizationViewModel>(sr.ReadToEnd(), this.jsonSchemaGeneratorSettings?.ActualSerializerSettings);
                    //return Newtonsoft.Json.JsonConvert.DeserializeObject<GpsLocalizationViewModel>(sr.ReadToEnd(), this.jsonSchemaGeneratorSettings?.ActualSerializerSettings);
                    return null;
                }
            }
            catch (AggregateException e)
            {
                throw e.InnerException;
            }
        }
    }
    
}
