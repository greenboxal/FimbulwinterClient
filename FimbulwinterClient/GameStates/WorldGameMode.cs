using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulwinterClient.Core;
using Axiom.Core;
using Axiom.Graphics;
using SharpInputSystem;
using Axiom.Math;

namespace FimbulwinterClient.GameStates
{
    public class WorldGameState : GameState
    {
        private string _worldName;

        public string WorldName
        {
            get { return _worldName; }
        }

        private Camera _camera;

        public Camera Camera
        {
            get { return _camera; }
        }

        private Viewport _viewport;

        public Viewport Viewport
        {
            get { return _viewport; }
        }

        public WorldGameState(string worldName)
        {
            _worldName = worldName;
        }

        protected override void CreateSceneManager()
        {
            SceneManager = Root.Instance.CreateSceneManager("RagnarokSceneManager", _worldName + "RSWInstance");
        }

        protected override void LocateResources()
        {
            ResourceGroupManager.Instance.CreateResourceGroup("World");
        }

        protected override void LoadResources()
        {
            ResourceGroupManager.Instance.LinkWorldGeometryToResourceGroup("World", _worldName, SceneManager);
            ResourceGroupManager.Instance.InitializeResourceGroup("World");
            ResourceGroupManager.Instance.LoadResourceGroup("World", false, true);
        }

        protected override void UnloadResources()
        {
            ResourceGroupManager.Instance.UnloadResourceGroup("World");
        }

        protected override void SetupView()
        {
            _camera = SceneManager.CreateCamera("MainCamera");
            _viewport = Window.AddViewport(_camera);

            _camera.AspectRatio = _viewport.ActualWidth/(Real) _viewport.ActualHeight;
            _camera.Near = 1.0F;
            _camera.Far = 5000.0F;
        }
    }
}