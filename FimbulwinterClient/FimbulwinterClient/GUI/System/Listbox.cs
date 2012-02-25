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
        private List<object> m_items;
        public List<object> Items
        {
            get { return m_items; }
            set { m_items = value; }
        }

        private int m_selectedIndex;
        public int SelectedIndex
        {
            get { return m_selectedIndex; }
            set { m_selectedIndex = value; }
        }

        private int drawStart = 0;
        private int drawCount = 0;
        private int lineHeight = 0;

        public Listbox()
        {
            m_items = new List<object>();
            m_selectedIndex = -1;

            lineHeight = (int)Arial10.MeasureString("A").Y;
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
            for (int i = drawStart; i < drawCount && i < m_items.Count; i++)
            {
                string str = m_items[i].ToString();

                if (i == m_selectedIndex)
                {
                    Utils.DrawBackground(sb, Color.CornflowerBlue, absX, atY, (int)Size.X, (int)lineHeight);
                }

                sb.DrawString(Arial10, str, new Vector2(absX, atY), ForeColor);

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
                    m_selectedIndex = i;
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
