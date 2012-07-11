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

        private short m_version;
        public short Version
        {
            get { return m_version; }
            set { m_version = value; }
        }

        private short m_palCount;
        public short PalCount
        {
            get { return m_palCount; }
            set { m_palCount = value; }
        }

        private short m_rgbaCount;
        public short RgbaCount
        {
            get { return m_rgbaCount; }
            set { m_rgbaCount = value; }
        }

        private RawImage[] m_images;
        public RawImage[] RawImages
        {
            get { return m_images; }
            set { m_images = value; }
        }

        private Palette m_palette;
        public Palette Palette
        {
            get { return m_palette; }
        }

        public Sprite()
        {

        }

        public virtual void SetPalette(Palette p)
        {
            m_palette = p;

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

            m_version = br.ReadInt16();

            if (m_version != 0x100 && m_version != 0x101 &&
                m_version != 0x200 && m_version != 0x201)
            {
                return false;
            }

            m_palCount = br.ReadInt16();
            if (m_version >= 0x200)
                m_rgbaCount = br.ReadInt16();

            List<RawImage> images = new List<RawImage>();

            if (m_palCount > 0)
            {
                for (int p = 0; p < m_palCount; p++)
                {
                    ushort w = br.ReadUInt16();
                    ushort h = br.ReadUInt16();
                    int pixelCount = w * h;

                    if (pixelCount > 0)
                    {
                        byte[] data;

                        if (m_version >= 0x201)
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

            if (m_rgbaCount > 0)
            {
                for (int p = 0; p < m_rgbaCount; p++)
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

            m_images = images.ToArray();

            if (m_version >= 0x101)
            {
                m_palette = new Palette();
                m_palette.Read(br);

                RemapPalette();
            }

            return true;
        }

        protected virtual void RemapPalette()
        {
            for (int p = 0; p < palData.Count; p++)
            {
                byte[] data = palData[p];
                int w = m_images[p].Width;
                int h = m_images[p].Height;

                byte[] texData = new byte[w * h * 4];
                for (int i = 0; i < texData.Length; i += 4)
                {
                    byte idx = data[i / 4];
                    ROFormats.Palette.Color c = m_palette.Colors[idx];

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

                m_images[p].RawData = texData;
            }
        }

        public int GetIndex(int idx, int type)
        {
            if (type == 0)
            {
                if (idx >= 0 && idx < m_palCount)
                    return idx;
            }
            else if (type == 1)
            {
                if (idx >= 0 && idx < m_rgbaCount)
                    return idx + m_palCount;
            }

            return -1;
        }
    }
}
