using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace FimbulwinterClient.GUI.System
{
    public class ControlCollection : List<Control>
    {
        private Control _owner;
        public Control Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        public ControlCollection(Control owner)
        {
            _owner = owner;
        }

        public new void Add(Control c)
        {
            c.Parent = _owner;

            base.Add(c);

            SortByZOrder();
        }

        public void SortByZOrder()
        {
            Sort(ZOrderComparer);
        }

        private int ZOrderComparer(Control f, Control l)
        {
            return f.ZOrder - l.ZOrder;
        }
    }
}
