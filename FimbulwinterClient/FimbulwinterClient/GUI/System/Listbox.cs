using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FimbulwinterClient.GUI.System
{
    public class Listbox : Control
    {
        private List<object> _items;
        public List<object> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set { _selectedIndex = value; }
        }

        private int drawStart = 0;
        private int drawCount = 0;
        private int lineHeight = 0;

        public Listbox()
        {
            _items = new List<object>();
            _selectedIndex = -1;

            lineHeight = (int)Gulim8.MeasureString("A").Y;
        }

        public override void Update(GameTime gt)
        {
            drawCount = (int)Size.Y / lineHeight;

            base.Update(gt);
        }

        public override void Draw(SpriteBatch sb, GameTime gt)
        {
            int absX = (int)GetAbsX();
            int absY = (int)GetAbsY();

            int atY = absY;
            for (int i = drawStart; i < drawCount && i < _items.Count; i++)
            {
                string str = _items[i].ToString();

                if (i == _selectedIndex)
                {
                    Utils.DrawBackground(sb, Color.CornflowerBlue, absX, atY, (int)Size.X, (int)lineHeight);
                }

                sb.DrawString(Gulim8, str, new Vector2(absX, atY + 1), ForeColor);

                atY += (int)lineHeight;
            }

            base.Draw(sb, gt);
        }

        public override void OnClick(Nuclex.Input.MouseButtons buttons, float x, float y)
        {
            int atY = 0;
            for (int i = drawStart; i < drawCount; i++)
            {
                if (y >= atY && y < atY + lineHeight)
                {
                    _selectedIndex = i;
                    break;
                }

                atY += lineHeight;
            }

            base.OnClick(buttons, x, y);
        }

        public override void OnKeyDown(Microsoft.Xna.Framework.Input.Keys key)
        {
            base.OnKeyDown(key);

            if (key == Microsoft.Xna.Framework.Input.Keys.Enter)
                if (OnActivate != null)
                    OnActivate();
        }

        public event Action OnActivate;
    }
}
