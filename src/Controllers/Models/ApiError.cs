using System.Net;

namespace WebApplication.Controllers.Models
{
    public class ApiError
    {
        public HttpStatusCode Code { get; set; }
        public ApiErrorsEnum Type { get; set; }
        public string Message { get; set; }
    }
}