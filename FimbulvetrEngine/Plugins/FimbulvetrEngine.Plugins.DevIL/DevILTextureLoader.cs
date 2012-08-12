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

            Il.ilLoadL(Il.IL_TYPE_UNKNOWN, buffer, buffer.Length);

            int error = Il.ilGetError();
            if (error != Il.IL_NO_ERROR)
                return false;

            texture.SetSize(Il.ilGetInteger(Il.IL_IMAGE_WIDTH), Il.ilGetInteger(Il.IL_IMAGE_HEIGHT));

            Dispatcher.Instance.DispatchCoreTask(o =>
                {
                    texture.SetTexture(Ilut.ilutGLBindTexImage());
                    texture.SetLoaded();

                    Il.ilDeleteImages(1, ref id);

                    _mutex.Set();
                }, background);

            return true;
        }
    }
}
