using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Axiom.Core;
using FimbulwinterClient.Core.Content.World;
using FimbulwinterClient.Core.Content.World.Internals;

namespace FimbulwinterClient.Core
{
    public class Initialization
    {
        public static void DoInit()
        {
            Root.Instance.AddSceneManagerFactory(new RagnarokSceneManagerFactory());

            new RswResourceManager();
            new GatResourceManager();
            new GndResourceManager();
        }
    }
}
