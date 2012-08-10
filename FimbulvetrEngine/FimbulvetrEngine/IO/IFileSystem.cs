using System.IO;

namespace FimbulvetrEngine.IO
{
    public interface IFileSystem
    {
        string Type { get; }
        bool Load();
        Stream OpenStream(string name);
        void Close();
    }
}
