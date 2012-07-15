using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FimbulwinterClient.Content
{
    class Palette : ROFormats.Palette
    {
        public bool Load(Stream s)
        {
            using (BinaryReader br = new BinaryReader(s))
            {
                base.Load(br);
            }
            return true;
        }
    }
}
