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
        private static Dictionary<Type, IContentLoader> _loaders;
        public static Dictionary<Type, IContentLoader> Loaders
        {
            get { return ROContentManager._loaders; }
        }

        static ROContentManager()
        {
            _loaders = new Dictionary<Type, IContentLoader>();

            _loaders.Add(typeof(Stream), new StreamLoader());
            _loaders.Add(typeof(Texture2D), new Texture2DLoader());
            _loaders.Add(typeof(SoundEffect), new SoundEffectLoader());
            _loaders.Add(typeof(Sprite), new SpriteLoader());
            _loaders.Add(typeof(SpriteAction), new SpriteActionLoader());
            _loaders.Add(typeof(RsmModel), new RsmModelLoader());
            _loaders.Add(typeof(Lub), new LubLoader());
        }

        private Dictionary<string, object> _cache;
        public Dictionary<string, object> Cache
        {
            get { return _cache; }
        }

        private ROFileSystem _fs;
        public ROFileSystem FileSystem
        {
            get { return _fs; }
        }

        private ROClient _game;
        public ROClient Game
        {
            get { return _game; }
            set { _game = value; }
        }

        public ROContentManager(IServiceProvider isp, ROClient g)
            : base(isp)
        {
            _cache = new Dictionary<string, object>();
            _fs = new ROFileSystem();
            _game = g;
        }

        public T LoadContent<T>(string asset)
        {
            if (!_loaders.ContainsKey(typeof(T)))
                return default(T);
            
            asset = asset.ToLower();
            if (_cache.ContainsKey(asset))
            {
                if (_cache[asset] != null)
                    return (T)_cache[asset];
                else
                    _cache.Remove(asset);
            }

            Stream fs = _fs.LoadFile(asset);

            if (fs == null)
                return default(T);

            _cache.Add(asset, _loaders[typeof(T)].LoadContent(this, fs, asset));
            
            return (T)_cache[asset];
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
            return _fs.LoadFile(asset);
        }
    }
}
