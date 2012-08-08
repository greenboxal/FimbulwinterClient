#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: BoneCollection.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections.Generic;
using Axiom.Animating;
using Axiom.Collections;

#endregion

namespace Axiom.Animating.Collections
{
    /// <summary>
    ///   Represents a collection of <see cref="Bone">Bones</see> that are sorted by key.
    /// </summary>
    public class BoneCollection : AxiomSortedCollection<ushort, Bone>
    {
    }
}