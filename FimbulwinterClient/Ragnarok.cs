using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Axiom.Core;
using Axiom.FileSystem;
using Axiom.Framework;
using Axiom.Framework.Configuration;
using FimbulwinterClient.Core;
using FimbulwinterClient.Core.Content;
using FimbulwinterClient.GameStates;
using SharpInputSystem;
using Axiom.Graphics;
using RenderSystem = Axiom.Graphics.RenderSystem;

namespace FimbulwinterClient
{
    public class Ragnarok : IWindowEventListener, IKeyboardListener, IMouseListener
    {
        public static Ragnarok Instance { get; private set; }

        private Root _engine;

        public Root Engine
        {
            get { return _engine; }
        }

        private IConfigurationManager _configurationManager;

        public IConfigurationManager ConfigurationManager
        {
            get { return _configurationManager; }
        }

        private RenderWindow _window;

        public RenderWindow Window
        {
            get { return _window; }
        }

        private RenderSystem _renderSystem;

        public RenderSystem RenderSystem
        {
            get { return _renderSystem; }
        }

        private InputManager _inputManager;

        public InputManager InputManager
        {
            get { return _inputManager; }
        }

        private Keyboard _keyboard;

        public Keyboard Keyboard
        {
            get { return _keyboard; }
        }

        private Mouse _mouse;

        public Mouse Mouse
        {
            get { return _mouse; }
        }

        private GameState _gameState;

        public GameState GameState
        {
            get { return _gameState; }
        }

        public Ragnarok()
        {
            if (Instance != null)
                throw new Exception("This class can have only one instance, use Ragnarok.Instance.");

            Instance = this;
        }

        public void Run()
        {
            CreateEngine();
            CreateRenderSystem();
            CreateRenderWindow();
            CreateInput();
            LoadContent();
            Initialize();
            _engine.StartRendering();
        }

        private void CreateEngine()
        {
            _configurationManager = ConfigurationManagerFactory.CreateDefault();

            _engine = new Root(_configurationManager.LogFilename);
            _engine.FrameStarted += Update;

            ArchiveManager.Instance.AddArchiveFactory(new GrfArchiveFactory());
            Initialization.DoInit();

            _configurationManager.RestoreConfiguration(_engine);
        }

        private void CreateRenderSystem()
        {
            if (_engine.RenderSystem == null)
            {
                if (Environment.OSVersion.Platform == PlatformID.Win32NT ||
                    Environment.OSVersion.Platform == PlatformID.Win32S ||
                    Environment.OSVersion.Platform == PlatformID.Win32Windows)
                {
                    bool foundDX = false;

                    foreach (RenderSystem rs in _engine.RenderSystems)
                    {
                        if (rs.Name.Contains("DirectX"))
                        {
                            _renderSystem = _engine.RenderSystem = rs;
                            foundDX = true;
                            break;
                        }
                    }

                    if (!foundDX)
                        _renderSystem = _engine.RenderSystem = _engine.RenderSystems.First().Value;
                }
                else
                {
                    _renderSystem = _engine.RenderSystem = _engine.RenderSystems.First().Value;
                }
            }
            else
            {
                _renderSystem = _engine.RenderSystem;
            }
        }

        public void CreateRenderWindow()
        {
            _window = Root.Instance.Initialize(true, "Ragnarök");

            WindowEventMonitor.Instance.RegisterListener(this.Window, this);
        }

        public void CreateInput()
        {
            ParameterList pl = new ParameterList();
            pl.Add(new Parameter("WINDOW", this.Window["WINDOW"]));

            if (_renderSystem.Name.Contains("DirectX"))
            {
                pl.Add(new SharpInputSystem.Parameter("w32_mouse", "CLF_BACKGROUND"));
                pl.Add(new SharpInputSystem.Parameter("w32_mouse", "CLF_NONEXCLUSIVE"));
            }

            _inputManager = InputManager.CreateInputSystem(pl);

            _keyboard = this.InputManager.CreateInputObject<Keyboard>(true, String.Empty);
            _keyboard.EventListener = this;

            _mouse = this.InputManager.CreateInputObject<Mouse>(true, String.Empty);
            _mouse.EventListener = this;
        }

        public void LoadContent()
        {
            ResourceGroupManager.Instance.InitializeAllResourceGroups();
        }

        public void Initialize()
        {
            ChangeWorld("prontera");
        }

        private void Update(object sender, FrameEventArgs e)
        {
        }

        public void ChangeWorld(string name)
        {
            ChangeGameState(new WorldGameState(name));
        }

        public void ChangeGameState(GameState state)
        {
            if (_gameState != null)
            {
                _gameState.Shutdown();
            }

            Window.RemoveAllViewports();

            if (state != null)
            {
                state.Setup(Window, Keyboard, Mouse);
            }

            _gameState = state;
        }

        public void WindowClosed(RenderWindow rw)
        {
            if (_gameState != null)
                _gameState.WindowClosed(rw);
        }

        public void WindowFocusChange(RenderWindow rw)
        {
            if (_gameState != null)
                _gameState.WindowFocusChange(rw);
        }

        public void WindowMoved(RenderWindow rw)
        {
            if (_gameState != null)
                _gameState.WindowMoved(rw);
        }

        public void WindowResized(RenderWindow rw)
        {
            if (_gameState != null)
                _gameState.WindowResized(rw);
        }

        public bool KeyPressed(KeyEventArgs e)
        {
            if (_gameState != null)
                return _gameState.KeyPressed(e);

            return false;
        }

        public bool KeyReleased(KeyEventArgs e)
        {
            if (_gameState != null)
                return _gameState.KeyReleased(e);

            return false;
        }

        public bool MouseMoved(MouseEventArgs arg)
        {
            if (_gameState != null)
                return _gameState.MouseMoved(arg);

            return false;
        }

        public bool MousePressed(MouseEventArgs arg, MouseButtonID id)
        {
            if (_gameState != null)
                return _gameState.MousePressed(arg, id);

            return false;
        }

        public bool MouseReleased(MouseEventArgs arg, MouseButtonID id)
        {
            if (_gameState != null)
                return _gameState.MouseReleased(arg, id);

            return false;
        }
    }
}