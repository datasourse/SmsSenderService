using System.Text.RegularExpressions;

namespace WebApplication.Extensions
{
    public static class StringExtensions
    {
        public static bool IsValidSmsEncoding(this string message)
        {
            var regex = new Regex(
                @"(?i)^[АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя@£$¥èéùìòÇøå_Ææß!""#%&'()*+,.\/\w:;<=>? ¡§¿äöñüà^{}\[~\]|€-]*$");
            return regex.IsMatch(message);
        }
    }
}