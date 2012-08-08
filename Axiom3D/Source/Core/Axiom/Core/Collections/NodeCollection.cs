#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: NodeCollection.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using Axiom.Core;
using Axiom.Collections;

#endregion

namespace Axiom.Core.Collections
{
    ///<summary>
    ///  Represents a collection of <see cref="Node">Nodes</see> that are sorted by name.
    ///</summary>
#if !( XBOX || XBOX360 )
    [Serializable]
#endif
    public class NodeCollection : AxiomCollection<Node>
    {
        #region Instance Methods

        ///<summary>
        ///  Adds a <see cref="Node" /> to the collection and uses its name automatically as key.
        ///</summary>
        ///<param name="item"> A <see cref="Node" /> to add to the collection. </param>
        public override void Add(Node item)
        {
            Add(item.Name, item);
        }

        /// <summary>
        ///   Removes the specified <see cref="Node" />.
        /// </summary>
        /// <param name="item"> The <see cref="Node" /> to remove. </param>
        public void Remove(Node item)
        {
            base.Remove(item.Name);
        }

        #endregion
    }
}