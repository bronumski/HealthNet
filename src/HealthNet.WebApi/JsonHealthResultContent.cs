using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace HealthNet
{
    public class JsonHealthResultContent : HttpContent
    {
        private readonly MemoryStream _Stream = new MemoryStream();
        public JsonHealthResultContent(HealthResult value)
        {
            Headers.ContentType = new MediaTypeHeaderValue(Constants.Response.ContentType.Json);
            new HealthResultJsonSerializer().SerializeToStream(_Stream, value);
            _Stream.Position = 0;
        }
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            return _Stream.CopyToAsync(stream);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = _Stream.Length;
            return true;
        }
    }
}