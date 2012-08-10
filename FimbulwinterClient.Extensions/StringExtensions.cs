using System.Text;

namespace FimbulwinterClient.Extensions
{
    public static class StringExtensions
    {
        private static readonly Encoding Encoding;
        private static readonly Encoding Encoding2;

        static StringExtensions()
        {
            Encoding = Encoding.GetEncoding("EUC-KR");
            Encoding2 = Encoding.GetEncoding("ISO-8859-1");
        }

        public static string Korean(this string text)
        {
            return Encoding.GetString(Encoding2.GetBytes(text));
        }

        public static string Ascii(this string text)
        {
            return Encoding2.GetString(Encoding.GetBytes(text));
        }
    }
}