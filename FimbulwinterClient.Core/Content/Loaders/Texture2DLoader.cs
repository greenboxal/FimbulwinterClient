using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace FimbulwinterClient.Core.Content.Loaders
{
    public class Texture2DLoader : IContentLoader
    {
        private Texture2D GetTexture(GraphicsDevice device, Bitmap bmp)
        {
            uint[] imageData = new uint[bmp.Width * bmp.Height];
            Texture2D texture = new Texture2D(device, bmp.Width, bmp.Height);

            unsafe
            {
                BitmapData origdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                uint *byteData = (uint *)origdata.Scan0;

                for (int i = 0; i < imageData.Length; i++)
                {
                    imageData[i] = (byteData[i] & 0x000000ff) << 16 | (byteData[i] & 0x0000FF00) | (byteData[i] & 0x00FF0000) >> 16 | (byteData[i] & 0xFF000000);
                }

                byteData = null;
                bmp.UnlockBits(origdata);
            }

            texture.SetData(imageData);

            return texture;
        }

        public object Load(Stream stream, string assetName)
        {
            Texture2D result = null;

            if (assetName.EndsWith(".jpg") || assetName.EndsWith(".png") || assetName.EndsWith(".gif"))
            {
                result = Texture2D.FromStream(SharedInformation.GraphicsDevice, stream);
            }
            else
            {
                using (Bitmap bitmap = (Bitmap)Image.FromStream(stream))
                {
                    result = GetTexture(SharedInformation.GraphicsDevice, bitmap);
                }
            }

            stream.Close();

            return result;
        }
    }
}
