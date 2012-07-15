using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulwinterClient.GUI;
using FimbulwinterClient.GUI.Ingame;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FimbulwinterClient.Screens
{
    class IngameScreen : IGameScreen
    {
        public IngameScreen()
        {
            ROClient.Singleton.GuiManager.Controls.Add(new QuickSlotWindow());
            ROClient.Singleton.GuiManager.Controls.Add(new CollectionInfoWindow());
        }

        public virtual void Draw(SpriteBatch sb, GameTime gameTime)
        {
            sb.Begin();
            //sb.Draw(_background, new Rectangle(0, 0, ROClient.Singleton.Config.ScreenWidth, ROClient.Singleton.Config.ScreenHeight), Color.White);
            sb.End();
        }

        public virtual void Update(SpriteBatch sb, GameTime gameTime)
        {

        }

        public virtual void Dispose()
        {
        }
    }
}
