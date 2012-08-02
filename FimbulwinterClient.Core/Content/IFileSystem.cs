using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FimbulwinterClient.Core.Content
{
    public interface IFileSystem
    {
        Stream Load(string filename);
    }
}
