#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: NodeRotationControllerValue.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using Axiom.Core;
using Axiom.Math;

#endregion Namespace Declarations

namespace Axiom.Controllers.Canned
{
    /// <summary>
    ///   Summary description for NodeRotationControllerValue.
    /// </summary>
    public class NodeRotationControllerValue : IControllerValue<Real>
    {
        // commented out (read access only private)
        //private float radians; //[FXCop Optimization : Do not initialize unnecessarily]
        private readonly Node node;
        private readonly Vector3 axis;

        public NodeRotationControllerValue(Node node, Vector3 axis)
        {
            this.node = node;
            this.axis = axis;
        }

        #region IControllerValue Members

        public Real Value
        {
            get
            {
                //return radians;
                return 0.0f;
            }
            set { this.node.Rotate(this.axis, value); }
        }

        #endregion
    }
}