using System.IO;

namespace FimbulvetrEngine.Content.Loaders
{
    public class StreamLoader : IContentLoader
    {
        public object LoadContent(ContentManager contentManager, string contentName, Stream stream)
        {
            return stream;
        }
    }
}
