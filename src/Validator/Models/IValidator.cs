using System.Threading.Tasks;
using WebApplication.Controllers.Models;
using WebApplication.MessageSender.Models;

namespace WebApplication.Validator.Models
{
    public interface IValidator
    {
        public Task<IServiceResult<ApiError>> Validate(IMessageDto messageDto);
    }
}