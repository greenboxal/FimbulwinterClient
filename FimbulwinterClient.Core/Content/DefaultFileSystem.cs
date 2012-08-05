using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IrrlichtLime.IO;

namespace FimbulwinterClient.Core.Content
{
    public class DefaultFileSystem : IFileSystem
    {
        public Stream LoadStream(string filename)
        {
            if (File.Exists(filename))
                return new FileStream(filename, FileMode.Open);

            return null;
        }

        public ReadFile LoadReadFile(string filename)
        {
            return SharedInformation.Device.FileSystem.CreateReadFile(filename);
        }
    }
}
