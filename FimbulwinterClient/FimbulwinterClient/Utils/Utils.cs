using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

public static class Utils
{
    public static string ReadCString(this BinaryReader br)
    {
        string str = "";

        do
        {
            byte b = br.ReadByte();

            if (b == 0)
                break;

            str += (char)b;
        }
        while (true);

        return str;
    }

    public static string ReadCString(this BinaryReader br, int size)
    {
        int i;
        string str = "";

        for (i = 0; i < size; i++)
        {
            byte b = br.ReadByte();

            if (b != 0)
                str += (char)b;
        }

        return str;
    }

    public static void WriteCString(this BinaryWriter bw, string str, int size)
    {
        for (int i = 0; i < size; i++)
        {
            if (i < str.Length)
                bw.Write((byte)str[i]);
            else
                bw.Write((byte)0);
        }
    }

    public static string Korean(this string text)
    {
        return System.Text.Encoding.GetEncoding("EUC-KR").GetString(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(text));
    }
}