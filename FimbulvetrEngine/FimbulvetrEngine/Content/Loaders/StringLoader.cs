using System.IO;
using FimbulvetrEngine.IO;

namespace FimbulvetrEngine.Content.Loaders
{
    public class StringLoader : IContentLoader
    {
        public object LoadContent(ContentManager contentManager, string contentName, bool background)
        {
            Stream stream = FileSystemManager.Instance.OpenStream(contentName);

            if (stream == null)
                return null;

            string content = new StreamReader(stream).ReadToEnd();

            contentManager.CacheContent(contentName, content);
            stream.Close();

            return content;
        }
    }
}
