using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FimbulvetrEngine.Content;
using FimbulvetrEngine.IO;
using FimbulwinterClient.Core.Graphics;

namespace FimbulwinterClient.Core.Content.Loaders
{
    public class RsmModelLoader : IContentLoader
    {
        public object LoadContent(ContentManager contentManager, string contentName, bool background)
        {
            Stream stream = FileSystemManager.Instance.OpenStream(contentName);

            if (stream == null)
                return null;

            RsmModel model = new RsmModel();

            if (!model.Load(stream))
                return null;

            return model;
        }
    }
}
