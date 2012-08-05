using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IrrlichtLime.Scene;
using FimbulwinterClient.Core;
using FimbulwinterClient.Core.Assets;
using System.Threading;

namespace FimbulwinterClient.GameModes
{
    public class LoadingGameMode : SceneNode
    {
        // Map
        private string _mapName;
        private int _state;
        private Map _map;

        public LoadingGameMode(string mapName)
            : base(SharedInformation.Scene.RootNode, SharedInformation.Scene)
        {
            OnRegisterSceneNode += LoadingGameMode_OnRegisterSceneNode;
            OnRender += LoadingGameMode_OnRender;

            _mapName = mapName;
            _state = 0;
        }

        void LoadingGameMode_OnRegisterSceneNode()
        {
            if (Visible)
                SharedInformation.Scene.RegisterNodeForRendering(this);
        }

        void LoadingGameMode_OnRender()
        {
            if (_state == 0)
            {
                new Thread(_Load).Start();
                _state++;
            }
            else if (_state == 1)
            {

            }
            else if (_state == 2)
            {
                Ragnarok.Instance.ChangeGameMode(new WorldGameMode(_map));
                _state++;
            }
        }

        private void _Load()
        {
            _map = SharedInformation.ContentManager.Load<Map>(@"data\" + _mapName + ".gat");
            _state++;
        }

        public void Dispose()
        {

        }
    }
}
