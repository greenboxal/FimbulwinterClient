using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;

namespace FimbulwinterClient.IO.ContentLoaders
{
    public class Texture2DLoader : IContentLoader
    {
        public object LoadContent(ROContentManager rcm, Stream s, string fn)
        {
            if (fn.EndsWith(".bmp"))
            {
                Bitmap bmp = (Bitmap)Bitmap.FromStream(s);
                int[] argbData = new int[bmp.Width * bmp.Height];
                Texture2D texture = new Texture2D(rcm.Game.GraphicsDevice, bmp.Width, bmp.Height);

                unsafe
                {
                    // lock bitmap
                    System.Drawing.Imaging.BitmapData bmpData =  bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

                    uint* bgraData = (uint*)bmpData.Scan0;

                    for (int i = 0; i < argbData.Length; i++)
                    {
                        if (bgraData[i] == 0x00FF00FF)
                            argbData[i] = 0xFF;
                        else
                            argbData[i] = (int)((bgraData[i] & 0x000000ff) << 16 | (bgraData[i] & 0x0000FF00) | (bgraData[i] & 0x00FF0000) >> 16 /* | (bgraData[i] & 0xFF000000) */);
                    }

                    bgraData = null;

                    bmp.UnlockBits(bmpData);
                }

                texture.SetData(argbData);

                return texture;
            }
            else
            {
                Texture2D r = Texture2D.FromStream(rcm.Game.GraphicsDevice, s);

                s.Close();

                return r;
            }
        }
    }
}
