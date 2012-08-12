using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FimbulvetrEngine.Graphics
{
    public interface ITextureLoader
    {
        bool LoadTexture2D(Stream stream, Texture2D texture, bool background);
    }
}
