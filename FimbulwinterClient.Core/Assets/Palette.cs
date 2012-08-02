using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;

namespace FimbulwinterClient.Core.Assets
{
    public class Palette
    {
        private Color[] _colors;
        public Color[] Colors
        {
            get { return _colors; }
            set { _colors = value; }
        }

        public Palette()
        {
            _colors = new Color[256];
        }

        public bool Load(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);

            for (int i = 0; i < 256; i++)
            {
                int r = reader.ReadByte();
                int g = reader.ReadByte();
                int b = reader.ReadByte();

                reader.ReadByte();

                _colors[i] = new Color(r, g, b, 255);
            }

            _colors[0] = Color.Transparent;

            return true;
        }
    }
}
