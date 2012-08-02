using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulwinterClient.Gui.System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FimbulwinterClient.Core;

namespace FimbulwinterClient.Gui.Ingame
{
    class QuickSlotWindow : Window
    {
        public QuickSlotWindow()
        {
            this.FullImage = SharedInformation.ContentManager.Load<Texture2D>("data\\texture\\유저인터페이스\\basic_interface\\quickslot.bmp");
            this.Size = new Vector2(this.FullImage.Width, this.FullImage.Height);
        }
    }
}
