using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace HealthNet
{
    public class JsonHealthResultContent : HttpContent
    {
        private readonly MemoryStream writeStream = new MemoryStream();
        public JsonHealthResultContent(HealthResult value)
        {
            Headers.ContentType = new MediaTypeHeaderValue(Constants.Response.ContentType.Json) { CharSet = "utf-8" };
            new HealthResultJsonSerializer().SerializeToStream(writeStream, value);
            writeStream.Position = 0;
        }
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            return writeStream.CopyToAsync(stream);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = writeStream.Length;
            return true;
        }
    }
}