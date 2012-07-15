using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulwinterClient.GUI.System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FimbulwinterClient.GUI.Ingame
{
    class QuickSlotWindow : Window
    {
        public QuickSlotWindow()
        {
            this.FullImage = ROClient.Singleton.ContentManager.LoadContent<Texture2D>("data\\texture\\유저인터페이스\\basic_interface\\quickslot.bmp");
            this.Size = new Vector2(this.FullImage.Width, this.FullImage.Height);
        }
    }
}
