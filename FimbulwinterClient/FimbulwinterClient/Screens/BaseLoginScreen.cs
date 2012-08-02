using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FimbulwinterClient.Gui;
using FimbulwinterClient.Gui.System;
using FimbulwinterClient.Core;

namespace FimbulwinterClient.Screens
{
    public class BaseLoginScreen : IGameScreen
    {
        private MessageBox _wait;
        private static Texture2D _background;

        public BaseLoginScreen()
        {
            if (_background == null)
            {
                Texture2D[][] background = new Texture2D[3][];
                for (int y = 0; y < 3; y++)
                {
                    background[y] = new Texture2D[4];
                    for (int x = 0; x < 4; x++)
                    {
                        background[y][x] = SharedInformation.ContentManager.Load<Texture2D>(string.Format("data\\texture\\유저인터페이스\\t_배경{0}-{1}.bmp", y + 1, x + 1));
                    }
                }
                
                var gd = ROClient.Singleton.GraphicsDevice;
                SpriteBatch sprite = new SpriteBatch(gd);
                RenderTarget2D render2D = new RenderTarget2D(gd, 1024, 768);
                gd.SetRenderTarget(render2D);
                sprite.Begin();
                for (int y = 0; y < 3; ++y)
                    for (int x = 0; x < 4; ++x)
                        sprite.Draw(background[y][x], new Vector2(x * 256, y * 256), Color.White);
                sprite.End();
                _background = render2D;
                gd.SetRenderTarget(null);
            }

            ROClient.Singleton.BgmManager.PlayBGM("01");
        }

        public void ShowWait()
        {
            _wait = MessageBox.ShowMessage("Please wait...");
            _wait.Position = new Vector2(SharedInformation.Config.ScreenWidth / 2 - 140, SharedInformation.Config.ScreenHeight - 140 - 120);
        }

        public void CloseWait()
        {
            if (_wait != null)
                _wait.Close();
        }

        public virtual void Draw(SpriteBatch sb, GameTime gameTime)
        {
            sb.Begin();
            sb.Draw(_background, new Rectangle(0, 0, SharedInformation.Config.ScreenWidth, SharedInformation.Config.ScreenHeight), Color.White);
            sb.End();
        }

        public virtual void Update(SpriteBatch sb, GameTime gameTime)
        {

        }

        public virtual void Dispose()
        {
            if (_wait != null)
                _wait.Close();
        }
    }
}
