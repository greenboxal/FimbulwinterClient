using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FimbulvetrEngine.IO
{
    public class FileSystemManager
    {
        public static FileSystemManager Instance { get; private set; }

        public Dictionary<string, IFileSystemFactory> Factories { get; private set; }
        public List<IFileSystem> FileSystems { get; private set; }

        public FileSystemManager()
        {
            if (Instance != null)
                throw new Exception("Only one instance of FileSystemManager is allowed, use the Instance property.");

            Factories = new Dictionary<string, IFileSystemFactory>();
            FileSystems = new List<IFileSystem>();

            RegisterDefaultFactories();

            Instance = this;
        }

        ~FileSystemManager()
        {
            foreach (IFileSystem fileSystem in FileSystems)
            {
                fileSystem.Close();
            }

            FileSystems.Clear();
        }

        private void RegisterDefaultFactories()
        {
            RegisterFileSystemFactory(new DefaultFileSystemFactory());
        }

        public void RegisterFileSystemFactory(IFileSystemFactory fileSystemFactory)
        {
            if (Factories.ContainsKey(fileSystemFactory.Type))
                throw new Exception("There is already a FileSystemFactory with type '" + fileSystemFactory.Type + "' registered.");

            Factories.Add(fileSystemFactory.Type, fileSystemFactory);
        }

        public void RegisterFileSystem(IFileSystem fileSystem)
        {
            FileSystems.Add(fileSystem);
        }

        public void RegisterFileSystem(string type, string path, string md5Check)
        {
            if (!Factories.ContainsKey(type))
                throw new Exception("There is no FileSystemFactory with type '" + type + "' registered.");

            RegisterFileSystem(Factories[type].Create(path, md5Check));
        }

        public IFileSystem FindFileSystem(string typeName)
        {
            return FileSystems.FirstOrDefault(fileSystem => String.Compare(fileSystem.Type, typeName, StringComparison.OrdinalIgnoreCase) == 0);
        }

        public void LoadAll()
        {
            foreach (IFileSystem fileSystem in FileSystems.Where(fileSystem => !fileSystem.Load()))
                FileSystems.Remove(fileSystem);
        }
                    
        public Stream OpenStream(string fileName)
        {
            return FileSystems.Select(fileSystem => fileSystem.OpenStream(fileName)).FirstOrDefault(result => result != null);
        }
    }
}
