#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: SceneQueryMovableObjectIntersectionList.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using Axiom.Core;
using System.Collections.Generic;

#endregion

namespace Axiom.Collections
{
    /// <summary>
    ///   Represents a pair of two <see cref="MovableObject">MovableObjects</see>.
    /// </summary>
    public class SceneQueryMovableObjectPair
    {
        #region Constructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="SceneQueryMovableObjectPair" /> class.
        /// </summary>
        /// <param name="first"> The first <see cref="MovableObject" /> . </param>
        /// <param name="second"> The second <see cref="MovableObject" /> . </param>
        public SceneQueryMovableObjectPair(MovableObject first, MovableObject second)
        {
            this.first = first;
            this.second = second;
        }

        #endregion

        #region Instance Properties

        /// <summary>
        ///   Gets or sets the first <see cref="MovableObject" />.
        /// </summary>
        /// <value> A <see cref="MovableObject" /> . </value>
        public MovableObject first { get; set; }

        /// <summary>
        ///   Gets or sets the second <see cref="MovableObject" />.
        /// </summary>
        /// <value> A <see cref="MovableObject" /> . </value>
        public MovableObject second { get; set; }

        #endregion
    }

    /// <summary>
    ///   Represents a collection of <see cref="SceneQueryMovableObjectPair">SceneQueryMovableObjectPairs</see>
    /// </summary>
    public class SceneQueryMovableObjectIntersectionList : List<SceneQueryMovableObjectPair>
    {
    }
}