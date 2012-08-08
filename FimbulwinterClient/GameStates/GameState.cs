using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulwinterClient.Core;
using System.Threading;
using Axiom.Core;
using SharpInputSystem;
using Axiom.Graphics;

namespace FimbulwinterClient.GameStates
{
    public abstract class GameState : IWindowEventListener, IKeyboardListener, IMouseListener
    {
        protected RenderWindow Window;
        protected Keyboard Keyboard;
        protected Mouse Mouse;
        protected bool ResourcesLoaded;
        protected bool ContentSetup;

        private SceneManager _sceneManager;

        public SceneManager SceneManager
        {
            get { return this._sceneManager; }
            protected set { this._sceneManager = value; }
        }

        private bool _done;

        public bool IsDone
        {
            get { return this._done; }
            protected set { this._done = value; }
        }

        public GameState()
        {
            Window = null;
            ResourcesLoaded = false;
            ContentSetup = false;

            _sceneManager = null;
            _done = true;
        }

        public virtual void Setup(RenderWindow window, Keyboard keyboard, Mouse mouse)
        {
            Window = window;
            Keyboard = keyboard;
            Mouse = mouse;

            LocateResources();
            CreateSceneManager();
            SetupView();
            LoadResources();
            ResourcesLoaded = true;
            SetupContent();
            ContentSetup = true;

            _done = false;
        }

        public virtual void Shutdown()
        {
            if (this._sceneManager != null)
            {
                this._sceneManager.ClearScene();
            }

            if (this.ContentSetup)
            {
                CleanupContent();
            }
            this.ContentSetup = false;

            if (this.ResourcesLoaded)
            {
                UnloadResources();
            }
            this.ResourcesLoaded = false;

            if (this._sceneManager != null)
            {
                Root.Instance.DestroySceneManager(this._sceneManager);
            }
            this._sceneManager = null;

            this._done = true;
        }

        public virtual bool FrameStarted(FrameEventArgs evt)
        {
            return false;
        }

        public virtual bool FrameRenderingQueued(FrameEventArgs evt)
        {
            return false;
        }

        public virtual bool FrameEnded(FrameEventArgs evt)
        {
            return false;
        }

        public virtual void WindowMoved(RenderWindow rw)
        {
        }

        public virtual void WindowResized(RenderWindow rw)
        {
        }

        public virtual bool WindowClosing(RenderWindow rw)
        {
            return true;
        }

        public virtual void WindowClosed(RenderWindow rw)
        {
        }

        public virtual void WindowFocusChange(RenderWindow rw)
        {
        }

        public virtual bool KeyPressed(KeyEventArgs evt)
        {
            return true;
        }

        public virtual bool KeyReleased(KeyEventArgs evt)
        {
            return true;
        }

        public virtual bool MouseMoved(MouseEventArgs evt)
        {
            return true;
        }

        public virtual bool MousePressed(MouseEventArgs evt, MouseButtonID id)
        {
            return true;
        }

        public virtual bool MouseReleased(MouseEventArgs evt, MouseButtonID id)
        {
            return true;
        }

        protected virtual void LocateResources()
        {
        }

        protected virtual void LoadResources()
        {
        }

        protected virtual void CreateSceneManager()
        {
            this._sceneManager = Root.Instance.CreateSceneManager("DefaultSceneManager");
        }

        protected virtual void SetupView()
        {
        }

        protected virtual void SetupContent()
        {
        }

        protected virtual void CleanupContent()
        {
        }

        protected virtual void UnloadResources()
        {
        }
    }
}