using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Constants;
using WebApplication.Controllers.Helpers;
using WebApplication.DB.Models;
using WebApplication.MessageSender.Models;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route(Routes.InvitePrefix)]
    public class InviteController : ControllerBase
    {
        private readonly IMessageSender _messageSender;
        private readonly IDbRepository _dbRepository;

        public InviteController(IMessageSender messageSender, IDbRepository dbRepository)
        {
            _messageSender = messageSender;
            _dbRepository = dbRepository;
        }

        [HttpPost(Routes.InvitePost)]
        public async Task<IActionResult> Post(SmsDto smsDto)
        {
            //проверяем полученные данные
            var validateResult = await _messageSender.Validate(smsDto);
            if (!validateResult.IsSuccess)
            {
                return ResponseHelper.GetResult(validateResult.Errors.First());
            }

            //добавляем в бд
            await _dbRepository.Messages.AddRangeAsync(smsDto.Phones.Select(s => new DbMessage
            {
                Id = Guid.NewGuid().ToString(),
                Phone = s,
                Text = smsDto.Text,
                AppId = AppConstants.AppId,
                DtsCreate = DateTime.Now
            }));

            await _dbRepository.SaveChangesAsync();

            //отправляем смс
            var sendResult = await _messageSender.Send(smsDto);

            return sendResult.IsSuccess
                ? ResponseHelper.GetEmptyResult()
                : ResponseHelper.GetResult(sendResult.Errors.First());
        }
    }
}