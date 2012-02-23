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
using Nuclex.Input;
using FimbulwinterClient.GUI;
using FimbulwinterClient.Content;
using FimbulwinterClient.GUI.System;

namespace FimbulwinterClient
{
    public enum ROClientState
    {
        None,
        Test,
        Login,
        Loading,
        InGame,
    }

    public enum ROLoginState
    {
        ServiceSelect,
        Login,
        CharServerSelect,
        CharSelect,
        CreateChar,
    }

    public class ROClient : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        ROConfig cfg;
        BGMManager bgmManager;
        EffectManager effectManager;

        ROClientState state;
        ROLoginState loginState;

        Texture2D loginScreenBG;

        InputManager inputManager;
        GuiManager guiManager;

        public GuiManager GuiManager
        {
            get { return guiManager; }
            set { guiManager = value; }
        }

        public InputManager InputManager
        {
            get { return inputManager; }
            set { inputManager = value; }
        }

        public ROConfig Config
        {
            get { return cfg; }
        }

        public ROContentManager ContentManager
        {
            get { return (ROContentManager)Content; }
        }

        public BGMManager BgmManager
        {
            get { return bgmManager; }
        }

        public EffectManager EffectManager
        {
            get { return effectManager; }
        }

        public ROClient()
        {
            graphics = new GraphicsDeviceManager(this);

            Window.Title = "Ragnarök - Fimbulwinter Client";

            Content = (ContentManager)new ROContentManager(Services, this);
            Content.RootDirectory = "data";

            cfg = new ROConfig(this);
            cfg.ReadConfig();

            bgmManager = new BGMManager(this, cfg);
            effectManager = new EffectManager(this, cfg);

            inputManager = new Nuclex.Input.InputManager(Services, Window.Handle);

            guiManager = new GuiManager(this);
            guiManager.DrawOrder = 1000;

            Components.Add(inputManager);
            Components.Add(guiManager);

            Services.AddService(typeof(InputManager), inputManager);

            graphics.PreferredBackBufferWidth = cfg.ScreenWidth;
            graphics.PreferredBackBufferHeight = cfg.ScreenHeight;
            graphics.ApplyChanges();

            GraphicsDevice.BlendState = BlendState.AlphaBlend;
        }

        protected override void Initialize()
        {
            base.Initialize();

            ChangeState(ROClientState.Test);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            loginScreenBG = ContentManager.LoadContent<Texture2D>("data/texture/À¯ÀúÀÎÅÍÆäÀÌ½º/bgi_temp.bmp");
        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (state == ROClientState.Login || state == ROClientState.Test)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(loginScreenBG, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);                
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        public void ChangeLoginState(ROLoginState s)
        {
            if (state != ROClientState.Login)
                ChangeState(ROClientState.Login);

            if (s == ROLoginState.ServiceSelect)
            {
            }

            loginState = s;
        }

        public void ChangeState(ROClientState s)
        {
            if (s == ROClientState.Login || s == ROClientState.Test)
            {
                bgmManager.PlayBGM("01");

                ServiceSelectWindow w = new ServiceSelectWindow(cfg);
                guiManager.Controls.Add(w);
            }

            state = s;
        }
    }
}
