#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: SceneQueryMovableObjectWorldFragmentIntersectionList.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using Axiom.Core;
using System.Collections.Generic;

#endregion

namespace Axiom.Collections
{
    /// <summary>
    ///   Represents a pair of a <see cref="MovableObject" /> and a <see cref="SceneQuery.WorldFragment" />.
    /// </summary>
    public class SceneQueryMovableObjectWorldFragmentPair
    {
        #region Constructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="SceneQueryMovableObjectWorldFragmentPair" /> class.
        /// </summary>
        /// <param name="obj"> A <see cref="MovableObject" /> . </param>
        /// <param name="fragment"> A <see cref="SceneQuery.WorldFragment" /> . </param>
        public SceneQueryMovableObjectWorldFragmentPair(MovableObject obj, SceneQuery.WorldFragment fragment)
        {
            this.obj = obj;
            this.fragment = fragment;
        }

        #endregion

        #region Instance Properties

        /// <summary>
        ///   Gets or sets the <see cref="SceneQuery.WorldFragment" />.
        /// </summary>
        /// <value> A <see cref="SceneQuery.WorldFragment" /> . </value>
        public SceneQuery.WorldFragment fragment { get; set; }

        /// <summary>
        ///   Gets or sets the <see cref="MovableObject" />.
        /// </summary>
        /// <value> A <see cref="MovableObject" /> . </value>
        public MovableObject obj { get; set; }

        #endregion
    }

    /// <summary>
    ///   Represents a collection of <see cref="SceneQueryMovableObjectWorldFragmentPair">SceneQueryMovableObjectWorldFragmentPairs</see> that are sorted by name.
    /// </summary>
    public class SceneQueryMovableObjectWorldFragmentIntersectionList : List<SceneQueryMovableObjectWorldFragmentPair>
    {
    }
}