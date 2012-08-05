using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IrrlichtLime.IO;

namespace FimbulwinterClient.Core.Content
{
    public enum LoadType
    {
        Stream,
        ReadFile
    }

    public interface IContentLoader
    {
        LoadType Type { get; }
        object Load(Stream stream, string assetName);
        object Load(ReadFile readFile, string assetName);
    }
}
