using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FimbulwinterClient.Core.Content
{
    public class DefaultFileSystem : IFileSystem
    {
        public Stream Load(string filename)
        {
            if (File.Exists(filename))
                return new FileStream(filename, FileMode.Open);

            return null;
        }
    }
}
