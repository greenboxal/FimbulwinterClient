using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

public static class BinaryWriterExtensions
{
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
}
