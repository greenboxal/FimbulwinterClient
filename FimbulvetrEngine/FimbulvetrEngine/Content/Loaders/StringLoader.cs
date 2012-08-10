using System.IO;

namespace FimbulvetrEngine.Content.Loaders
{
    public class StringLoader : IContentLoader
    {
        public object LoadContent(ContentManager contentManager, string contentName, Stream stream)
        {
            string content = new StreamReader(stream).ReadToEnd();

            contentManager.CacheContent(contentName, content);
            stream.Close();

            return content;
        }
    }
}
