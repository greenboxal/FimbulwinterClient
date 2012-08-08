using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Axiom.Core;
using Axiom.Graphics;
using Axiom.Core.Collections;
using FimbulwinterClient.Core.Content.World.Internals;

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

        private SceneNode _groundNode;

        public SceneNode GroundNode
        {
            get { return _groundNode; }
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
            _rswWorld =
                RswResourceManager.Instance.Load(
                    ResourceGroupManager.Instance.OpenResource(@"data\" + filename + ".rsw", "World"), "World");
            _gatWorld =
                GatResourceManager.Instance.Load(
                    ResourceGroupManager.Instance.OpenResource(@"data\" + filename + ".gat", "World"), "World");
            _gndWorld =
                GndResourceManager.Instance.Load(
                    ResourceGroupManager.Instance.OpenResource(@"data\" + filename + ".gnd", "World"), "World");

            _groundNode = RootSceneNode.CreateChildSceneNode("GroundRoot");
            _groundNode.AttachObject(new GroundRenderable(_gndWorld, _rswWorld));
        }

        protected override void RenderSingleObject(IRenderable renderable, Pass pass, bool doLightIteration,
                                                   LightList manualLightList)
        {
            if (typeof (GroundRenderable) == renderable.GetType())
            {
                GroundRenderable gr = renderable as GroundRenderable;
                RenderOperation op = new RenderOperation();

                op.vertexData = gr.Vertices;
                op.operationType = OperationType.TriangleList;
                op.useIndices = true;

                for (int i = 0; i < gr.Materials.Length; i++)
                {
                    Technique t = gr.Materials[i].GetTechnique(0);

                    op.indexData = new IndexData();
                    op.indexData.indexBuffer = gr.Indexes[i];
                    op.indexData.indexStart = 0;
                    op.indexData.indexCount = gr.Indexes[i].IndexCount;

                    for (int n = 0; n < t.PassCount; n++)
                    {
                        SetPass(t.GetPass(n));
                        Root.Instance.RenderSystem.Render(op);
                    }
                }
            }
            else
            {
                base.RenderSingleObject(renderable, pass, doLightIteration, manualLightList);
            }
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