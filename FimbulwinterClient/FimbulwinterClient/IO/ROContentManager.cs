using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using FimbulwinterClient.IO.ContentLoaders;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

namespace FimbulwinterClient.IO
{
    public interface IContentLoader
    {
        object LoadContent(ROContentManager rcm, Stream s);
    }

    public class ROContentManager : GameComponent
    {
        private static Dictionary<Type, IContentLoader> m_loaders;
        public static Dictionary<Type, IContentLoader> Loaders
        {
            get { return ROContentManager.m_loaders; }
        }

        static ROContentManager()
        {
            m_loaders = new Dictionary<Type, IContentLoader>();

            m_loaders.Add(typeof(Texture2D), new Texture2DLoader());
            m_loaders.Add(typeof(SoundEffect), new SoundEffectLoader());
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

        private GraphicsDevice m_gd;
        public GraphicsDevice GraphicsDevice
        {
            get { return m_gd; }
            set { m_gd = value; }
        }

        public ROContentManager(Game g)
            : base(g)
        {
            m_cache = new Dictionary<string, object>();
            m_fs = new ROFileSystem();
            m_gd = g.GraphicsDevice;
        }

        public T LoadContent<T>(string asset) where T : class
        {
            if (!m_loaders.ContainsKey(typeof(T)))
                return null;
            
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
                return null;

            m_cache.Add(asset, m_loaders[typeof(T)].LoadContent(this, fs));
            
            return (T)m_cache[asset];
        }
    }
}
