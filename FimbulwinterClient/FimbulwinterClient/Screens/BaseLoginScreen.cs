using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FimbulwinterClient.GUI;
using FimbulwinterClient.GUI.System;

namespace FimbulwinterClient.Screens
{
    public class BaseLoginScreen : IGameScreen
    {
        private Texture2D background;
        private MessageBox wait;

        public BaseLoginScreen()
        {
            background = ROClient.Singleton.ContentManager.LoadContent<Texture2D>("data\\fb\\texture\\login_screen.png");
            ROClient.Singleton.BgmManager.PlayBGM("01");
        }

        public void ShowWait()
        {
            wait = MessageBox.ShowMessage("Please wait...");
            wait.Position = new Vector2(ROClient.Singleton.Config.ScreenWidth / 2 - 140, ROClient.Singleton.Config.ScreenHeight - 140 - 120);
        }

        public void CloseWait()
        {
            if (wait != null)
                wait.Close();
        }

        public virtual void Draw(SpriteBatch sb, GameTime gameTime)
        {
            sb.Begin();
            sb.Draw(background, new Rectangle(0, 0, ROClient.Singleton.Config.ScreenWidth, ROClient.Singleton.Config.ScreenHeight), Color.White);
            sb.End();
        }

        public virtual void Update(SpriteBatch sb, GameTime gameTime)
        {
            
        }

        public virtual void Dispose()
        {
            if (wait != null)
                wait.Close();
        }
    }
}
