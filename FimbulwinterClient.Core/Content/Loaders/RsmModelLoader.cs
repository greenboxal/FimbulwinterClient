using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FimbulvetrEngine;
using FimbulvetrEngine.Content;
using FimbulvetrEngine.IO;
using FimbulwinterClient.Core.Graphics;

namespace FimbulwinterClient.Core.Content.Loaders
{
    public class RsmModelLoader : IContentLoader
    {
        public object LoadContent(ContentManager contentManager, string contentName, bool background)
        {
            RsmModel result = new RsmModel();

            Dispatcher.Instance.DispatchTask(o =>
            {
                Stream stream = FileSystemManager.Instance.OpenStream(contentName);

                if (stream == null)
                    return;

                result.Load(stream);
            }, background);

            return result;
        }
    }
}
