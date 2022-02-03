using System.Threading.Tasks;
using WebApplication.Controllers.Models;
using WebApplication.MessageSender.Models;

namespace Test.MockedServices
{
    public class SmsSenderApiMock : ISenderApi
    {
        public async Task<IServiceResult<ApiError>> Send(IMessageDto messages)
        {
            return ServiceResult<ApiError>.Success();
        }
    }
}