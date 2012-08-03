using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using FimbulwinterClient.Core.Content.Loaders;
using FimbulwinterClient.Core.Assets;
using Microsoft.Xna.Framework.Audio;

namespace FimbulwinterClient.Core.Content
{
    public class AdvancedContentManager : ContentManager
    {
        private static List<IFileSystem> _fileSystems;
        public static List<IFileSystem> FileSystems
        {
            get { return AdvancedContentManager._fileSystems; }
        }

        private static Dictionary<Type, IContentLoader> _contentLoaders;
        public static Dictionary<Type, IContentLoader> ContentLoaders
        {
            get { return _contentLoaders; }
        }

        static AdvancedContentManager()
        {
            _fileSystems = new List<IFileSystem>();
            _contentLoaders = new Dictionary<Type, IContentLoader>();

            _fileSystems.Add(new DefaultFileSystem());
            _fileSystems.Add(new GrfFileSystem());

            _contentLoaders.Add(typeof(Stream), new StreamLoader());
            _contentLoaders.Add(typeof(Texture2D), new Texture2DLoader());
            _contentLoaders.Add(typeof(SoundEffect), new SoundEffectLoader());
            _contentLoaders.Add(typeof(Sprite), new SpriteLoader());
            _contentLoaders.Add(typeof(SpriteAction), new SpriteActionLoader());
            _contentLoaders.Add(typeof(Palette), new PaletteLoader());
            _contentLoaders.Add(typeof(Map), new MapLoader());
            _contentLoaders.Add(typeof(GravityModel), new GravityModelLoader());
        }

        private GraphicsDevice _graphicsDevice;
        public GraphicsDevice GraphicsDevice
        {
            get { return _graphicsDevice; }
        }

        private Hashtable _cache;
        public Hashtable Cache
        {
            get { return _cache; }
        }

        public AdvancedContentManager(IServiceProvider serviceProvider, GraphicsDevice graphicsDevice)
            : base(serviceProvider, "data")
        {
            _graphicsDevice = graphicsDevice;
            _cache = CollectionsUtil.CreateCaseInsensitiveHashtable();
        }

        public override T Load<T>(string assetName)
        {
            T value = default(T);
            Stream stream;

            T cached = (T)_cache[assetName];

            if (cached != null)
                return cached;

            if (assetName.EndsWith(".xnb"))
                return base.Load<T>(Path.Combine(RootDirectory, assetName));

            stream = OpenStream(assetName);

            if (stream == null)
                return value;

            if (_contentLoaders.ContainsKey(typeof(T)))
                value = (T)_contentLoaders[typeof(T)].Load(stream, assetName);

            _cache.Add(assetName, value);

            return value;
        }

        protected override Stream OpenStream(string assetName)
        {
            foreach (IFileSystem fs in _fileSystems)
            {
                Stream stream = fs.Load(assetName);

                if (stream != null)
                    return stream;
            }

            return null;
        }
    }
}
