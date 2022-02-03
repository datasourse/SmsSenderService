namespace WebApplication.Controllers.Models
{
    public class ApiErrorStrings
    {
        public const string PhoneFormat = "One or several phone numbers do not match with international format.";
        public const string PhoneMissing = "Phone numbers are missing.";
        public const string PhoneAlreadySended = "A message has already been sent to all numbers from the list.";
        public const string MaxPhonesPerQry = "Too much phone numbers, should be less or equal to 16 per request.";
        public const string MaxPhonesPerDay = "Too much phone numbers, should be less or equal to 128 per day.";
        public const string FoundPhoneDuplicates = "Duplicate numbers detected.";
        public const string MessageIsMissing = "Invite message is missing.";
        public const string GsmEncodingError = "Invitemessage should contain only characters in 7-bit GSM encoding or Cyrillic letters as well.";
        public const string TooLongMessage = "Invite message too long, should be less or equal to 128 characters of 7-bit GSM charset.";
    }
}