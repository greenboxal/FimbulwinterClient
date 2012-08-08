using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class StringExtensions
{
    private static Encoding _encoding;
    private static Encoding _encoding2;

    static StringExtensions()
    {
        _encoding = System.Text.Encoding.GetEncoding("EUC-KR");
        _encoding2 = System.Text.Encoding.GetEncoding("ISO-8859-1");
    }

    public static string Korean(this string text)
    {
        return _encoding.GetString(_encoding2.GetBytes(text));
    }

    public static string Ascii(this string text)
    {
        return _encoding2.GetString(_encoding.GetBytes(text));
    }
}