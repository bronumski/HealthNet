using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace HealthNet
{
    class HealthResultJsonSerializer
    {
        private readonly JsonSerializerSettings jsonSerializerSettings;

        public HealthResultJsonSerializer()
        {
            jsonSerializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
            };
            jsonSerializerSettings.Converters.Add(new StringEnumConverter());
        }

        public long SerializeToStream(Stream outputStream, HealthResult result)
        {
            var serializer = JsonSerializer.Create(jsonSerializerSettings);

            using (var writeStream = new MemoryStream())
            using (var streamWritter = new StreamWriter(writeStream))
            {
                serializer.Serialize(streamWritter, result);

                streamWritter.Flush();
                writeStream.Position = 0;

                writeStream.CopyTo(outputStream);
                return writeStream.Position;
            }
        }
    }
}