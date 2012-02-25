using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulwinterClient.Content;

namespace FimbulwinterClient.IO.ContentLoaders
{
    public class SpriteActionLoader : IContentLoader
    {
        public object LoadContent(ROContentManager rcm, System.IO.Stream s, string fn)
        {
            Sprite spr = rcm.LoadContent<Sprite>(fn.Replace(".act", ".spr"));

            if (spr == null)
                return null;

            SpriteAction act = new SpriteAction(spr);

            if (!act.Load(s))
                return null;

            s.Close();

            return act;
        }
    }
}
