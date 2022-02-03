using System.Text;

namespace Test.TestData
{
    public static class Constants
    {
        public const string PhoneWithWrongFormat = "+71234445678";

        public static readonly string TextWithWrongFormat = Encoding.Unicode.GetString(Encoding.UTF8.GetBytes("test string"));
    }
}