using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using FimbulwinterClient.Core.Content.Loaders;
using IrrlichtLime.Video;
using FimbulwinterClient.Core.Assets;
using IrrlichtLime.IO;

namespace FimbulwinterClient.Core.Content
{
    public class AdvancedContentManager
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
            _contentLoaders.Add(typeof(String), new StringLoader());
            _contentLoaders.Add(typeof(Texture), new TextureLoader());
            /*_contentLoaders.Add(typeof(SoundEffect), new SoundEffectLoader());
            _contentLoaders.Add(typeof(Sprite), new SpriteLoader());
            _contentLoaders.Add(typeof(SpriteAction), new SpriteActionLoader());
            _contentLoaders.Add(typeof(Palette), new PaletteLoader());*/
            _contentLoaders.Add(typeof(Map), new MapLoader());
            //_contentLoaders.Add(typeof(GravityModel), new GravityModelLoader());*/
        }

        private Hashtable _cache;
        public Hashtable Cache
        {
            get { return _cache; }
        }

        public AdvancedContentManager()
        {
            _cache = CollectionsUtil.CreateCaseInsensitiveHashtable();
        }

        public T Load<T>(string assetName)
        {
            T value = default(T);

            object cached = (T)_cache[assetName];

            if (cached != null)
                return (T)cached;

            if (_contentLoaders.ContainsKey(typeof(T)))
            {
                IContentLoader loader = _contentLoaders[typeof(T)];

                if (loader.Type == LoadType.Stream)
                {
                    Stream stream = OpenStream(assetName);

                    if (stream == null)
                    {
#if DEBUG
                        SharedInformation.Logger.Write("Loading " + assetName + "..." + " Error!");
#endif
                        return value;
                    }

#if DEBUG
                    SharedInformation.Logger.Write("Loading " + assetName + "...");
#endif

                    value = (T)loader.Load(stream, assetName);
                }
                else if (loader.Type == LoadType.ReadFile)
                {
                    ReadFile readFile = OpenReadFile(assetName);

                    if (readFile == null)
                    {
#if DEBUG
                        SharedInformation.Logger.Write("Loading " + assetName + "..." + " Error!");
#endif
                        return value;
                    }

#if DEBUG
                    SharedInformation.Logger.Write("Loading " + assetName + "...");
#endif

                    value = (T)loader.Load(readFile, assetName);
                    readFile.Drop();
                }
            }

            if (value == null)
                return value;

            _cache.Add(assetName, value);

            return value;
        }

        protected Stream OpenStream(string assetName)
        {
            foreach (IFileSystem fs in _fileSystems)
            {
                Stream stream = fs.LoadStream(assetName);

                if (stream != null)
                    return stream;
            }

            return null;
        }

        protected ReadFile OpenReadFile(string assetName)
        {
            foreach (IFileSystem fs in _fileSystems)
            {
                ReadFile readFile = fs.LoadReadFile(assetName);

                if (readFile != null)
                    return readFile;
            }

            return null;
        }
    }
}
