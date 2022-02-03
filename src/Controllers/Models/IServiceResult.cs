namespace WebApplication.Controllers.Models
{
    public interface IServiceResult<out TError>
    {
        TError[] Errors { get; }
        bool IsSuccess { get; }
    }

    public interface IServiceResult<out TError, out TResult> : IServiceResult<TError>
    {
        TResult Result { get; }
    }
}