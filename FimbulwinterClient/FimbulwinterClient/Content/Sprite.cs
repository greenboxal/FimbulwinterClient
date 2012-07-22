using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FimbulwinterClient.Content
{
    public class Sprite : ROFormats.Sprite
    {
        private Texture2D[] _images;
        public Texture2D[] Images
        {
            get { return _images; }
            set { _images = value; }
        }

        private GraphicsDevice _gd;

        public bool Load(GraphicsDevice gd, Stream s)
        {
            bool res;

            _gd = gd;
            res = base.Load(s);

            if (!res)
                return false;

            _images = new Texture2D[RawImages.Length];

            if (RgbaCount > 0)
            {
                for (int i = PalCount; i < RawImages.Length; i++)
                {
                    _images[i] = new Texture2D(_gd, RawImages[i].Width, RawImages[i].Height, false, SurfaceFormat.Color);
                }
            }

            RecreatePalImages();

            return true;
        }

        public override void SetPalette(ROFormats.Palette p)
        {
            base.SetPalette(p);

            RecreatePalImages();
        }

        private void RecreatePalImages()
        {
            for (int i = 0; i < PalCount; i++)
            {
                _images[i] = new Texture2D(_gd, RawImages[i].Width, RawImages[i].Height, false, SurfaceFormat.Color);
                _images[i].SetData(RawImages[i].RawData);
            }
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

            sb.Draw(_images[idx], r, null, new Color(mo.Clips[i].Mask.R, mo.Clips[i].Mask.G, mo.Clips[i].Mask.B, mo.Clips[i].Mask.A), 
                (float)(Math.PI * mo.Clips[i].Angle / 180.0F), default(Vector2),
                se, 0);
        }
    }
}
