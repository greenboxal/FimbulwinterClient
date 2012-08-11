using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using QuickFont;

namespace FimbulvetrEngine.Content.Loaders
{
    public class QFontLoader : IContentLoader
    {
        public object LoadContent(ContentManager contentManager, string contentName, Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            stream.Close();

            IntPtr unmanagedPointer = Marshal.AllocHGlobal(bytes.Length);
            Marshal.Copy(bytes, 0, unmanagedPointer, bytes.Length);

            // TODO: Turn this more flexibile, maybee transfer this code to some font manager
            QFont result = new QFont(unmanagedPointer, bytes.Length, 8, FontStyle.Regular, new QFontBuilderConfiguration(false));

            Marshal.FreeHGlobal(unmanagedPointer);

            return result;
        }
    }
}
