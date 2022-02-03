using System.Threading.Tasks;
using WebApplication.Controllers.Models;

namespace WebApplication.MessageSender.Models
{
    public interface ISenderApi
    {
        public Task<IServiceResult<ApiError>> Send(IMessageDto messages);
    }
}