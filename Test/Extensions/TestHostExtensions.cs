using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Test.Extensions
{
    public static class TestHostExtensions
    {
        public static Task<HttpResponseMessage> Post(this TestHost app, string requestUri, object value)
        {
            return app.Client.PostAsJsonAsync(requestUri, value);
        }
    }
}