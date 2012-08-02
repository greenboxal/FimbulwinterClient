using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FimbulwinterClient.Core.Content
{
    public interface IContentLoader
    {
        object Load(Stream stream, string assetName);
    }
}
