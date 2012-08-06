using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Axiom.Core;
using Axiom.Graphics;
using Axiom.Core.Collections;

namespace FimbulwinterClient.Core.Content.World
{
    public class RagnarokSceneManager : SceneManager
    {
        private RswWorld _rswWorld;
        public RswWorld RswWorld
        {
            get { return _rswWorld; }
        }

        private GndWorld _gndWorld;
        public GndWorld GndWorld
        {
            get { return _gndWorld; }
        }

        private GatWorld _gatWorld;
        public GatWorld GatWorld
        {
            get { return _gatWorld; }
        }

        public override string TypeName
        {
            get { return "RagnarokSceneManager"; }
        }

        public RagnarokSceneManager(string name)
            : base(name)
        {
        }

        public override void SetWorldGeometry(string filename)
        {
            _rswWorld = RswResourceManager.Instance.Load(ResourceGroupManager.Instance.OpenResource(@"data\" + filename + ".rsw", "World"), "World");
            _gatWorld = GatResourceManager.Instance.Load(ResourceGroupManager.Instance.OpenResource(@"data\" + filename + ".gat", "World"), "World");
            _gndWorld = GndResourceManager.Instance.Load(ResourceGroupManager.Instance.OpenResource(@"data\" + filename + ".gnd", "World"), "World");
        }

        public override void FindVisibleObjects(Camera camera, bool onlyShadowCasters)
        {
            renderQueue.AddRenderable(_gndWorld);
        }

        protected override void RenderSingleObject(IRenderable renderable, Pass pass, bool doLightIteration, LightList manualLightList)
        {
            if (renderable is GndWorld)
            {
                _gndWorld.Render();
            }
        }

        private void RenderStaticGeometry()
        {
            
        }
    }

    public class RagnarokSceneManagerFactory : SceneManagerFactory
    {
        public override SceneManager CreateInstance(string name)
        {
            return new RagnarokSceneManager(name);
        }

        public override void DestroyInstance(SceneManager instance)
        {
            instance.ClearScene();
        }

        protected override void InitMetaData()
        {
            metaData.typeName = "RagnarokSceneManager";
            metaData.description = "Ragnarök Online RSW SceneManager.";
            metaData.sceneTypeMask = SceneType.ExteriorClose;
            metaData.worldGeometrySupported = true;
        }
    }
}
