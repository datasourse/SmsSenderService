using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApplication.Controllers.Models;
using WebApplication.DB.Models;
using WebApplication.Extensions;
using WebApplication.MessageSender.Models;
using WebApplication.Settings.Models;
using WebApplication.Validator.Models;
using WebApplication.Validator.Transliterator;
using WebApplication.Validator.Transliterator.Models;

namespace WebApplication.Validator
{
    public sealed class SmsValidator : IValidator
    {
        private static IOptions<AppSettings> _options;
        private static IDbRepository _dbRepository;
        private static TransliterationService _transliterationService;

        private static readonly char[] RestrictedPhoneSymbols = {' ', '+', '(', ')'};

        //список проверок, выполняемых по порядку для IMessageDto
        private readonly List<Func<IMessageDto, Task<IServiceResult<ApiError>>>> _checks = new()
        {
            CheckPhonesListEmpty,
            CheckMaxMessagesLimitPerDay,
            CheckMaxPhonesQty,
            CheckPhoneDuplicates,
            CheckPhoneFormat,
            CheckEmptyText,
            CheckTextMaxLength,
            CheckTextEncoding
        };

        public SmsValidator(TransliterationService transliterationService, IOptions<AppSettings> options,
            IDbRepository dbRepository)
        {
            _transliterationService = transliterationService;
            _options = options;
            _dbRepository = dbRepository;
        }

        public async Task<IServiceResult<ApiError>> Validate(IMessageDto messageDto)
        {
            foreach (var check in _checks)
            {
                await check(messageDto);
            }

            return ServiceResult<ApiError>.Success();
        }

        /// <summary>
        /// Проверка лимита отправленных сообщений в день
        /// </summary>
        /// <param name="messageDto"></param>
        /// <returns></returns>
        /// <exception cref="ApiException"></exception>
        private static async Task<IServiceResult<ApiError>> CheckMaxMessagesLimitPerDay(IMessageDto messageDto)
        {
            //проверка на максимальное количество смс в день
            var todayMessageCount = await
                _dbRepository.Messages.Where(m => m.AppId == 4 && m.DtsCreate.DayOfYear == DateTime.Now.DayOfYear)
                    .CountAsync();

            if (todayMessageCount + messageDto.Phones.Count > _options.Value.MaxPhonesQtyPerDay)
                throw new ApiException(new ApiError
                {
                    Code = HttpStatusCode.Forbidden,
                    Type = ApiErrorsEnum.PHONE_NUMBERS_INVALID,
                    Message = ApiErrorStrings.MaxPhonesPerDay
                });

            return ServiceResult<ApiError>.Success();
        }

        /// <summary>
        /// Проверка на пустой список телефонов
        /// </summary>
        /// <param name="messageDto"></param>
        /// <returns></returns>
        /// <exception cref="ApiException"></exception>
        private static async Task<IServiceResult<ApiError>> CheckPhonesListEmpty(IMessageDto messageDto)
        {
            var apiErr = new ApiError
            {
                Code = HttpStatusCode.Unauthorized,
                Type = ApiErrorsEnum.PHONE_NUMBERS_EMPTY
            };

            if (messageDto.Phones.IsNullOrEmpty())
            {
                apiErr.Message = ApiErrorStrings.PhoneMissing;
                throw new ApiException(apiErr);
            }

            //отсеиваем номера, на которые уже отправлялась смс
            var existedPhonesToday = await _dbRepository.Messages.Where(m =>
                    m.DtsCreate.DayOfYear == DateTime.Now.DayOfYear && messageDto.Phones.Any(p => p == m.Phone))
                .Select(x => x.Phone).ToListAsync();

            messageDto.Phones.RemoveAll(p => existedPhonesToday.Any(ep => ep == p));

            if (messageDto.Phones.IsNullOrEmpty())
            {
                apiErr.Message = ApiErrorStrings.PhoneAlreadySended;
                throw new ApiException(apiErr);
            }

            return ServiceResult<ApiError>.Success();
        }

        /// <summary>
        /// Проверка на максимальное количество телефонов в запросе
        /// </summary>
        /// <param name="messageDto"></param>
        /// <returns></returns>
        /// <exception cref="ApiException"></exception>
        private static Task<IServiceResult<ApiError>> CheckMaxPhonesQty(IMessageDto messageDto)
        {
            return Task.Run(() =>
                {
                    if (messageDto.Phones.Count > _options.Value.MaxPhonesQtyPerQuery)
                        throw new ApiException(new ApiError
                        {
                            Code = HttpStatusCode.PaymentRequired,
                            Type = ApiErrorsEnum.PHONE_NUMBERS_INVALID,
                            Message = ApiErrorStrings.MaxPhonesPerQry
                        });
                    return ServiceResult<ApiError>.Success();
                }
            );
        }

