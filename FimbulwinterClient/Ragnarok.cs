using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IrrlichtLime;
using IrrlichtLime.Video;
using FimbulwinterClient.Core;
using IrrlichtLime.Core;
using System.IO;
using FimbulwinterClient.Core.Config;
using FimbulwinterClient.GameModes;
using IrrlichtLime.Scene;
using FimbulwinterClient.Core.Assets;

namespace FimbulwinterClient
{
    public class Ragnarok : IDisposable
    {
        public static Ragnarok Instance { get; private set; }

        private IrrlichtDevice _device;
        public IrrlichtDevice Device
        {
            get { return _device; }
        }

        private SceneNode _gameMode;
        public SceneNode GameMode
        {
            get { return _gameMode; }
        }

        public Ragnarok()
        {
            Instance = this;
        }

        public void Run()
        {
            int frames = 0;

            Initialize();

            while (_device.Run())
			{
                SharedInformation.Graphics.BeginScene(true, true, new Color(100, 101, 140));

                SharedInformation.Scene.DrawAll();
                SharedInformation.GUI.DrawAll();

                SharedInformation.Graphics.EndScene();

                if (++frames == 100)
                {
                    _device.SetWindowCaption(string.Format("Ragnarök Online [{0}] FPS: {1}", SharedInformation.Graphics.Name, SharedInformation.Graphics.FPS));
                    frames = 0;
                }
			}
        }

        protected void Initialize()
        {
            try
            {
                Stream s = SharedInformation.ContentManager.Load<Stream>(@"data\fb\config\config.xml");
                SharedInformation.Config = Configuration.FromStream(s);
                s.Close();
            }
            catch
            {
                SharedInformation.Config = new Configuration();
            }

            SharedInformation.Config.ReadConfig();

            _device = IrrlichtDevice.CreateDevice(DriverType.OpenGL, new Dimension2Di(SharedInformation.Config.ScreenWidth, SharedInformation.Config.ScreenHeight));
            SharedInformation.Initialize(_device);

            _device.SetWindowCaption("Ragnarök Online");
            _device.SetWindowResizable(false);
            _device.CursorControl.Visible = false;

            ChangeMap("prontera");
            ChangeGameMode(new LoadingGameMode("prontera"));
        }

        public void ChangeMap(string mapname)
        {
            ChangeGameMode(new LoadingGameMode(mapname));
        }

        public void ChangeGameMode(SceneNode gameMode)
        {
            if (_gameMode != null)
            {
                _gameMode.Remove();
            }

            _gameMode = gameMode;
        }

        public void Dispose()
        {
            if (_device != null)
            {
                _device.Drop();
                _device = null;
            }
        }
    }
}
