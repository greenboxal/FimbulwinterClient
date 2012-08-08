#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: AnimationTrackCollection.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using Axiom.Animating;
using Axiom.Core;
using Axiom.Collections;

#endregion

namespace Axiom.Animating.Collections
{
    /// <summary>
    ///   Represents a collection of <see cref="SceneNode">SceneNodes</see> that are sorted by key.
    /// </summary>
    public class AnimationTrackCollection : AxiomSortedCollection<ushort, AnimationTrack>
    {
        #region Instance Methods

        ///<summary>
        ///  Adds an <see cref="AnimationTrack" /> to the collection and uses its handle automatically as the key.
        ///</summary>
        ///<param name="item"> </param>
        public void Add(AnimationTrack item)
        {
            Add(item.Handle, item);
        }

        #endregion
    }
}