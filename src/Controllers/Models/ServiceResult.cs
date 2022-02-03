using System;

namespace WebApplication.Controllers.Models
{
    public class ServiceResult<TError> : IServiceResult<TError>
    {
        protected ServiceResult(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        private readonly TError[] _errors;

        public TError[] Errors
        {
            get => _errors ?? Array.Empty<TError>();
            protected init => _errors = value;
        }

        public bool IsSuccess { get; }

        public static IServiceResult<TError> Success() => new ServiceResult<TError>(true);

        public static IServiceResult<TError> Fault(params TError[] errors)
        {
            var serviceResult = new ServiceResult<TError>(false)
            {
                Errors = errors
            };
            return serviceResult;
        }
    }

    public class ServiceResult<TError, TResult> : ServiceResult<TError>, IServiceResult<TError, TResult>
    {
        private ServiceResult(TResult result, bool isSuccess) : base(isSuccess)
        {
            Result = result;
        }

        public TResult Result { get; }


        public static IServiceResult<TError, TResult> Success(TResult result) =>
            new ServiceResult<TError, TResult>(result, true);

        public static IServiceResult<TError, TResult> Fault(TResult result, params TError[] errors)
        {
            var serviceResult = new ServiceResult<TError, TResult>(result, false)
            {
                Errors = errors
            };
            return serviceResult;
        }
    }
}