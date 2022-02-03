using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApplication.Controllers.Models;

namespace WebApplication.Controllers.Helpers
{
    public class CustomExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var e = context.Exception;
            ApiError error;
            if (e is ApiException apiExc)
                error = new ApiError {Code = apiExc.Code, Type = apiExc.Type, Message = apiExc.Message};
            else
                error = new ApiError
                    {Code = HttpStatusCode.InternalServerError, Type = ApiErrorsEnum.SMS_SERVICE, Message = e.Message};

            context.Result = ResponseHelper.GetResult(error);
        }
    }
}