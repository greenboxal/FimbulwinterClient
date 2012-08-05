using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulwinterClient.Core;
using IrrlichtLime.Scene;
using FimbulwinterClient.Core.Assets;
using IrrlichtLime.Core;
using IrrlichtLime.Video;

namespace FimbulwinterClient.GameModes
{
    public class WorldGameMode : SceneNode
    {
        private AABBox _boundingBox;

        private Map _map;
        public Map Map
        {
            get { return _map; }
        }

        public WorldGameMode(Map map)
            : base(SharedInformation.Scene.RootNode, SharedInformation.Scene)
        {
            _map = map;

            _boundingBox = new AABBox();
            _boundingBox.AddInternalBox(_map.BoundingBox);

            OnGetBoundingBox += WorldGameMode_OnGetBoundingBox;
            OnRegisterSceneNode += WorldGameMode_OnRegisterSceneNode;
            OnRender += WorldGameMode_OnRender;

            SharedInformation.Scene.AddCameraSceneNodeFPS(this);
        }

        AABBox WorldGameMode_OnGetBoundingBox()
        {
            return _boundingBox;
        }

        void WorldGameMode_OnRegisterSceneNode()
        {
            if (Visible)
                SharedInformation.Scene.RegisterNodeForRendering(this);
        }

        void WorldGameMode_OnRender()
        {
            _map.Update();

            SharedInformation.Graphics.SetTransform(TransformationState.World, AbsoluteTransformation);
            _map.Render();
        }

        public void Dispose()
        {
            
        }
    }
}
