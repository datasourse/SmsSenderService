using System.Net;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Controllers.Models;

namespace WebApplication.Controllers.Helpers
{
    public static class ResponseHelper
    {
        public static ObjectResult GetResult(ApiError error)
        {
            return new ObjectResult($"{error.Type} {error.Message}")
            {
                StatusCode = (int) error.Code
            };
        }

        public static ObjectResult GetResult(HttpStatusCode code, string desc, string message)
        {
            return new ObjectResult($"{desc} {message}")
            {
                StatusCode = (int) code
            };
        }

        public static ObjectResult GetEmptyResult()
        {
            return new ObjectResult(string.Empty)
            {
                StatusCode = (int) HttpStatusCode.OK
            };
        }
    }
}