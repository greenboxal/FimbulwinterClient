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


        private Control m_activeControl;
        public Control ActiveControl
        {
            get { return m_activeControl; }
            set { m_activeControl = value; }
        }

        SpriteAction m_cursor;
        public SpriteAction Cursor
        {
            get { return m_cursor; }
            set { m_cursor = value; }
        }

        private Queue<Control> m_deleteQueue;
        private SpriteBatch spriteBatch;

        float mouseX;
        float mouseY;

        private Control m_hoverControl;
        private Control m_downControl;
        private MouseButtons m_downButtons;

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
            m_deleteQueue = new Queue<Control>();

            m_cursor = m_client.ContentManager.LoadContent<SpriteAction>("data\\sprite\\cursors.act");
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
            if (m_activeControl != null && character == '\t' && !m_activeControl.HandleTab())
            {
                if (m_activeControl.Parent != null)
                {
                    Control parent = m_activeControl.Parent;
                    int cIdx = -1;

                    for (int i = 0; i < parent.Controls.Count; i++)
                    {
                        if (parent.Controls[i] == m_activeControl)
                        {
                            cIdx = i;
                            break;
                        }
                    }

                    if (cIdx != -1)
                    {
                        cIdx++;
                        if (cIdx >= parent.Controls.Count)
                            cIdx = 0;

                        Control next = parent.Controls[cIdx];

                        int started = cIdx;
                        while (!next.TabStop)
                        {
                            cIdx++;
                            if (cIdx >= parent.Controls.Count)
                                cIdx = 0;

                            next = parent.Controls[cIdx];

                            if (cIdx == started)
                                break;
                        }

                        SetActiveControl(parent.Controls[cIdx]);
                    }

                    return;
                }
            }

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
                m_hoverControl.OnMouseMove(mouseX - m_hoverControl.GetAbsX(), mouseY - m_hoverControl.GetAbsY());
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
                    m_hoverControl.OnMouseMove(mouseX - m_hoverControl.GetAbsX(), mouseY - m_hoverControl.GetAbsY());
            }
        }

        void mouse_MouseButtonReleased(MouseButtons buttons)
        {
            Control c = GetControlByPosition(m_controls, null, 0, 0, (int)mouseX, (int)mouseY);

            if (m_downControl != null)
            {
                m_downControl.OnMouseUp(buttons, mouseX - m_downControl.GetAbsX(), mouseY - m_downControl.GetAbsY());

                if (m_downControl == c && (m_downButtons & buttons) != 0)
                {
                    m_downControl.OnClick(m_downButtons & buttons, mouseX - m_downControl.GetAbsX(), mouseY - m_downControl.GetAbsY());
                }
            }

            m_downButtons &= ~buttons;
        }

        void mouse_MouseButtonPressed(MouseButtons buttons)
        {
            SetActiveControl(GetControlByPosition(m_controls, null, 0, 0, (int)mouseX, (int)mouseY));

            if (m_activeControl != null)
                m_activeControl.OnMouseDown(buttons, mouseX - m_activeControl.GetAbsX(), mouseY - m_activeControl.GetAbsY());

            m_downControl = m_activeControl;
            m_downButtons |= buttons;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            while (m_deleteQueue.Count > 0)
                m_controls.Remove(m_deleteQueue.Dequeue());

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

            m_cursor.Draw(spriteBatch, new Point((int)mouseX, (int)mouseY), null, false);
            spriteBatch.End();
        }

        private static Control GetControlByPosition(IList<Control> controls, Control def, float totalX, float totalY, float x, float y)
        {
            Control c = def;

            for (int i = controls.Count - 1; i >= 0; --i)
            {
                Control ctl = controls[i];

                if (x >= (totalX + ctl.Position.X) && y > (totalY + ctl.Position.Y) && x < (totalX + ctl.Position.X + ctl.Size.X) && y < (totalY + ctl.Position.Y + ctl.Size.Y))
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

        public void EnqueueRemove(Window window)
        {
            m_deleteQueue.Enqueue(window);
        }

        public void SetActiveControl(Control c)
        {
            m_activeControl = c;

            Control tmp = m_activeControl;
            while (tmp != null && tmp.Parent != null)
            {
                tmp = tmp.Parent;
            }

            if (tmp != null)
            {
                if (m_controls.Contains(tmp))
                {
                    m_controls.Remove(tmp);
                    m_controls.Add(tmp);
                }
            }
        }
    }
}
