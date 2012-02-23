using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nuclex.Input;

namespace FimbulwinterClient.GUI.System
{
    public class Control
    {
        private static Texture2D formSkin;
        public static Texture2D FormSkin
        {
            get { return Control.formSkin; }
        }

        private static SpriteFont arial10;
        public static SpriteFont Arial10
        {
            get { return Control.arial10; }
        }

        private Control m_parent;
        public Control Parent
        {
            get { return m_parent; }
            set { m_parent = value; }
        }

        private ControlCollection m_controls;
        public ControlCollection Controls
        {
            get { return m_controls; }
        }

        private bool m_enabled;
        public bool Enabled
        {
          get { return m_enabled; }
          set { m_enabled = value; }
        }

        private bool m_visible;
        public bool Visible
        {
          get { return m_visible; }
          set { m_visible = value; }
        }

        private Vector2 m_position;
        public Vector2 Position
        {
            get { return m_position; }
            set { m_position = value; }
        }

        private Vector2 m_size;
        public Vector2 Size
        {
            get { return m_size; }
            set { m_size = value; }
        }

        private int m_handle;
        public int Handle
        {
            get { return m_handle; }
        }

        private string m_text;
        public string Text
        {
            get { return m_text; }
            set { m_text = value; }
        }

        private Color m_foreColor;
        public Color ForeColor
        {
            get { return m_foreColor; }
            set { m_foreColor = value; }
        }

        private Color m_backColor;
        public Color BackColor
        {
            get { return m_backColor; }
            set { m_backColor = value; }
        }

        internal float GetAbsX()
        {
            if (m_parent != null)
                return m_parent.GetAbsX() + m_position.X;
            else
                return m_position.X;
        }

        internal float GetAbsY()
        {
            if (m_parent != null)
                return m_parent.GetAbsY() + m_position.Y;
            else
                return m_position.Y;
        }

        public Control()
        {
            if (formSkin == null)
                formSkin = GuiManager.Singleton.Client.ContentManager.LoadContent<Texture2D>("data/fb/texture/wndskin.png");

            if (arial10 == null)
                arial10 = GuiManager.Singleton.Client.Content.Load<SpriteFont>("fb/arial10");

            m_foreColor = Color.Black;
            m_backColor = Color.White;

            m_controls = new ControlCollection(this);

            m_visible = true;
            m_enabled = true;

            m_handle = GuiManager.Singleton.GetNewHandle();
        }

        public virtual void Update(GameTime gt)
        {
            foreach (Control c in m_controls)
                if (c.Visible && c.Enabled)
                    c.Update(gt);
        }

        public virtual void Draw(SpriteBatch sb, GameTime gt)
        {
            foreach (Control c in m_controls)
                if (c.Visible)
                    c.Draw(sb, gt);
        }

        public event Action<MouseButtons, float, float> Clicked;
        public event Action<MouseButtons, float, float> DoubleClicked;
        public event Action<MouseButtons, float, float> MouseDown;
        public event Action<MouseButtons, float, float> MouseUp;
        public event Action<float, float> MouseMove;
        public event Action MouseHover;
        public event Action MouseLeave;
        public event Action<float> MouseWheel;
        public event Action<Keys> KeyDown;
        public event Action<Keys> KeyUp;
        public event Action<char> KeyPress;

        public virtual void OnClick(MouseButtons buttons, float x, float y)
        {
            if (Clicked != null)
                Clicked(buttons, x, y);
        }

        public virtual void OnDoubleClick(MouseButtons buttons, float x, float y)
        {
            if (DoubleClicked != null)
                DoubleClicked(buttons, x, y);
        }

        public virtual void OnMouseDown(MouseButtons buttons, float x, float y)
        {
            if (MouseDown != null)
                MouseDown(buttons, x, y);
        }

        public virtual void OnMouseUp(MouseButtons buttons, float x, float y)
        {
            if (MouseUp != null)
                MouseUp(buttons, x, y);
        }

        public virtual void OnMouseMove(float x, float y)
        {
            if (MouseMove != null)
                MouseMove(x, y);
        }

        public virtual void OnMouseHover()
        {
            if (MouseHover != null)
                MouseHover();
        }

        public virtual void OnMouseLeave()
        {
            if (MouseLeave != null)
                MouseLeave();
        }

        public virtual void OnMouseWheel(float ticks)
        {
            if (MouseWheel != null)
                MouseWheel(ticks);
        }

        public virtual void OnKeyDown(Keys key)
        {
            if (KeyDown != null)
                KeyDown(key);
        }

        public virtual void OnKeyUp(Keys key)
        {
            if (KeyUp != null)
                KeyUp(key);
        }

        public virtual void OnKeyPress(char c)
        {
            if (KeyPress != null)
                KeyPress(c);
        }

        public override bool Equals(object obj)
        {
            if (typeof(Control) == obj.GetType() || obj.GetType().IsSubclassOf(typeof(Control)))
                return ((Control)obj).Handle == this.Handle;

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return m_handle;
        }

        public void Focus()
        {
            GuiManager.Singleton.SetActiveControl(this);
        }
    }
}
