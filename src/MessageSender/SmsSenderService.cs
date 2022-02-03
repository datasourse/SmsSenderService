using System.Threading.Tasks;
using WebApplication.Controllers.Models;
using WebApplication.MessageSender.Models;
using WebApplication.Validator.Models;

namespace WebApplication.MessageSender
{
    public class SmsSenderService : IMessageSender
    {
        private readonly ISenderApi _senderApi;
        private readonly IValidator _validator;

        public SmsSenderService(ISenderApi senderApi, IValidator validator)
        {
            _senderApi = senderApi;
            _validator = validator;
        }

        public Task<IServiceResult<ApiError>> Validate(IMessageDto msg)
        {
            return _validator.Validate(msg);
        }

        public async Task<IServiceResult<ApiError>> Send(IMessageDto msg)
        {
            await _senderApi.Send(msg);
            return ServiceResult<ApiError>.Success();
        }
    }
}