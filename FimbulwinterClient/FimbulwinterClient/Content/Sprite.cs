using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FimbulwinterClient.Content
{
    public class Sprite
    {
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

        private Texture2D[] m_images;
        public Texture2D[] Images
        {
            get { return m_images; }
            set { m_images = value; }
        }

        private Palette m_palette;
        internal Palette Palette
        {
            get { return m_palette; }
            set { m_palette = value; RemapPalette(); }
        }

        private GraphicsDevice m_gd;
        public GraphicsDevice GraphicsDevice
        {
            get { return m_gd; }
            set { m_gd = value; }
        }

        public Sprite()
        {

        }

        public bool Load(GraphicsDevice gd, Stream s)
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

            List<Texture2D> images = new List<Texture2D>();

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
                        images.Add(new Texture2D(gd, w, h, false, SurfaceFormat.Color));
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

                    Texture2D tex = new Texture2D(gd, w, h, false, SurfaceFormat.Color);
                    tex.SetData(texData);
                    images.Add(tex);
                }
            }

            m_images = images.ToArray();
            m_gd = gd;

            if (m_version >= 0x101)
            {
                m_palette = new Palette();
                m_palette.Read(br);

                RemapPalette();
            }

            return true;
        }

        private void RemapPalette()
        {
            for (int p = 0; p < palData.Count; p++)
            {
                byte[] data = palData[p];
                int w = m_images[p].Width;
                int h = m_images[p].Height;

                Texture2D tex = new Texture2D(m_gd, w, h, false, SurfaceFormat.Color);
                uint[] texData = new uint[w * h];

                for (int i = 0; i < texData.Length; i++)
                {
                    byte idx = data[i];
                    Color c = m_palette.Colors[idx];

                    if (idx == 0)
                    {
                        texData[i] = Color.Transparent.PackedValue;
                    }
                    else
                    {
                        texData[i] = c.PackedValue;
                    }
                }

                m_images[p].SetData(texData);
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

        public void Draw(SpriteAction.Motion mo, int i, SpriteBatch sb, int x, int y, bool ext)
        {
            SpriteAction.SpriteClip sc = mo.Clips[i];
            int idx = GetIndex(sc.SpriteNumber, sc.SpriteType);

            if (idx == -1)
                return;

            float w, h;
            w = m_images[idx].Width;
            h = m_images[idx].Height;

            w *= sc.Zoom.X;
            h *= sc.Zoom.Y;

            if (ext && mo.AttachPoints.Count > 0)
            {
                x -= mo.AttachPoints[0].Position.X;
                y -= mo.AttachPoints[0].Position.Y;
            }

            Rectangle r = new Rectangle(
                (int)(x - Math.Ceiling(w / 2) + sc.Position.X),
                (int)(y - Math.Ceiling(h / 2) + sc.Position.Y),
                (int)w,
                (int)h);

            sb.Draw(m_images[idx], r, null, mo.Clips[i].Mask, 
                (float)(Math.PI * mo.Clips[i].Angle / 180.0F), default(Vector2), 
                SpriteEffects.None, 0);
        }
    }
}
