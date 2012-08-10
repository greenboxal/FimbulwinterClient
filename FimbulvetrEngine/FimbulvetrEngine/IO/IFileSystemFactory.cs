namespace FimbulvetrEngine.IO
{
    public interface IFileSystemFactory
    {
        string Type { get; }
        IFileSystem Create(string path, string md5Check);
    }
}
