using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Nuclex.Input;
using Nuclex.Input.Devices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FimbulwinterClient.Core.Assets;
using FimbulwinterClient.Core;

namespace FimbulwinterClient.GUI.System
{
    public class GuiManager : DrawableGameComponent
    {
        private static GuiManager _singleton;
        public static GuiManager Singleton
        {
            get { return GuiManager._singleton; }
        }

        private ROClient _client;
        public ROClient Client
        {
            get { return _client; }
        }

        private List<Control> _controls;
        public List<Control> Controls
        {
            get { return _controls; }
        }


        private Control _activeControl;
        public Control ActiveControl
        {
            get { return _activeControl; }
            set { _activeControl = value; }
        }

        SpriteAction _cursor;
        public SpriteAction Cursor
        {
            get { return _cursor; }
            set { _cursor = value; }
        }

        private Queue<Control> _deleteQueue;
        private SpriteBatch spriteBatch;

        float mouseX;
        float mouseY;

        private Control _hoverControl;
        private Control _downControl;
        private MouseButtons _downButtons;

        public GuiManager(ROClient roc)
            : base(roc)
        {
            _client = roc;

            _singleton = this;
        }

        public override void Initialize()
        {
            base.Initialize();

            spriteBatch = new SpriteBatch(_client.GraphicsDevice);
            _controls = new List<Control>();
            _deleteQueue = new Queue<Control>();

            _cursor = SharedInformation.ContentManager.Load<SpriteAction>("data\\sprite\\cursors.act");
            _cursor.Loop = true;

            InputManager im = (InputManager)_client.Services.GetService(typeof(InputManager));

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
            if (_activeControl != null && character == '\t' && !_activeControl.HandleTab())
            {
                if (_activeControl.Parent != null)
                {
                    Control parent = _activeControl.Parent;
                    int cIdx = -1;

                    for (int i = 0; i < parent.Controls.Count; i++)
                    {
                        if (parent.Controls[i] == _activeControl)
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

            if (_activeControl != null)
                _activeControl.OnKeyPress(character);
        }

        void kb_KeyReleased(Keys key)
        {
            if (_activeControl != null)
                _activeControl.OnKeyUp(key);
        }

        void kb_KeyPressed(Keys key)
        {
            if (_activeControl != null)
                _activeControl.OnKeyDown(key);
        }

        void mouse_MouseWheelRotated(float ticks)
        {
            if (_activeControl != null)
                _activeControl.OnMouseWheel(ticks);
        }

        void mouse_MouseMoved(float x, float y)
        {
            mouseX = x;
            mouseY = y;

            if ((_downButtons & MouseButtons.Left) != 0 && _downControl != null)
            {
                _hoverControl.OnMouseMove(mouseX - _hoverControl.GetAbsX(), mouseY - _hoverControl.GetAbsY());
            }
            else
            {
                Control c = GetControlByPosition(_controls, null, 0, 0, (int)mouseX, (int)mouseY);

                if (_hoverControl != c)
                {
                    if (_hoverControl != null)
                        _hoverControl.OnMouseLeave();

                    _hoverControl = c;

                    if (_hoverControl != null)
                        _hoverControl.OnMouseHover();
                }

                if (_hoverControl != null)
                    _hoverControl.OnMouseMove(mouseX - _hoverControl.GetAbsX(), mouseY - _hoverControl.GetAbsY());
            }
        }

        void mouse_MouseButtonReleased(MouseButtons buttons)
        {
            Control c = GetControlByPosition(_controls, null, 0, 0, (int)mouseX, (int)mouseY);

            if (_downControl != null)
            {
                _downControl.OnMouseUp(buttons, mouseX - _downControl.GetAbsX(), mouseY - _downControl.GetAbsY());

                if (_downControl == c && (_downButtons & buttons) != 0)
                {
                    _downControl.OnClick(_downButtons & buttons, mouseX - _downControl.GetAbsX(), mouseY - _downControl.GetAbsY());
                }
            }

            _downButtons &= ~buttons;
        }

        void mouse_MouseButtonPressed(MouseButtons buttons)
        {
            SetActiveControl(GetControlByPosition(_controls, null, 0, 0, (int)mouseX, (int)mouseY));

            if (_activeControl != null)
                _activeControl.OnMouseDown(buttons, mouseX - _activeControl.GetAbsX(), mouseY - _activeControl.GetAbsY());

            _downControl = _activeControl;
            _downButtons |= buttons;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            while (_deleteQueue.Count > 0)
                _controls.Remove(_deleteQueue.Dequeue());

            for (int i = 0; i < _controls.Count; i++)
            {
                if (_controls[i].Visible && _controls[i].Enabled)
                    _controls[i].Update(gameTime);
            }

            _cursor.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            spriteBatch.Begin();
            for (int i = 0; i < _controls.Count; i++)
            {
                if (_controls[i].Visible)
                    _controls[i].Draw(spriteBatch, gameTime);
            }

            _cursor.Draw(spriteBatch, new Point((int)mouseX, (int)mouseY), null, false);
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

        private int _handlePointer;
        public int GetNewHandle()
        {
            return ++_handlePointer;
        }

        public void EnqueueRemove(Window window)
        {
            _deleteQueue.Enqueue(window);
        }

        public void SetActiveControl(Control c)
        {
            _activeControl = c;

            Control tmp = _activeControl;
            while (tmp != null && tmp.Parent != null)
            {
                tmp = tmp.Parent;
            }

            if (tmp != null)
            {
                if (_controls.Contains(tmp))
                {
                    _controls.Remove(tmp);
                    _controls.Add(tmp);
                }
            }
        }
    }
}
