using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using FimbulvetrEngine.Content;
using FimbulvetrEngine.Graphics;
using Tao.DevIl;

namespace FimbulvetrEngine.Plugins.DevIL
{
    public class DevILTextureLoader : ITextureLoader
    {
        private AutoResetEvent _mutex = new AutoResetEvent(true);

        public bool LoadTexture2D(Stream stream, Texture2D texture, bool background)
        {
            int id;

            _mutex.WaitOne();

            Il.ilGenImages(1, out id);
            Il.ilBindImage(id);

            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);

            Il.ilSetInteger(Il.IL_ORIGIN_MODE, Il.IL_ORIGIN_UPPER_LEFT);

            Il.ilLoadL(Il.IL_TYPE_UNKNOWN, buffer, buffer.Length);

            int error = Il.ilGetError();
            if (error != Il.IL_NO_ERROR)
                return false;

            int format = Il.ilGetInteger(Il.IL_IMAGE_FORMAT);
            int type = Il.ilGetInteger(Il.IL_IMAGE_TYPE);

            if (format == Il.IL_COLOR_INDEX)
            {
                format = Il.IL_RGBA;
                type = Il.IL_UNSIGNED_BYTE;
                Il.ilConvertImage(format, type);
            }
            else if (type != Il.IL_BYTE && type != Il.IL_UNSIGNED_BYTE && type != Il.IL_FLOAT && type != Il.IL_UNSIGNED_SHORT && type != Il.IL_SHORT)
            {
                type = Il.IL_UNSIGNED_BYTE;

                if (format != Il.IL_RGBA)
                    format = Il.IL_RGBA;

                Il.ilConvertImage(format, type);
            }
            else if (format != Il.IL_RGBA)
            {
                format = Il.IL_RGBA;

                Il.ilConvertImage(format, type);
            }

            int width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
            int height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);

            texture.SetSize(width, height);

            if (background)
            {
                ContentManager.Instance.FinalizeBackgroundLoading(o => FinalizeLoading(texture, id));
            }
            else
            {
                FinalizeLoading(texture, id);
            }

            return true;
        }

        public void FinalizeLoading(Texture2D texture, int id)
        {
            texture.Bind();
            Ilut.ilutGLTexImage(0);
            texture.SetLoaded();

            Il.ilDeleteImages(1, ref id);

            _mutex.Set();
        }
    }
}
