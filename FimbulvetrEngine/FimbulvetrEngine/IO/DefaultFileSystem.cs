using System.IO;

namespace FimbulvetrEngine.IO
{
    public class DefaultFileSystem : IFileSystem
    {
        public string Type
        {
            get { return "Folder"; }
        }

        public string Root { get; private set; }

        public DefaultFileSystem()
            : this(".")
        {

        }

        public DefaultFileSystem(string root)
        {
            Root = Path.GetFullPath(root);
        }

        public bool Load()
        {
            return Directory.Exists(Root);
        }

        public Stream OpenStream(string name)
        {
            string path = Path.Combine(Root, name);

            return File.Exists(path) ? new FileStream(path, FileMode.Open) : null;
        }

        public void Close()
        {
            
        }
    }
}
