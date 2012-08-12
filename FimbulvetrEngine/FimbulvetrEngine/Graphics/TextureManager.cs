using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FimbulvetrEngine.Content;

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

        public void LoadFromStream(Stream stream, Texture2D texture, bool background = false)
        {
            foreach (ITextureLoader loader in Loaders)
            {
                if (loader.LoadTexture2D(stream, texture, background))
                    break;
            }
        }
    }
}
