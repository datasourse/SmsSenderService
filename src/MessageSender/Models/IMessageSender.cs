using System.Threading.Tasks;
using WebApplication.Controllers.Models;

namespace WebApplication.MessageSender.Models
{
    public interface IMessageSender
    {
        public Task<IServiceResult<ApiError>> Validate(IMessageDto msg);
        public Task<IServiceResult<ApiError>> Send(IMessageDto msg);
    }
}