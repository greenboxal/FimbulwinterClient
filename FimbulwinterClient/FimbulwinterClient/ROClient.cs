using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FimbulwinterClient.IO;
using FimbulwinterClient.Audio;
using FimbulwinterClient.Lua;
using Nuclex.Input;
using Nuclex.Input.Devices;
using FimbulwinterClient.GUI;
using FimbulwinterClient.Content;
using FimbulwinterClient.GUI.System;
using System.IO;
using FimbulwinterClient.Network;
using FimbulwinterClient.Network.Packets;
using FimbulwinterClient.Network.Packets.Character;
using FimbulwinterClient.Network.Packets.Account;
using FimbulwinterClient.Screens;
using FimbulwinterClient.Config;

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

        Configuration cfg;
        public Configuration Config
        {
            get { return cfg; }
            set { cfg = value; }
        }

        public ROContentManager ContentManager
        {
            get { return (ROContentManager)Content; }
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
            Window.Title = "Ragnarok Online";
            Content = (ContentManager)new ROContentManager(Services, this);
            Content.RootDirectory = "data";

            try
            {
                Stream s = ContentManager.LoadContent<Stream>("data\\fb\\config\\config.xml");
                cfg = Configuration.FromStream(s);
                cfg.Client = this;
                s.Close();
            }
            catch
            {
                cfg = new Configuration(this);
            }

            cfg.ReadConfig();

            bgmManager = new BGMManager();
            effectManager = new EffectManager();
            luaManager = new LuaManager();

            inputManager = new Nuclex.Input.InputManager(Services, Window.Handle);

            guiManager = new GuiManager(this);
            guiManager.DrawOrder = 1000;

            Components.Add(inputManager);
            Components.Add(guiManager);

            Services.AddService(typeof(InputManager), inputManager);
            Services.AddService(typeof(GuiManager), guiManager);
            Services.AddService(typeof(EffectManager), effectManager);
            Services.AddService(typeof(BGMManager), bgmManager);
            Services.AddService(typeof(LuaManager), luaManager);

            graphics.PreferredBackBufferWidth = cfg.ScreenWidth;
            graphics.PreferredBackBufferHeight = cfg.ScreenHeight;
            graphics.ApplyChanges();

            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            netState = new NetworkState();
        }

        protected override void Initialize()
        {
            base.Initialize();

            InputManager im = (InputManager)Services.GetService(typeof(InputManager));
            IKeyboard kb = im.GetKeyboard();
            kb.KeyReleased += new KeyDelegate(kb_KeyReleased);

            GUI.Utils.Init(GraphicsDevice);
            //ChangeScreen(new ServiceSelectScreen());
            ChangeScreen(new TestMap("prontera.gat"));
        }

        void kb_KeyReleased(Keys key)
        {
            if (key == Keys.PrintScreen)
                MakeScreenshot();
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
    }
}
