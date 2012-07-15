using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extensions
{
    public static class StringExtensions
    {
        public static string Korean(this string text)
        {
            return System.Text.Encoding.GetEncoding("EUC-KR").GetString(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(text));
        }
    }
}
