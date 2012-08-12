using System.IO;
using FimbulvetrEngine.IO;

namespace FimbulvetrEngine.Content.Loaders
{
    public class StreamLoader : IContentLoader
    {
        public object LoadContent(ContentManager contentManager, string contentName, bool background)
        {
            Stream stream = FileSystemManager.Instance.OpenStream(contentName);

            if (stream == null)
                return null;

            return stream;
        }
    }
}
