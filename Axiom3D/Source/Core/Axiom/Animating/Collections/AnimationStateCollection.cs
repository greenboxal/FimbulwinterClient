#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: AnimationStateCollection.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System.Collections.Generic;
using Axiom.Animating;
using Axiom.Collections;

#endregion

namespace Axiom.Animating.Collections
{
    ///<summary>
    ///  Represents a collection of <see cref="AnimationState">AnimationStates</see> that are sorted by name.
    ///</summary>
    public class AnimationStateCollection : AxiomCollection<AnimationState>
    {
        #region Instance Methods

        /// <summary>
        ///   Clones this instance.
        /// </summary>
        /// <returns> </returns>
        public AnimationStateCollection Clone()
        {
            AnimationStateCollection newCol = new AnimationStateCollection();

            foreach (AnimationState entry in this)
            {
                newCol.Add(entry.Name, entry);
            }

            return newCol;
        }

        #endregion
    };
}