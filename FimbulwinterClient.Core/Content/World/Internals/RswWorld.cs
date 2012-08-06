using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Axiom.Collections;
using Axiom.Core;

namespace FimbulwinterClient.Core.Content.World.Internals
{
    public class RswWorld : Resource
    {
        public RswWorld(ResourceManager parent, string name, ulong handle, string group, bool isManual, IManualResourceLoader loader, NameValuePairList createParams)
            : base(parent, name, handle, group, isManual, loader)
        {
        }

        public void Load(Stream stream)
        {
            
        }

        protected override void load()
        {
            Stream stream = ResourceGroupManager.Instance.OpenResource(Name);
            Load(stream);
            stream.Close();
        }

        protected override void unload()
        {
            
        }
    }
}
