using System;
using System.Collections.Generic;
using Test.Helpers.Models;
using WebApplication.DB.Models;

namespace Test.Helpers
{
    public class TestDataHelper
    {
        public static IEnumerable<string> GeneratePhonesList(int count)
        {
            for (var i = 0; i < count; i++)
            {
                yield return GeneratePhone();
            }
        }

        public static string GeneratePhone()
        {
            var time = DateTime.Now.Ticks;
            return $"7{time % 10000000000}";
        }

        public static IEnumerable<DbMessage> GenerateDbMessages(int count)
        {
            for (var i = 1; i <= count; i++)
            {
                yield return new DbMessage
                {
                    Id = Guid.NewGuid().ToString(),
                    Phone = GeneratePhone(),
                    Text = GenerateString(10, GenerationEncoding.NonLatin),
                    AppId = 4,
                    DtsCreate = DateTime.Now
                };
            }
        }

        public static string GenerateString(int count, GenerationEncoding encoding)
        {
            return string.Concat(GenerateChars(count, encoding));
        }
        
        private static IEnumerable<char> GenerateChars(int count, GenerationEncoding encoding)
        {
            string allowedSymbols;
            
            switch (encoding)
            {
                case GenerationEncoding.Latin:
                    allowedSymbols = @"@Å.<JXft£å^!/=KYgu$Δ{“0>LZhv¥_}#1?MÄiwèΦ\¤2¡NÖjxéΓ[%3AOÑkyùΛ~&4BPÜlzìΩ]‘5CQ§mäòΠ|(6DR¿nöÇΨ€)7ESaoñΣÆ*8FTbpüØΘæ+9GUcqàøΞß,:HVdrÉ-;IWes ";
                    break;
                case GenerationEncoding.NonLatin:
                    allowedSymbols = "абвгдеёжзиклмнопрстуфхцчшщъыьэюя ";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(encoding), encoding, null);
            }

            var rand = new Random();
            
            for (var i = 1; i <= count; i++)
            {
                yield return allowedSymbols[rand.Next(0, allowedSymbols.Length - 1)];
            }
        }
    }
}