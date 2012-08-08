#region Namespace Declarations

using System;
using System.Linq;
using Axiom.Core;
using Axiom.Framework.Configuration;
using Axiom.Graphics;
using SharpInputSystem;
using Vector3 = Axiom.Math.Vector3;

#endregion Namespace Declarations

namespace Axiom.Framework
{
    public abstract class Game : IDisposable, IWindowEventListener
    {
        protected Root Engine;
        protected IConfigurationManager ConfigurationManager;
        protected ResourceGroupManager Content;
        protected SceneManager SceneManager;
        protected Camera Camera;
        protected Viewport Viewport;
        protected RenderWindow Window;
        protected Axiom.Graphics.RenderSystem RenderSystem;
        protected SharpInputSystem.InputManager InputManager;

        public virtual void Run()
        {
            ReadConfiguration();
            PreInitialize();
            LoadConfiguration();
            Initialize();
            CreateRenderSystem();
            CreateRenderWindow();
            LoadContent();
            CreateSceneManager();
            CreateCamera();
            CreateViewports();
            CreateInput();
            CreateScene();
            this.Engine.StartRendering();
        }

        protected virtual void ReadConfiguration()
        {
            this.ConfigurationManager = new DefaultConfigurationManager();
        }

        private void PreInitialize()
        {
            // instantiate the Root singleton
            this.Engine = new Root(this.ConfigurationManager.LogFilename);

            // add event handlers for frame events
            this.Engine.FrameStarted += Engine_FrameRenderingQueued;
        }

        public virtual void LoadConfiguration()
        {
            this.ConfigurationManager.RestoreConfiguration(this.Engine);
        }


        private void Engine_FrameRenderingQueued(object source, FrameEventArgs e)
        {
            Update(e.TimeSinceLastFrame);
        }

        public virtual void Initialize()
        {
        }

        public virtual void CreateRenderSystem()
        {
            if (this.Engine.RenderSystem == null)
            {
                this.RenderSystem = this.Engine.RenderSystem = this.Engine.RenderSystems.First().Value;
            }
            else
            {
                this.RenderSystem = this.Engine.RenderSystem;
            }
        }

        public virtual void CreateRenderWindow()
        {
            this.Window = Root.Instance.Initialize(true, "Axiom Framework Window");

            WindowEventMonitor.Instance.RegisterListener(this.Window, this);
        }

        public virtual void LoadContent()
        {
            ResourceGroupManager.Instance.InitializeAllResourceGroups();
        }

        public virtual void CreateSceneManager()
        {
            // Get the SceneManager, a generic one by default
            this.SceneManager = this.Engine.CreateSceneManager("DefaultSceneManager", "GameSMInstance");
            this.SceneManager.ClearScene();
        }

        public virtual void CreateCamera()
        {
            // create a camera and initialize its position
            this.Camera = this.SceneManager.CreateCamera("MainCamera");
            this.Camera.Position = new Vector3(0, 0, 500);
            this.Camera.LookAt(new Vector3(0, 0, -300));

            // set the near clipping plane to be very close
            this.Camera.Near = 5;

            this.Camera.AutoAspectRatio = true;
        }

        public virtual void CreateViewports()
        {
            // create a new viewport and set it's background color
            this.Viewport = this.Window.AddViewport(this.Camera, 0, 0, 1.0f, 1.0f, 100);
            this.Viewport.BackgroundColor = ColorEx.SteelBlue;
        }

        public virtual void CreateInput()
        {
            ParameterList pl = new SharpInputSystem.ParameterList();
            pl.Add(new SharpInputSystem.Parameter("WINDOW", this.Window["WINDOW"]));

            if (this.RenderSystem.Name.Contains("DirectX"))
            {
                //Default mode is foreground exclusive..but, we want to show mouse - so nonexclusive
                pl.Add(new SharpInputSystem.Parameter("w32_mouse", "CLF_BACKGROUND"));
                pl.Add(new SharpInputSystem.Parameter("w32_mouse", "CLF_NONEXCLUSIVE"));
            }

            //This never returns null.. it will raise an exception on errors
            this.InputManager = SharpInputSystem.InputManager.CreateInputSystem(pl);
        }

        public abstract void CreateScene();

        public virtual void Update(float timeSinceLastFrame)
        {
        }

        #region IDisposable Implementation

        #region IsDisposed Property

        /// <summary>
        ///   Determines if this instance has been disposed of already.
        /// </summary>
        public bool IsDisposed { get; set; }

        #endregion IsDisposed Property

        ///<summary>
        ///  Class level dispose method
        ///</summary>
        ///<remarks>
        ///  When implementing this method in an inherited class the following template should be used;
        ///  protected override void dispose( bool disposeManagedResources )
        ///  {
        ///  if ( !IsDisposed )
        ///  {
        ///  if ( disposeManagedResources )
        ///  {
        ///  // Dispose managed resources.
        ///  }
        /// 
        ///  // If there are unmanaged resources to release, 
        ///  // they need to be released here.
        ///  }
        ///
        ///  // If it is available, make the call to the
        ///  // base class's Dispose(Boolean) method
        ///  base.dispose( disposeManagedResources );
        ///  }
        ///</remarks>
        ///<param name="disposeManagedResources"> True if Unmanaged resources should be released. </param>
        protected virtual void dispose(bool disposeManagedResources)
        {
            if (!IsDisposed)
            {
                if (disposeManagedResources)
                {
                    if (this.Engine != null)
                    {
                        // remove event handlers
                        this.Engine.FrameStarted -= Engine_FrameRenderingQueued;
                    }
                    if (this.SceneManager != null)
                    {
                        this.SceneManager.RemoveAllCameras();
                    }
                    this.Camera = null;
                    if (Root.Instance != null)
                    {
                        Root.Instance.RenderSystem.DetachRenderTarget(this.Window);
                    }
                    if (this.Window != null)
                    {
                        WindowEventMonitor.Instance.UnregisterWindow(this.Window);
                        this.Window.Dispose();
                    }
                    if (this.Engine != null)
                    {
                        this.Engine.Dispose();
                    }
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }
            IsDisposed = true;
        }

        /// <summary>
        ///   Call to when class is no longer needed
        /// </summary>
        public void Dispose()
        {
            dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Game()
        {
            dispose(false);
        }

        #endregion IDisposable Implementation

        #region IWindowEventListener Implementation

        /// <summary>
        ///   Window has moved position
        /// </summary>
        /// <param name="rw"> The RenderWindow which created this event </param>
        public void WindowMoved(RenderWindow rw)
        {
        }

        /// <summary>
        ///   Window has resized
        /// </summary>
        /// <param name="rw"> The RenderWindow which created this event </param>
        public void WindowResized(RenderWindow rw)
        {
        }

        /// <summary>
        ///   Window has closed
        /// </summary>
        /// <param name="rw"> The RenderWindow which created this event </param>
        public void WindowClosed(RenderWindow rw)
        {
            // Only do this for the Main Window
            if (rw == this.Window)
            {
                Root.Instance.QueueEndRendering();
            }
        }

        /// <summary>
        ///   Window lost/regained the focus
        /// </summary>
        /// <param name="rw"> The RenderWindow which created this event </param>
        public void WindowFocusChange(RenderWindow rw)
        {
        }

        #endregion
    }
}