using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FimbulwinterClient.Content
{
    public class Lub : ROFormats.Lub
    {
        private byte[] _content;
        public byte[] Content
        {
            get { return _content; }
        }

        public bool Load(Stream s)
        {
            BinaryReader br = new BinaryReader(s);
            _content = br.ReadBytes(Convert.ToInt32(s.Length));
            return true;
        }
    }
}
