using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulwinterClient.GUI;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FimbulwinterClient.Network.Packets.Char;

namespace FimbulwinterClient.Screens
{
    public class CharSelectScreen : BaseLoginScreen
    {
        private CharSelectWindow window;

        public CharSelectScreen()
        {
            window = new CharSelectWindow();
            window.OnCreateChar += new Action<int>(window_OnCreateChar);
            window.OnSelectChar += new Action<int>(window_OnSelectChar);

            ROClient.Singleton.GuiManager.Controls.Add(window);
        }

        void window_OnSelectChar(int obj)
        {
            
        }

        void window_OnCreateChar(int obj)
        {
            ROClient.Singleton.ChangeScreen(new CreateCharScreen());
        }

        public override void Update(SpriteBatch sb, GameTime gameTime)
        {
            base.Update(sb, gameTime);

            if (gameTime.TotalGameTime.TotalSeconds % 12 < 1.0F)
            {
                new CSPing((int)gameTime.TotalGameTime.TotalMilliseconds).Write(ROClient.Singleton.CurrentConnection.BinaryWriter);
            }
        }
    }
}
