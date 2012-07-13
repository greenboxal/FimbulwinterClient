using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ROFormats
{
    public class Sprite
    {
        public struct RawImage
        {
            public byte[] RawData { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
        }

        List<byte[]> palData = new List<byte[]>();

        private short _version;
        public short Version
        {
            get { return _version; }
            set { _version = value; }
        }

        private short _palCount;
        public short PalCount
        {
            get { return _palCount; }
            set { _palCount = value; }
        }

        private short _rgbaCount;
        public short RgbaCount
        {
            get { return _rgbaCount; }
            set { _rgbaCount = value; }
        }

        private RawImage[] _images;
        public RawImage[] RawImages
        {
            get { return _images; }
            set { _images = value; }
        }

        private Palette _palette;
        public Palette Palette
        {
            get { return _palette; }
        }

        public Sprite()
        {

        }

        public virtual void SetPalette(Palette p)
        {
            _palette = p;

            RemapPalette();
        }

        public virtual bool Load(Stream s)
        {
            BinaryReader br = new BinaryReader(s);
            byte[] magic = br.ReadBytes(2);

            if (magic[0] != (byte)'S' || magic[1] != (byte)'P')
            {
                return false;
            }

            _version = br.ReadInt16();

            if (_version != 0x100 && _version != 0x101 &&
                _version != 0x200 && _version != 0x201)
            {
                return false;
            }

            _palCount = br.ReadInt16();
            if (_version >= 0x200)
                _rgbaCount = br.ReadInt16();

            List<RawImage> images = new List<RawImage>();

            if (_palCount > 0)
            {
                for (int p = 0; p < _palCount; p++)
                {
                    ushort w = br.ReadUInt16();
                    ushort h = br.ReadUInt16();
                    int pixelCount = w * h;

                    if (pixelCount > 0)
                    {
                        byte[] data;

                        if (_version >= 0x201)
                        {
                            data = new byte[pixelCount];

                            uint next = 0;
                            ushort encoded;

                            encoded = br.ReadUInt16();
                            while (next < pixelCount && encoded > 0)
                            {
                                byte c = br.ReadByte();

                                encoded--;

                                if (c == 0)
                                {
                                    byte len = br.ReadByte();

                                    encoded--;

                                    if (len == 0)
                                        len = 1;

                                    if (next + len > pixelCount)
                                        return false;

                                    for (int i = 0; i < len; i++)
                                        data[next] = 0;

                                    next += len;
                                }
                                else
                                {
                                    data[next++] = c;
                                }
                            }

                            if (next != pixelCount || encoded > 0)
                                return false;
                        }
                        else
                        {
                            data = br.ReadBytes(pixelCount);
                        }

                        palData.Add(data);
                        images.Add(new RawImage() { Height = h, Width = w, RawData = data });
                    }
                }
            }

            if (_rgbaCount > 0)
            {
                for (int p = 0; p < _rgbaCount; p++)
                {
                    ushort w = br.ReadUInt16();
                    ushort h = br.ReadUInt16();
                    byte[] texData = new byte[w * h * 4];

                    for (int i = 0; i < texData.Length; i += 4)
                    {
                        // RGBA to ARGB
                        texData[i + 1] = br.ReadByte();
                        texData[i + 2] = br.ReadByte();
                        texData[i + 3] = br.ReadByte();
                        texData[i + 0] = br.ReadByte();
                    }

                    images.Add(new RawImage() { Height = h, Width = w, RawData = texData });
                }
            }

            _images = images.ToArray();

            if (_version >= 0x101)
            {
                _palette = new Palette();
                _palette.Read(br);

                RemapPalette();
            }

            return true;
        }

        protected virtual void RemapPalette()
        {
            for (int p = 0; p < palData.Count; p++)
            {
                byte[] data = palData[p];
                int w = _images[p].Width;
                int h = _images[p].Height;

                byte[] texData = new byte[w * h * 4];
                for (int i = 0; i < texData.Length; i += 4)
                {
                    byte idx = data[i / 4];
                    ROFormats.Palette.Color c = _palette.Colors[idx];

                    if (idx == 0)
                    {
                        texData[i + 0] = 0;
                        texData[i + 1] = 0;
                        texData[i + 2] = 0;
                        texData[i + 3] = 0;
                    }
                    else
                    {
                        texData[i + 0] = c.R;
                        texData[i + 1] = c.G;
                        texData[i + 2] = c.B;
                        texData[i + 3] = 255;
                    }
                }

                _images[p].RawData = texData;
            }
        }

        public int GetIndex(int idx, int type)
        {
            if (type == 0)
            {
                if (idx >= 0 && idx < _palCount)
                    return idx;
            }
            else if (type == 1)
            {
                if (idx >= 0 && idx < _rgbaCount)
                    return idx + _palCount;
            }

            return -1;
        }
    }
}
