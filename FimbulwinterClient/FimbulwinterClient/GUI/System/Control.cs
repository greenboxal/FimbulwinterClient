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

        protected float GetAbsX()
        {
            if (m_parent != null)
                return m_parent.GetAbsX() + m_position.X;
            else
                return m_position.X;
        }

        protected float GetAbsY()
        {
            if (m_parent != null)
                return m_parent.GetAbsY() + m_position.Y;
            else
                return m_position.Y;
        }

        public Control()
        {
            m_controls = new ControlCollection(this);

            m_visible = true;
            m_enabled = true;

            m_handle = GuiManager.Singleton.GetNewHandle();
        }

        public virtual void Update(GameTime gt)
        {

        }

        public virtual void Draw(SpriteBatch sb, GameTime gt)
        {

        }

        public virtual void OnClick(MouseButtons buttons, float x, float y)
        {

        }

        public virtual void OnMouseDown(MouseButtons buttons, float x, float y)
        {

        }

        public virtual void OnMouseUp(MouseButtons buttons, float x, float y)
        {

        }

        public virtual void OnMouseMove(float x, float y)
        {

        }

        public virtual void OnMouseHover()
        {

        }

        public virtual void OnMouseLeave()
        {
        }

        public virtual void OnMouseWheel(float ticks)
        {

        }

        public virtual void OnKeyDown(Keys key)
        {

        }

        public virtual void OnKeyUp(Keys key)
        {

        }

        public virtual void OnKeyPress(char c)
        {

        }

        public override bool Equals(object obj)
        {
            if (typeof(Control) == obj.GetType() || obj.GetType().IsSubclassOf(typeof(Control)))
                return ((Control)obj).Handle == this.Handle;

            return base.Equals(obj);
        }
    }
}
