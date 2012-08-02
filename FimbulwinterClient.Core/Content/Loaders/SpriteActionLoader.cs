using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FimbulwinterClient.Core.Assets;

namespace FimbulwinterClient.Core.Content.Loaders
{
    public class SpriteActionLoader : IContentLoader
    {
        public object Load(Stream stream, string assetName)
        {
            Sprite spr = SharedInformation.ContentManager.Load<Sprite>(assetName.Replace(".act", ".spr"));

            if (spr == null)
                return null;

            SpriteAction act = new SpriteAction(spr);

            if (!act.Load(stream))
                return null;

            stream.Close();

            return act;
        }
    }
}
