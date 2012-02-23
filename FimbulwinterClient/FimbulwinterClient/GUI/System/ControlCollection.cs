using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace FimbulwinterClient.GUI.System
{
    public class ControlCollection : List<Control>
    {
        private Control m_owner;
        public Control Owner
        {
            get { return m_owner; }
            set { m_owner = value; }
        }

        public ControlCollection(Control owner)
        {
            m_owner = owner;
        }

        public new void Add(Control c)
        {
            c.Parent = m_owner;

            base.Add(c);
        }
    }
}
