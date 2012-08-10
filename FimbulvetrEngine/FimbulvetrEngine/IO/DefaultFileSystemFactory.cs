namespace FimbulvetrEngine.IO
{
    public class DefaultFileSystemFactory : IFileSystemFactory
    {
        public string Type
        {
            get { return "Folder"; }
        }

        public IFileSystem Create(string path, string md5Check)
        {
            return new DefaultFileSystem(path);
        }
    }
}