using System.IO;

namespace FimbulvetrEngine.Content
{
    public interface IContentLoader
    {
        object LoadContent(ContentManager contentManager, string contentName, Stream stream);
    }
}
