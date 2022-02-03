using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Test.Extensions;
using Test.Helpers;
using Test.Helpers.Models;
using Test.TestData;
using WebApplication.Controllers;
using WebApplication.Controllers.Models;
using WebApplication.MessageSender.Models;
using Xunit;

namespace Test.Tests
{
    public class InviteControllerTest
    {
        [Fact]
        public async Task BadPhonesFormat()
        {
            using var host = await TestHost.InitTestHost();

            var response = await host.Post($"{Routes.InvitePrefix}/{Routes.InvitePost}", new SmsDto
            {
                Phones = new List<string> {Constants.PhoneWithWrongFormat},
                Text = TestDataHelper.GenerateString(5, GenerationEncoding.Latin)
            });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal($"{ApiErrorsEnum.PHONE_NUMBERS_INVALID} {ApiErrorStrings.PhoneFormat}",
                await response.ExtractBody());
        }

        [Fact]
        public async Task PhonesListEmpty()
        {
            using var host = await TestHost.InitTestHost();

            var response = await host.Post($"{Routes.InvitePrefix}/{Routes.InvitePost}", new SmsDto
            {
                Phones = new List<string>(),
                Text = TestDataHelper.GenerateString(5, GenerationEncoding.Latin)
            });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal($"{ApiErrorsEnum.PHONE_NUMBERS_EMPTY} {ApiErrorStrings.PhoneMissing}",
                await response.ExtractBody());
        }

        [Fact]
        public async Task MaxPhonesPerQry()
        {
            using var host = await TestHost.InitTestHost();

            var response = await host.Post($"{Routes.InvitePrefix}/{Routes.InvitePost}", new SmsDto
            {
                Phones = TestDataHelper.GeneratePhonesList(host.AppSettings.MaxPhonesQtyPerQuery + 1).ToList(),
                Text = TestDataHelper.GenerateString(5, GenerationEncoding.Latin)
            });

            Assert.Equal(HttpStatusCode.PaymentRequired, response.StatusCode);
            Assert.Equal($"{ApiErrorsEnum.PHONE_NUMBERS_INVALID} {ApiErrorStrings.MaxPhonesPerQry}",
                await response.ExtractBody());
        }

        [Fact]
        public async Task MaxPhonesPerDay()
        {
            using var host = await TestHost.InitTestHost();

            await using var db = await host.GetDbContext();

            var messages = TestDataHelper.GenerateDbMessages(host.AppSettings.MaxPhonesQtyPerDay).ToList();

            await db.Messages.AddRangeAsync(messages);

            await db.SaveChangesAsync();

            var response = await host.Post($"{Routes.InvitePrefix}/{Routes.InvitePost}", new SmsDto
            {
                Phones = new List<string> {TestDataHelper.GeneratePhone()},
                Text = TestDataHelper.GenerateString(5, GenerationEncoding.Latin)
            });

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            Assert.Equal($"{ApiErrorsEnum.PHONE_NUMBERS_INVALID} {ApiErrorStrings.MaxPhonesPerDay}",
                await response.ExtractBody());
        }

        [Fact]
        public async Task PhonesDuplicates()
        {
            using var host = await TestHost.InitTestHost();

            var phone = TestDataHelper.GeneratePhone();

            var response = await host.Post($"{Routes.InvitePrefix}/{Routes.InvitePost}", new SmsDto
            {
                Phones = new List<string> {phone, phone},
                Text = TestDataHelper.GenerateString(5, GenerationEncoding.Latin)
            });

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal($"{ApiErrorsEnum.PHONE_NUMBERS_INVALID} {ApiErrorStrings.FoundPhoneDuplicates}",
                await response.ExtractBody());
        }

        [Fact]
        public async Task EmptyText()
        {
            using var host = await TestHost.InitTestHost();

            var phone = TestDataHelper.GeneratePhone();

            var response = await host.Post($"{Routes.InvitePrefix}/{Routes.InvitePost}", new SmsDto
            {
                Phones = new List<string> {phone},
                Text = string.Empty
            });

            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
            Assert.Equal($"{ApiErrorsEnum.MESSAGE_EMPTY} {ApiErrorStrings.MessageIsMissing}",
                await response.ExtractBody());
        }

        [Fact]
        public async Task TextMaxLengthNonLat()
        {
            using var host = await TestHost.InitTestHost();

            var phone = TestDataHelper.GeneratePhone();

            var response = await host.Post($"{Routes.InvitePrefix}/{Routes.InvitePost}", new SmsDto
            {
                Phones = new List<string> {phone},
                Text = TestDataHelper.GenerateString(host.AppSettings.MaxMessageLengthNonLat + 1,
                    GenerationEncoding.NonLatin)
            });

            Assert.Equal(HttpStatusCode.ProxyAuthenticationRequired, response.StatusCode);
            Assert.Equal($"{ApiErrorsEnum.MESSAGE_INVALID} {ApiErrorStrings.TooLongMessage}",
                await response.ExtractBody());
        }

        [Fact]
        public async Task TextMaxLengthLat()
        {
            using var host = await TestHost.InitTestHost();

            var phone = TestDataHelper.GeneratePhone();

            var response = await host.Post($"{Routes.InvitePrefix}/{Routes.InvitePost}", new SmsDto
            {
                Phones = new List<string> {phone},
                Text = TestDataHelper.GenerateString(host.AppSettings.MaxMessageLengthLat + 1, GenerationEncoding.Latin)
            });

            Assert.Equal(HttpStatusCode.ProxyAuthenticationRequired, response.StatusCode);
            Assert.Equal($"{ApiErrorsEnum.MESSAGE_INVALID} {ApiErrorStrings.TooLongMessage}",
                await response.ExtractBody());
        }

        [Fact]
        public async Task NonGsmEncoding()
        {
            using var host = await TestHost.InitTestHost();

            var phone = TestDataHelper.GeneratePhone();

            var response = await host.Post($"{Routes.InvitePrefix}/{Routes.InvitePost}", new SmsDto
            {
                Phones = new List<string> {phone},
                Text = Constants.TextWithWrongFormat
            });

            Assert.Equal(HttpStatusCode.NotAcceptable, response.StatusCode);
            Assert.Equal($"{ApiErrorsEnum.MESSAGE_INVALID} {ApiErrorStrings.GsmEncodingError}",
                await response.ExtractBody());
        }
    }
}