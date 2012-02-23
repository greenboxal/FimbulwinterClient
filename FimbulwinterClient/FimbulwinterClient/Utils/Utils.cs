using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FimbulwinterClient.Utils
{
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

                if (b == 0)
                    break;

                str += (char)b;
            }

            if (i < size)
                br.ReadBytes(size - i);

            return str;
        }
    }
}
