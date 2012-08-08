#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id:"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using Axiom.Core;
using Axiom.Graphics;

#endregion Namespace Declarations

namespace Axiom.Graphics
{
    public class RenderQueueInvocation
    {
        #region Fields and Properties

        public static readonly string Shadows = "SHADOWS";
        public string Name;
        public uint RenderQueueGroupID;

        #endregion Fields and Properties

        #region Construction and Destruction

        #endregion Construction and Destruction

        #region Methods

        internal void Invoke(RenderQueueGroup queueGroup, SceneManager sceneManager)
        {
            throw new NotImplementedException();
        }

        #endregion Methods
    }
}