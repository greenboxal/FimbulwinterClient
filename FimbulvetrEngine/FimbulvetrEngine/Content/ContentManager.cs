using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using FimbulvetrEngine.Content.Loaders;
using FimbulvetrEngine.Graphics;
using FimbulvetrEngine.IO;
using QuickFont;

namespace FimbulvetrEngine.Content
{
    public class ContentManager
    {
        public static ContentManager Instance { get; private set; }

        public Dictionary<Type, IContentLoader> ContentLoaders { get; private set; }
        public Dictionary<string, object> Cache { get; private set; }

        public ContentManager()
        {
            if (Instance != null)
                throw new Exception("Only one instance of ContentManager is allowed, use the Instance property.");

            ContentLoaders = new Dictionary<Type, IContentLoader>();
            Cache = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            RegisterDefaultLoaders();

            Instance = this;
        }

        private void RegisterDefaultLoaders()
        {
            RegisterLoader<Stream>(new StreamLoader());
            RegisterLoader<String>(new StringLoader());
            RegisterLoader<Texture2D>(new Texture2DLoader());
            RegisterLoader<QFont>(new QFontLoader());
        }

        public void RegisterLoader<T>(IContentLoader loader)
        {
            if (!ContentLoaders.ContainsKey(typeof(T)))
                ContentLoaders.Add(typeof(T), loader);
        }

        public T Load<T>(string contentName)
        {
            object cached;

            if (Cache.TryGetValue(contentName, out cached))
            {
                return (T)cached;
            }

            return LoadNewContent<T>(contentName);
        }

        private T LoadNewContent<T>(string contentName)
        {
            if (!ContentLoaders.ContainsKey(typeof(T)))
                return default(T);

            Stream stream = FileSystemManager.Instance.OpenStream(contentName);

            if (stream == null)
                return default(T);

            return (T)ContentLoaders[typeof(T)].LoadContent(this, contentName, stream);
        }

        public void CacheContent(string contentName, object value)
        {
            Cache.Add(contentName, value);
        }
    }
}
