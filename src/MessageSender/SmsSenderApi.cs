using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebApplication.Controllers.Models;
using WebApplication.MessageSender.Models;
using WebApplication.Settings.Models;

namespace WebApplication.MessageSender
{
    public class SmsSenderApi : ISenderApi
    {
        private readonly HttpClient _client;
        private readonly IOptions<SmsSettings> _settings;
        private readonly ILogger<SmsSenderApi> _logger;

        public SmsSenderApi(HttpClient client, IOptions<SmsSettings> settings, ILogger<SmsSenderApi> logger)
        {
            _client = client;
            _settings = settings;
            _logger = logger;

            _client.BaseAddress = new Uri(_settings.Value.Host);
        }

        private string GetAuthorizeHeader()
        {
            return
                $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_settings.Value.UserName}:{_settings.Value.Pass}"))}";
        }

        public async Task<IServiceResult<ApiError>> Send(IMessageDto messages)
        {
            using var req = new HttpRequestMessage(HttpMethod.Post, "api/send");
            req.Headers.Add("Authorization", GetAuthorizeHeader());

            HttpResponseMessage response;
            try
            {
                response = await _client.SendAsync(req,
                    new CancellationTokenSource(TimeSpan.FromSeconds(_settings.Value.Timeout)).Token);
            }
            catch (Exception e)
            {
                _logger.LogError($"{nameof(SmsSenderApi)} error: {e.Message}");
                throw new ApiException(new ApiError
                {
                    Code = HttpStatusCode.BadRequest,
                    Type = ApiErrorsEnum.SMS_SERVICE,
                    Message = $"SmsSenderApi error: {e.Message}"
                });
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError($"{nameof(SmsSenderApi)} error {response.StatusCode}: {response.ReasonPhrase}");
                //имитация сервиса, поэтому ошибку не возвращаем
            }

            _logger.LogInformation("All messages sent");

            return ServiceResult<ApiError>.Success();
        }
    }
}