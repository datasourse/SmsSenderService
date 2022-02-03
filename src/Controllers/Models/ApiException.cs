using System;
using System.Net;

namespace WebApplication.Controllers.Models
{
    public class ApiException : Exception
    {
        public HttpStatusCode Code { get; set; }
        public ApiErrorsEnum Type { get; set; }
        
        public ApiException(HttpStatusCode code, ApiErrorsEnum type, string message, Exception innerException = null) : base(message, innerException)
        {
            Code = code;
            Type = type;
        }
        
        public ApiException(ApiError apiError, Exception innerException = null) : base(apiError.Message, innerException)
        {
            Code = apiError.Code;
            Type = apiError.Type;
        }
    }
}