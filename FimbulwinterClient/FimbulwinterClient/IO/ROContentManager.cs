using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using FimbulwinterClient.IO.ContentLoaders;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using FimbulwinterClient.Content;

namespace FimbulwinterClient.IO
{
    public interface IContentLoader
    {
        object LoadContent(ROContentManager rcm, Stream s, string fn);
    }

    public class ROContentManager : ContentManager
    {
        private static Dictionary<Type, IContentLoader> m_loaders;
        public static Dictionary<Type, IContentLoader> Loaders
        {
            get { return ROContentManager.m_loaders; }
        }

        static ROContentManager()
        {
            m_loaders = new Dictionary<Type, IContentLoader>();

            m_loaders.Add(typeof(Stream), new StreamLoader());
            m_loaders.Add(typeof(Texture2D), new Texture2DLoader());
            m_loaders.Add(typeof(SoundEffect), new SoundEffectLoader());
            m_loaders.Add(typeof(Sprite), new SpriteLoader());
            m_loaders.Add(typeof(SpriteAction), new SpriteActionLoader());
            m_loaders.Add(typeof(RsmModel), new RsmModelLoader());
        }

        private Dictionary<string, object> m_cache;
        public Dictionary<string, object> Cache
        {
            get { return m_cache; }
        }

        private ROFileSystem m_fs;
        public ROFileSystem FileSystem
        {
            get { return m_fs; }
        }

        private ROClient m_g;
        public ROClient Game
        {
            get { return m_g; }
            set { m_g = value; }
        }

        public ROContentManager(IServiceProvider isp, ROClient g)
            : base(isp)
        {
            m_cache = new Dictionary<string, object>();
            m_fs = new ROFileSystem();
            m_g = g;
        }

        public T LoadContent<T>(string asset)
        {
            if (!m_loaders.ContainsKey(typeof(T)))
                return default(T);
            
            asset = asset.ToLower();
            if (m_cache.ContainsKey(asset))
            {
                if (m_cache[asset] != null)
                    return (T)m_cache[asset];
                else
                    m_cache.Remove(asset);
            }

            Stream fs = m_fs.LoadFile(asset);

            if (fs == null)
                return default(T);

            m_cache.Add(asset, m_loaders[typeof(T)].LoadContent(this, fs, asset));
            
            return (T)m_cache[asset];
        }

        public override T Load<T>(string assetName)
        {
            if (typeof(T) == typeof(Texture2D))
            {
                return LoadContent<T>(this.RootDirectory + "/" + assetName);
            }
            
            return base.Load<T>(assetName);
        }

        protected override Stream OpenStream(string assetName)
        {
            return GetStream(this.RootDirectory + "/" + assetName + ".xnb");
        }

        public Stream GetStream(string asset)
        {
            return m_fs.LoadFile(asset);
        }
    }
}
