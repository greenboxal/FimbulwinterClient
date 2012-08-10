using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FimbulvetrEngine.IO;

namespace FimbulwinterClient.Core.IO
{
    public class GrfFileSystemFactory : IFileSystemFactory
    {
        public string Type
        {
            get { return "Grf"; }
        }

        public IFileSystem Create(string path, string md5Check)
        {
            return new GrfFileSystem(path);
        }
    }
}