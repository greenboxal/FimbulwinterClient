using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Threading;
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
        public Queue<Tuple<WaitCallback, object>> FinalBackgroundQueue { get; private set; }

        public ContentManager()
        {
            if (Instance != null)
                throw new Exception("Only one instance of ContentManager is allowed, use the Instance property.");

            ContentLoaders = new Dictionary<Type, IContentLoader>();
            Cache = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            FinalBackgroundQueue = new Queue<Tuple<WaitCallback, object>>();

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

        public T Load<T>(string contentName, bool background = false)
        {
            lock (Cache)
            {
                object cached;
                if (Cache.TryGetValue(contentName, out cached))
                {
                    return (T)cached;
                }
            }

            return LoadNewContent<T>(contentName, background);
        }

        private T LoadNewContent<T>(string contentName, bool background)
        {
            if (!ContentLoaders.ContainsKey(typeof(T)))
                return default(T);

            return (T)ContentLoaders[typeof(T)].LoadContent(this, contentName, background);
        }

        public void CacheContent(string contentName, object value)
        {
            lock (Cache)
            {
                Cache[contentName] = value;
            }
        }

        public void EnqueueBackgroundLoading(WaitCallback action, object state = null)
        {
            ThreadPool.QueueUserWorkItem(action, state);
        }

        public void FinalizeBackgroundLoading(WaitCallback action, object state = null)
        {
            lock (FinalBackgroundQueue)
            {
                FinalBackgroundQueue.Enqueue(new Tuple<WaitCallback, object>(action, state));
            }
        }

        public void PoolBackgroundLoading(int max = 0)
        {
            int count = 0;

            while (true)
            {
                Tuple<WaitCallback, object> callback = null;

                lock (FinalBackgroundQueue)
                {
                    if (FinalBackgroundQueue.Count > 0)
                        callback = FinalBackgroundQueue.Dequeue();
                }

                if (callback == null)
                    break;

                callback.Item1(callback.Item2);

                count++;
                if (max != 0 && count >= max)
                    break;
            }
        }
    }
}
