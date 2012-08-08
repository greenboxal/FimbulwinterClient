#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: MovableObjectCollection.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System.Collections.Generic;
using Axiom.Core;
using Axiom.Collections;

#endregion

namespace Axiom.Collections
{
    /// <summary>
    ///   Represents a collection of <see cref="MovableObject">MovableObjects</see> that are sorted by name.
    /// </summary>
    public class MovableObjectCollection : AxiomCollection<MovableObject>
    {
        public override void Add(MovableObject item)
        {
            base.Add(item.Name, item);
        }

        public new void Add(string key, MovableObject item)
        {
            base.Add(key, item);
            item.ObjectRenamed += ObjectRenamed;
        }

        public new void Remove(string key)
        {
            this[key].ObjectRenamed -= ObjectRenamed;
            base.Remove(key);
        }

        private void ObjectRenamed(MovableObject obj, string oldName)
        {
            // do not use overridden Add methods otherwise
            // the event handler will be attached again.
            base.Remove(oldName);
            base.Add(obj.Name, obj);
        }
    }

    /// <summary>
    ///   Represents a collection of <see cref="MovableObjectFactory">MovableObjectFactorys</see> accessable by name.
    /// </summary>
    public class MovableObjectFactoryMap : Dictionary<string, MovableObjectFactory>
    {
    }
}