        /// <summary>
        /// Проверка телефонов на дубли
        /// </summary>
        /// <param name="messageDto"></param>
        /// <returns></returns>
        /// <exception cref="ApiException"></exception>
        private static Task<IServiceResult<ApiError>> CheckPhoneDuplicates(IMessageDto messageDto)
        {
            return Task.Run(() =>
                {
                    var hasDuplicates = messageDto.Phones.GroupBy(x => x)
                        .Any(g => g.Count() > 1);

                    if (hasDuplicates)
                        throw new ApiException(new ApiError
                        {
                            Code = HttpStatusCode.NotFound,
                            Type = ApiErrorsEnum.PHONE_NUMBERS_INVALID,
                            Message = ApiErrorStrings.FoundPhoneDuplicates
                        });
                    return ServiceResult<ApiError>.Success();
                }
            );
        }

        /// <summary>
        /// Проверка формата каждого телефона
        /// </summary>
        /// <param name="messageDto"></param>
        /// <returns></returns>
        /// <exception cref="ApiException"></exception>
        private static Task<IServiceResult<ApiError>> CheckPhoneFormat(IMessageDto messageDto)
        {
            return Task.Run(() =>
                {
                    var checkRes = true;

                    foreach (var phone in messageDto.Phones)
                    {
                        if (string.IsNullOrEmpty(phone)
                            || phone.Length != 11
                            || phone[0] != '7'
                            || phone.Any(d => RestrictedPhoneSymbols.Contains(d)))
                            checkRes = false;
                        break;
                    }

                    if (checkRes) return ServiceResult<ApiError>.Success();
                    throw new ApiException(new ApiError
                    {
                        Code = HttpStatusCode.BadRequest,
                        Type = ApiErrorsEnum.PHONE_NUMBERS_INVALID,
                        Message = ApiErrorStrings.PhoneFormat
                    });
                }
            );
        }

        /// <summary>
        /// Проверка на пустой текст сообщения
        /// </summary>
        /// <param name="messageDto"></param>
        /// <returns></returns>
        /// <exception cref="ApiException"></exception>
        private static Task<IServiceResult<ApiError>> CheckEmptyText(IMessageDto messageDto)
        {
            return Task.Run(() =>
                {
                    if (string.IsNullOrEmpty(messageDto.Text))
                        throw new ApiException(new ApiError
                        {
                            Code = HttpStatusCode.MethodNotAllowed,
                            Type = ApiErrorsEnum.MESSAGE_EMPTY,
                            Message = ApiErrorStrings.MessageIsMissing
                        });
                    return ServiceResult<ApiError>.Success();
                }
            );
        }

        /// <summary>
        /// Проверка кодировки текста 
        /// </summary>
        /// <param name="messageDto"></param>
        /// <returns></returns>
        /// <exception cref="ApiException"></exception>
        private static Task<IServiceResult<ApiError>> CheckTextEncoding(IMessageDto messageDto)
        {
            return Task.Run(() =>
                {
                    //проверяем отсутствие символов не из GSM или кириллицы
                    if (!messageDto.Text.IsValidSmsEncoding())
                        throw new ApiException(new ApiError
                        {
                            Code = HttpStatusCode.NotAcceptable,
                            Type = ApiErrorsEnum.MESSAGE_INVALID,
                            Message = ApiErrorStrings.GsmEncodingError
                        });

                    //переводим русские символы в латиницу
                    messageDto.Text = _transliterationService.Transliterate(messageDto.Text, TransliterationType.Gost);

                    return ServiceResult<ApiError>.Success();
                }
            );
        }

        /// <summary>
        /// Проверка максимальной длины сообщения
        /// </summary>
        /// <param name="messageDto"></param>
        /// <returns></returns>
        private static Task<IServiceResult<ApiError>> CheckTextMaxLength(IMessageDto messageDto)
        {
            return Task.Run(() =>
                {
                    //текстом на латинице будем считать только если все символы принадлежат латинице
                    var isTextLat = messageDto.Text.All(ch => ch >= 97 && ch <= 122);

                    if (isTextLat && messageDto.Text.Length <= _options.Value.MaxMessageLengthLat
                        || !isTextLat && messageDto.Text.Length <= _options.Value.MaxMessageLengthNonLat)
                        return ServiceResult<ApiError>.Success();

                    throw new ApiException(new ApiError
                    {
                        Code = HttpStatusCode.ProxyAuthenticationRequired,
                        Type = ApiErrorsEnum.MESSAGE_INVALID,
                        Message = ApiErrorStrings.TooLongMessage
                    });
                }
            );
        }
    }
}