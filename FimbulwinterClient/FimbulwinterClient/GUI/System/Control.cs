using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nuclex.Input;
using Microsoft.Xna.Framework.Audio;

namespace FimbulwinterClient.GUI.System
{
    public class Control
    {
        private static Texture2D formSkin;
        public static Texture2D FormSkin
        {
            get { return Control.formSkin; }
        }

        private static SpriteFont gulim8;
        public static SpriteFont Gulim8
        {
            get { return gulim8; }
        }

        private static SpriteFont gulim8B;
        public static SpriteFont Gulim8B
        {
            get { return gulim8B; }
        }

        private static SoundEffect tingSound;
        public static SoundEffect TingSound
        {
            get { return Control.tingSound; }
            set { Control.tingSound = value; }
        }

        private Control _parent;
        public Control Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        private ControlCollection _controls;
        public ControlCollection Controls
        {
            get { return _controls; }
        }

        private bool _enabled;
        public bool Enabled
        {
          get { return _enabled; }
          set { _enabled = value; }
        }

        private bool _visible;
        public bool Visible
        {
          get { return _visible; }
          set { _visible = value; }
        }

        private Vector2 _position;
        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        private Vector2 _size;
        public Vector2 Size
        {
            get { return _size; }
            set { _size = value; }
        }

        private int _handle;
        public int Handle
        {
            get { return _handle; }
        }

        private string _text;
        public virtual string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        private Color _foreColor;
        public Color ForeColor
        {
            get { return _foreColor; }
            set { _foreColor = value; }
        }

        private Color _backColor;
        public Color BackColor
        {
            get { return _backColor; }
            set { _backColor = value; }
        }

        private int _zorder;
        public int ZOrder
        {
            get { return _zorder; }
            set { _zorder = value; if (_parent != null) _parent.Controls.SortByZOrder(); }
        }

        private bool _dragging;
        public bool Dragging
        {
            get { return _dragging; }
            set { _dragging = value; }
        }

        private bool _tabStop;
        public bool TabStop
        {
            get { return _tabStop; }
            set { _tabStop = value; }
        }

        private SpriteFont _font;
        public SpriteFont Font
        {
            get { return _font; }
            set { _font = value; }
        }

        internal float GetAbsX()
        {
            if (_parent != null)
                return _parent.GetAbsX() + _position.X;
            else
                return _position.X;
        }

        internal float GetAbsY()
        {
            if (_parent != null)
                return _parent.GetAbsY() + _position.Y;
            else
                return _position.Y;
        }

        public Control()
        {
            if (formSkin == null)
                formSkin = GuiManager.Singleton.Client.ContentManager.LoadContent<Texture2D>("data\\fb\\texture\\wndskin.png");

            if (gulim8 == null)
                gulim8 = GuiManager.Singleton.Client.Content.Load<SpriteFont>("fb\\Gulim8");

            if (gulim8B == null)
                gulim8B = GuiManager.Singleton.Client.Content.Load<SpriteFont>("fb\\Gulim8b");

            if (tingSound == null)
                tingSound = GuiManager.Singleton.Client.ContentManager.LoadContent<SoundEffect>("data\\wav\\버튼소리.wav");

            _controls = new ControlCollection(this);
            _handle = GuiManager.Singleton.GetNewHandle();

            _foreColor = Color.Black;
            _backColor = Color.White;

            _text = "";

            _visible = true;
            _enabled = true;
            _tabStop = false;

            _zorder = 0;
            _font = Gulim8;
        }

        public virtual void Update(GameTime gt)
        {
            foreach (Control c in _controls)
                if (c.Visible && c.Enabled)
                    c.Update(gt);
        }

        public virtual void Draw(SpriteBatch sb, GameTime gt)
        {
            foreach (Control c in _controls)
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

        public virtual bool HandleTab()
        {
            return false;
        }

        public override bool Equals(object obj)
        {
            if (typeof(Control) == obj.GetType() || obj.GetType().IsSubclassOf(typeof(Control)))
                return ((Control)obj).Handle == this.Handle;

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _handle;
        }

        public void Focus()
        {
            GuiManager.Singleton.SetActiveControl(this);
        }
    }
}
