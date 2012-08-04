using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FimbulwinterClient.Audio;
using FimbulwinterClient.Core.Config;
using FimbulwinterClient.Core.Assets;
using FimbulwinterClient.Core.Content;
using FimbulwinterClient.Gui;
using FimbulwinterClient.Gui.System;
using FimbulwinterClient.Lua;
using FimbulwinterClient.Network;
using FimbulwinterClient.Network.Packets;
using FimbulwinterClient.Network.Packets.Account;
using FimbulwinterClient.Network.Packets.Character;
using FimbulwinterClient.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Nuclex.Input;
using Nuclex.Input.Devices;
using FimbulwinterClient.Core;

namespace FimbulwinterClient
{
    public class ROClient : Microsoft.Xna.Framework.Game
    {
        public static ROClient Singleton { get; set; }

        GraphicsDeviceManager graphics;
        public GraphicsDeviceManager Graphics
        {
            get { return graphics; }
            set { graphics = value; }
        }

        SpriteBatch spriteBatch;
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
            set { spriteBatch = value; }
        }

        BGMManager bgmManager;
        public BGMManager BgmManager
        {
            get { return bgmManager; }
            set { bgmManager = value; }
        }

        EffectManager effectManager;
        public EffectManager EffectManager
        {
            get { return effectManager; }
            set { effectManager = value; }
        }

        LuaManager luaManager;
        public LuaManager LuaManager
        {
            get { return luaManager; }
            set { luaManager = value; }
        }

        InputManager inputManager;
        public InputManager InputManager
        {
            get { return inputManager; }
            set { inputManager = value; }
        }

        GuiManager guiManager;
        public GuiManager GuiManager
        {
            get { return guiManager; }
            set { guiManager = value; }
        }

        IGameScreen screen;
        public IGameScreen Screen
        {
            get { return screen; }
            set { screen = value; }
        }

        NetworkState netState;
        public NetworkState NetworkState
        {
            get { return netState; }
            set { netState = value; }
        }

        Connection currentConnection;
        public Connection CurrentConnection
        {
            get { return currentConnection; }
            set { currentConnection = value; }
        }

        public ROClient()
        {
            Singleton = this;
            graphics = new GraphicsDeviceManager(this);
            graphics.SynchronizeWithVerticalRetrace = false; // REMOVE ME LATER
            Window.Title = "Ragnarok Online";

            SharedInformation.Initialize(Services, GraphicsDevice);
            Content = SharedInformation.ContentManager;

            try
            {
                Stream s = Content.Load<Stream>(@"data\fb\config\config.xml");
                SharedInformation.Config = Configuration.FromStream(s);
                s.Close();
            }
            catch
            {
                SharedInformation.Config = new Configuration();
            }

            SharedInformation.Config.ReadConfig();

            bgmManager = new BGMManager();
            effectManager = new EffectManager();
            luaManager = new LuaManager();

            inputManager = new Nuclex.Input.InputManager(Services, Window.Handle);

            guiManager = new GuiManager(this);
            guiManager.DrawOrder = 1000;

            Components.Add(inputManager);
            Components.Add(guiManager);
            Components.Add(new FPSCounter(this)); // REMOVE ME LATER

            IsFixedTimeStep = false; // REMOVE ME LATER

            Services.AddService(typeof(InputManager), inputManager);
            Services.AddService(typeof(GuiManager), guiManager);
            Services.AddService(typeof(EffectManager), effectManager);
            Services.AddService(typeof(BGMManager), bgmManager);
            Services.AddService(typeof(LuaManager), luaManager);

            graphics.PreferredBackBufferWidth = SharedInformation.Config.ScreenWidth;
            graphics.PreferredBackBufferHeight = SharedInformation.Config.ScreenHeight;
            graphics.ApplyChanges();

            netState = new NetworkState();
        }

        protected override void Initialize()
        {
            SharedInformation.GraphicsDevice = GraphicsDevice;
            Gui.Utils.Init(GraphicsDevice);

            inputManager.GetKeyboard().KeyPressed += kb_KeyReleased;

            //ChangeScreen(new ServiceSelectScreen());
            //ChangeScreen(new LoadingScreen("prontera.gat"));
            StartMapChange("prontera");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (screen != null)
                screen.Update(spriteBatch, gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (screen != null)
                screen.Draw(spriteBatch, gameTime);

            base.Draw(gameTime);
        }

        public void ChangeScreen(IGameScreen screen)
        {
            if (this.screen != null)
                this.screen.Dispose();

            this.screen = screen;
        }

        private static int _counter;
        void MakeScreenshot()
        {
            int w = ROClient.Singleton.GraphicsDevice.PresentationParameters.BackBufferWidth;
            int h = ROClient.Singleton.GraphicsDevice.PresentationParameters.BackBufferHeight;

            Draw(new GameTime());

            int[] backBuffer = new int[w * h];
            ROClient.Singleton.GraphicsDevice.GetBackBufferData(backBuffer);

            //copy into a texture 
            Texture2D texture = new Texture2D(ROClient.Singleton.GraphicsDevice, w, h, false, GraphicsDevice.PresentationParameters.BackBufferFormat);
            texture.SetData(backBuffer);

            //save to disk 
            if (!System.IO.Directory.Exists("ScreenShot")) System.IO.Directory.CreateDirectory("ScreenShot");
            Stream stream = File.OpenWrite(System.IO.Path.Combine("ScreenShot", "screen" + _counter + ".png"));

            texture.SaveAsPng(stream, w, h);
            stream.Dispose();

            texture.Dispose();
            _counter++;
        }

        void kb_KeyReleased(Keys key)
        {
            if (key == Keys.PrintScreen)
                MakeScreenshot();
        }

        public void StartMapChange(string p)
        {
            LoadingScreen ls = new LoadingScreen(p);

            ls.Loaded += ls_Loaded;

            ChangeScreen(ls);
        }

        private void ls_Loaded(Map obj)
        {
            ChangeScreen(new IngameScreen(obj));
        }
    }
}
