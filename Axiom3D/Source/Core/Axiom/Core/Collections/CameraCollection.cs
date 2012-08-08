#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: CameraCollection.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using Axiom.Core;
using Axiom.Collections;

#endregion

namespace Axiom.Collections
{
    /// <summary>
    ///   Represents a collection of <see cref="Camera">Cameras</see> that are sorted by name.
    /// </summary>
    public class CameraCollection : AxiomCollection<Camera>
    {
        public override void Add(Camera item)
        {
            base.Add(item.Name, item);
        }
    }
}