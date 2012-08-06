using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Axiom.Core;
using System.IO;
using Axiom.Collections;

namespace FimbulwinterClient.Core.Content.World
{
    public class RswResourceManager : ResourceManager, ISingleton<RswResourceManager>
    {
        protected static RswResourceManager _instance;
        public static RswResourceManager Instance
        {
            get
            {
                return _instance;
            }
        }

        public RswResourceManager()
        {
            if (_instance == null)
            {
                _instance = this;
                ResourceType = "RswWorld";

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

        public RswWorld Load(Stream stream, string group)
        {
            RemoveAll();

            RswWorld world = (RswWorld)Create("RswWorld", "World", true, null, null);
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
            return new RswWorld(this, name, handle, group, isManual, loader, createParams);
        }
    }
}
