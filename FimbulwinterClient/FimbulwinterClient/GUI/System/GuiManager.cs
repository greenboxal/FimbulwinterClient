using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Nuclex.Input;
using Nuclex.Input.Devices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FimbulwinterClient.Content;

namespace FimbulwinterClient.GUI.System
{
    public class GuiManager : DrawableGameComponent
    {
        private static GuiManager m_singleton;
        public static GuiManager Singleton
        {
            get { return GuiManager.m_singleton; }
        }

        private ROClient m_client;
        public ROClient Client
        {
            get { return m_client; }
        }

        private List<Control> m_controls;
        public List<Control> Controls
        {
            get { return m_controls; }
        }

        private SpriteBatch spriteBatch;

        private Control m_activeControl;
        private Control m_hoverControl;
        private Control m_downControl;
        private MouseButtons m_downButtons;

        private SpriteAction m_cursor;
        public SpriteAction Cursor
        {
            get { return m_cursor; }
            set { m_cursor = value; }
        }

        private float mouseX;
        private float mouseY;

        public GuiManager(ROClient roc)
            : base(roc)
        {
            m_client = roc;

            m_singleton = this;
        }

        public override void Initialize()
        {
            base.Initialize();

            spriteBatch = new SpriteBatch(m_client.GraphicsDevice);
            m_controls = new List<Control>();

            m_cursor = m_client.ContentManager.LoadContent<SpriteAction>("data/sprite/cursors.act");
            m_cursor.Loop = true;

            InputManager im = (InputManager)m_client.Services.GetService(typeof(InputManager));

            IMouse mouse = im.GetMouse();
            mouse.MouseButtonPressed += new MouseButtonDelegate(mouse_MouseButtonPressed);
            mouse.MouseButtonReleased += new MouseButtonDelegate(mouse_MouseButtonReleased);
            mouse.MouseMoved += new MouseMoveDelegate(mouse_MouseMoved);
            mouse.MouseWheelRotated += new MouseWheelDelegate(mouse_MouseWheelRotated);

            IKeyboard kb = im.GetKeyboard();
            kb.KeyPressed += new KeyDelegate(kb_KeyPressed);
            kb.KeyReleased += new KeyDelegate(kb_KeyReleased);
            kb.CharacterEntered += new CharacterDelegate(kb_CharacterEntered);
        }

        void kb_CharacterEntered(char character)
        {
            if (m_activeControl != null)
                m_activeControl.OnKeyPress(character);
        }

        void kb_KeyReleased(Keys key)
        {
            if (m_activeControl != null)
                m_activeControl.OnKeyUp(key);
        }

        void kb_KeyPressed(Keys key)
        {
            if (m_activeControl != null)
                m_activeControl.OnKeyDown(key);
        }

        void mouse_MouseWheelRotated(float ticks)
        {
            if (m_activeControl != null)
                m_activeControl.OnMouseWheel(ticks);
        }

        void mouse_MouseMoved(float x, float y)
        {
            mouseX = x;
            mouseY = y;

            if ((m_downButtons & MouseButtons.Left) != 0 && m_downControl != null)
            {
                m_hoverControl.OnMouseMove(x, y);
            }
            else
            {
                Control c = GetControlByPosition(m_controls, null, 0, 0, (int)mouseX, (int)mouseY);

                if (m_hoverControl != c)
                {
                    if (m_hoverControl != null)
                        m_hoverControl.OnMouseLeave();

                    m_hoverControl = c;

                    if (m_hoverControl != null)
                        m_hoverControl.OnMouseHover();
                }

                if (m_hoverControl != null)
                    m_hoverControl.OnMouseMove(x, y);
            }
        }

        void mouse_MouseButtonReleased(MouseButtons buttons)
        {
            Control c = GetControlByPosition(m_controls, null, 0, 0, (int)mouseX, (int)mouseY);

            if (m_downControl != null)
            {
                m_downControl.OnMouseUp(buttons, mouseX, mouseY);

                if (m_downControl == c && (m_downButtons & buttons) != 0)
                {
                    m_downControl.OnClick(m_downButtons & buttons, mouseX, mouseY);
                }
            }

            m_downButtons &= ~buttons;
        }

        void mouse_MouseButtonPressed(MouseButtons buttons)
        {
            m_activeControl = GetControlByPosition(m_controls, null, 0, 0, (int)mouseX, (int)mouseY);

            if (m_activeControl != null)
                m_activeControl.OnMouseDown(buttons, mouseX, mouseY);

            m_downControl = m_activeControl;
            m_downButtons |= buttons;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            for (int i = 0; i < m_controls.Count; i++)
            {
                if (m_controls[i].Visible && m_controls[i].Enabled)
                    m_controls[i].Update(gameTime);
            }

            m_cursor.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            spriteBatch.Begin();
            for (int i = 0; i < m_controls.Count; i++)
            {
                if (m_controls[i].Visible)
                    m_controls[i].Draw(spriteBatch, gameTime);
            }

            m_cursor.Draw(spriteBatch, new Point((int)mouseX, (int)mouseY), null);
            spriteBatch.End();
        }

        private static Control GetControlByPosition(IList<Control> controls, Control def, float totalX, float totalY, float x, float y)
        {
            Control c = def;

            foreach (Control ctl in controls)
            {
                if (x > (totalX + ctl.Position.X) && y > (totalY + ctl.Position.Y) && x < (totalX + ctl.Position.X + ctl.Size.X) && y < (totalY + ctl.Position.Y + ctl.Size.Y))
                {
                    c = GetControlByPosition(ctl.Controls, ctl, totalX + ctl.Position.X, totalY + ctl.Position.Y, x, y);
                    break;
                }
            }

            return c;
        }

        private int m_handlePointer;
        public int GetNewHandle()
        {
            return ++m_handlePointer;
        }
    }
}
