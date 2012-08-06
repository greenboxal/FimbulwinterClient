using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Axiom.Core;
using System.IO;
using Axiom.Collections;

namespace FimbulwinterClient.Core.Content.World.Internals
{
    public class GatResourceManager : ResourceManager, ISingleton<GatResourceManager>
    {
        protected static GatResourceManager _instance;
        public static GatResourceManager Instance
        {
            get
            {
                return _instance;
            }
        }

        public GatResourceManager()
        {
            if (_instance == null)
            {
                _instance = this;
                ResourceType = "GatWorld";

                ResourceGroupManager.Instance.RegisterResourceManager(ResourceType, this);
            }
            else
            {
                throw new AxiomException("Cannot create another instance of {0}. Use Instance property instead", GetType().Name);
            }
        }

        public bool Initialize(params object[] args)
        {
            return true;
        }

        public GatWorld Load(Stream stream, string group)
        {
            RemoveAll();

            GatWorld world = (GatWorld)Create("GatWorld", "World", true, null, null);
            world.Load(stream);

            return world;
        }

        public override Resource Load(string name, string group, bool isManual, IManualResourceLoader loader, NameValuePairList loadParams, bool backgroundThread)
        {
            RemoveAll();

            return base.Load(name, group, isManual, loader, loadParams, backgroundThread);
        }

        protected override Resource _create(string name, ulong handle, string group, bool isManual, IManualResourceLoader loader, NameValuePairList createParams)
        {
            return new GatWorld(this, name, handle, group, isManual, loader, createParams);
        }
    }
}
