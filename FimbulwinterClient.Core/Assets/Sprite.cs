using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FimbulwinterClient.Core.Assets
{
    public class Sprite
    {
        byte[][] _palData;

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

        private Texture2D[] _images;
        public Texture2D[] Images
        {
            get { return _images; }
            set { _images = value; }
        }

        private Palette _palette;
        public Palette Palette
        {
            get { return _palette; }
        }

        private GraphicsDevice _graphicsDevice;
        public GraphicsDevice GraphicsDevice
        {
            get { return _graphicsDevice; }
        }

        public Sprite(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        public bool Load(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            byte[] magic = reader.ReadBytes(2);

            if (magic[0] != (byte)'S' || magic[1] != (byte)'P')
            {
                return false;
            }

            _version = reader.ReadInt16();

            if (_version != 0x100 && _version != 0x101 &&
                _version != 0x200 && _version != 0x201)
            {
                return false;
            }

            _palCount = reader.ReadInt16();
            if (_version >= 0x200)
                _rgbaCount = reader.ReadInt16();

            _palData = new byte[_palCount][];
            _images = new Texture2D[_palCount + _rgbaCount];
            if (_palCount > 0)
            {
                for (int p = 0; p < _palCount; p++)
                {
                    ushort w = reader.ReadUInt16();
                    ushort h = reader.ReadUInt16();
                    int pixelCount = w * h;

                    if (pixelCount > 0)
                    {
                        byte[] data;

                        if (_version >= 0x201)
                        {
                            data = new byte[pixelCount];

                            uint next = 0;
                            ushort encoded;

                            encoded = reader.ReadUInt16();
                            while (next < pixelCount && encoded > 0)
                            {
                                byte c = reader.ReadByte();

                                encoded--;

                                if (c == 0)
                                {
                                    byte len = reader.ReadByte();

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
                            data = reader.ReadBytes(pixelCount);
                        }

                        _palData[p] = data;
                        _images[p] = new Texture2D(_graphicsDevice, w, h, false, SurfaceFormat.Color);
                    }
                }
            }

            if (_rgbaCount > 0)
            {
                for (int p = 0; p < _rgbaCount; p++)
                {
                    ushort w = reader.ReadUInt16();
                    ushort h = reader.ReadUInt16();
                    byte[] texData = reader.ReadBytes(w * h * 4);

                    Texture2D tex = new Texture2D(_graphicsDevice, w, h, false, SurfaceFormat.Color);
                    tex.SetData(texData);

                    _images[_palCount + p] = tex;
                }
            }

            if (_version >= 0x101)
            {
                Palette pal = new Palette();
                pal.Load(stream);
                SetPalette(pal);
            }

            return true;
        }

        public void SetPalette(Palette palette)
        {
            _palette = palette;

            RecreatePalImages();
        }

        private void RecreatePalImages()
        {
            for (int i = 0; i < _palCount; i++)
            {
                Color[] data = new Color[_images[i].Width * _images[i].Height];

                for (int j = 0; j < _palData[i].Length; j++)
                {
                    data[j] = _palette.Colors[_palData[i][j]];
                }

                _images[i].SetData(data);
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

        public void Draw(SpriteAction.Motion mo, int i, SpriteBatch sb, int x, int y, bool ext, SpriteEffects se = SpriteEffects.None)
        {
            SpriteAction.SpriteClip sc = mo.Clips[i];
            int idx = GetIndex(sc.SpriteNumber, sc.SpriteType);

            if (idx == -1)
                return;

            float w, h;
            w = _images[idx].Width;
            h = _images[idx].Height;

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

            sb.Draw(_images[idx], r, null, new Color(mo.Clips[i].Mask.R, mo.Clips[i].Mask.G, mo.Clips[i].Mask.B, mo.Clips[i].Mask.A), MathHelper.ToRadians(mo.Clips[i].Angle), default(Vector2), se, 0);
        }
    }
}
