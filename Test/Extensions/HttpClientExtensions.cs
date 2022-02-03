using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Test.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<string> ExtractBody(this HttpResponseMessage request)
        {
            return Encoding.UTF8.GetString(await request.Content.ReadAsByteArrayAsync());
        }
    }
}