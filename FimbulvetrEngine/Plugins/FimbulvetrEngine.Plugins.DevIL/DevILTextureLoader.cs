using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FimbulvetrEngine.Graphics;
using Tao.DevIl;

namespace FimbulvetrEngine.Plugins.DevIL
{
    public class DevILTextureLoader : ITextureLoader
    {
        public Texture2D LoadTexture2D(Stream stream)
        {
            int id;

            Il.ilGenImages(1, out id);
            Il.ilBindImage(id);

            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);

            Il.ilEnable(Il.IL_ORIGIN_SET);
            Il.ilSetInteger(Il.IL_ORIGIN_MODE, Il.IL_ORIGIN_UPPER_LEFT);

            Il.ilLoadL(Il.IL_TYPE_UNKNOWN, buffer, buffer.Length);

            int error = Il.ilGetError();
            if (error != Il.IL_NO_ERROR)
                return null;

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

            Texture2D result = new Texture2D(width, height);

            result.Bind();
            Ilut.ilutGLTexImage(0);

            Il.ilDisable(Il.IL_ORIGIN_SET);
            Il.ilDisable(Il.IL_FORMAT_SET);

            Il.ilDeleteImages(1, ref id);

            return result;
        }
    }
}
