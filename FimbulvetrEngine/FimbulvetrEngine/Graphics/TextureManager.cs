using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FimbulvetrEngine.Graphics
{
    public class TextureManager
    {
        public static TextureManager Instance { get; private set; }

        public List<ITextureLoader> Loaders { get; private set; }

        public TextureManager()
        {
            if (Instance != null)
                throw new Exception("Only one instance of TextureManager is allowed, use the Instance property.");

            Loaders = new List<ITextureLoader>();

            Instance = this;
        }

        public void RegisterTextureLoader(ITextureLoader loader)
        {
            Loaders.Add(loader);
        }

        public Texture2D LoadFromStream(Stream stream)
        {
            return Loaders.Select(loader => loader.LoadTexture2D(stream)).FirstOrDefault(texture => texture != null);
        }
    }
}
