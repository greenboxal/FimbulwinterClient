using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulwinterClient.Content;

namespace FimbulwinterClient.IO.ContentLoaders
{
    public class SpriteLoader : IContentLoader
    {
        public object LoadContent(ROContentManager rcm, System.IO.Stream s, string fn)
        {
            Sprite spr = new Sprite();

            if (!spr.Load(rcm.Game.GraphicsDevice, s))
                return null;

            s.Close();

            return spr;
        }
    }
}
