using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FimbulvetrEngine.Graphics
{
    public interface ITextureLoader
    {
        Texture2D LoadTexture2D(Stream stream);
    }
}
