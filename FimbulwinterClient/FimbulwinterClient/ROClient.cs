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
using Nuclex.UserInterface;
using FimbulwinterClient.GUI;

namespace FimbulwinterClient
{
    public enum ROClientState
    {
        None,
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

        Screen currentScreen;

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

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 768;
            graphics.ApplyChanges();

            Window.Title = "Ragnarök - Fimbulwinter Client";

            Content = (ContentManager)new ROContentManager(Services, this);
            Content.RootDirectory = "data";

            cfg = new ROConfig(this);
            cfg.ReadConfig();

            bgmManager = new BGMManager(this, cfg);
            effectManager = new EffectManager(this, cfg);

            inputManager = new Nuclex.Input.InputManager(Services, Window.Handle);
            guiManager = new Nuclex.UserInterface.GuiManager(Services, ContentManager);

            Components.Add(inputManager);
            Components.Add(guiManager);

            guiManager.DrawOrder = 1000;

            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            ChangeState(ROClientState.Login);
            ChangeLoginState(ROLoginState.ServiceSelect);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            loginScreenBG = ContentManager.LoadContent<Texture2D>("data/fb/lsbg.jpg");
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

            if (state == ROClientState.Login)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(loginScreenBG, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);

                switch (loginState)
                {
                    case ROLoginState.ServiceSelect:
                        break;
                }
                
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
                Viewport viewport = GraphicsDevice.Viewport;

                currentScreen = new Screen(viewport.Width, viewport.Height);
                currentScreen.Desktop.Children.Add(new ServiceSelect(this));

                guiManager.Screen = currentScreen;
            }

            loginState = s;
        }

        public void ChangeState(ROClientState s)
        {
            if (s == ROClientState.Login)
            {
                bgmManager.PlayBGM("01");
            }

            state = s;
        }
    }
}
