using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FimbulwinterClient.GUI.System
{
    public class ArrowSelector : Control
    {
        private ImageButton ibScrollLeft;
        private ImageButton ibScrollRight;

        private int _maximum;
        public int Maximum
        {
            get { return _maximum; }
            set { _maximum = value; }
        }

        private int _minimum;
        public int Minimum
        {
            get { return _minimum; }
            set { _minimum = value; }
        }

        private int _value;
        public int Value
        {
            get { return _value; }
            set { _value = value; }
        }

        private Texture2D scrollmid;
        private Texture2D scroll1bar_left;
        private Texture2D scroll1bar_mid;
        private Texture2D scroll1bar_right;

        public event Action ValueChanged;

        public ArrowSelector()
        {
            this.scrollmid = ROClient.Singleton.ContentManager.LoadContent<Texture2D>("data\\texture\\유저인터페이스\\scroll1mid.bmp");
            this.scroll1bar_left = ROClient.Singleton.ContentManager.LoadContent<Texture2D>("data\\texture\\유저인터페이스\\scroll1bar_left.bmp");
            this.scroll1bar_mid = ROClient.Singleton.ContentManager.LoadContent<Texture2D>("data\\texture\\유저인터페이스\\scroll1bar_mid.bmp");
            this.scroll1bar_right = ROClient.Singleton.ContentManager.LoadContent<Texture2D>("data\\texture\\유저인터페이스\\scroll1bar_right.bmp");

            Texture2D scrollleft = ROClient.Singleton.ContentManager.LoadContent<Texture2D>("data\\texture\\유저인터페이스\\scroll1left.bmp");
            ibScrollLeft = new ImageButton(scrollleft, scrollleft, scrollleft);
            ibScrollLeft.Size = new Vector2(13, 13);
            ibScrollLeft.Clicked += new Action<Nuclex.Input.MouseButtons, float, float>(ibScrollLeft_Clicked);
            this.Controls.Add(ibScrollLeft);

            Texture2D scrollright = ROClient.Singleton.ContentManager.LoadContent<Texture2D>("data\\texture\\유저인터페이스\\scroll1right.bmp");
            ibScrollRight = new ImageButton(scrollright, scrollright, scrollright);
            ibScrollRight.Size = new Vector2(13, 13);
            ibScrollRight.Clicked += new Action<Nuclex.Input.MouseButtons, float, float>(ibScrollRight_Clicked);
            this.Controls.Add(ibScrollRight);
        }

        void ibScrollLeft_Clicked(Nuclex.Input.MouseButtons arg1, float arg2, float arg3)
        {
            if (_value > _minimum)
            {
                _value--;
                if (ValueChanged != null)
                    ValueChanged();
            }
        }

        void ibScrollRight_Clicked(Nuclex.Input.MouseButtons arg1, float arg2, float arg3)
        {
            if (_value < _maximum)
            {
                _value++;
                if (ValueChanged != null)
                    ValueChanged();
            }
        }

        public override void Draw(SpriteBatch sb, GameTime gt)
        {
            int absX = (int)GetAbsX();
            int absY = (int)GetAbsY();

            sb.Draw(this.scrollmid, new Rectangle(absX + 12, absY, (int)this.Size.X - 24, 13), new Rectangle(0, 0, 13, 13), Color.White);
            ibScrollLeft.Position = new Vector2(0, 0);
            ibScrollRight.Position = new Vector2(0 + this.Size.X - 13, 0);

            // draw dot
            int range = _maximum - _minimum;
            int valueInRange = _value - _minimum;

            int barwidth = (int)this.Size.X - 24;

            double cellwidth = (double)barwidth / (range + 1);
            int cellWidth = Convert.ToInt32(cellwidth);
            double cellstartx = valueInRange * cellwidth;
            int cellStartX = Convert.ToInt32(cellstartx);
            if (cellWidth < 8) cellWidth = 8;
            if (cellStartX + cellWidth > barwidth) cellStartX = barwidth - cellWidth;
            sb.Draw(this.scroll1bar_mid, new Rectangle(absX + 12 + cellStartX + 2, absY, cellWidth - 2, 13), new Rectangle(0, 0, 4, 13), Color.White);
            sb.Draw(this.scroll1bar_left, new Rectangle(absX + 12 + cellStartX, absY, 4, 13), new Rectangle(0, 0, 4, 13), Color.White);
            sb.Draw(this.scroll1bar_right, new Rectangle(absX + 12 + cellStartX + cellWidth - 4, absY, 4, 13), new Rectangle(0, 0, 4, 13), Color.White);

            base.Draw(sb, gt);
        }
    }
}
