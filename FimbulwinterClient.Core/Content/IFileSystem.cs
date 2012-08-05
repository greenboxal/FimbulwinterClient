using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IrrlichtLime.IO;

namespace FimbulwinterClient.Core.Content
{
    public interface IFileSystem
    {
        Stream LoadStream(string filename);
        ReadFile LoadReadFile(string filename);
    }
}
