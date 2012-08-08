#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: Lists.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System.Collections.Generic;
using Axiom.Animating;

#endregion

namespace Axiom.Animating.Collections
{
    /// <summary>
    ///   Represents a collection of <see cref="KeyFrame">KeyFrames</see>.
    /// </summary>
    public class KeyFrameList : List<KeyFrame>
    {
    }

    /// <summary>
    ///   Represents a collection of <see cref="Bone">Bones</see>.
    /// </summary>
    public class BoneList : List<Bone>
    {
    }
}