#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: DefaultHardwareBufferManager.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections.Generic;
using System.Text;
using Axiom.Core;

#endregion Namespace Declarations

namespace Axiom.Graphics
{
    public class DefaultHardwareBufferManager : HardwareBufferManager
    {
        public DefaultHardwareBufferManager()
            : base(new DefaultHardwareBufferManagerBase())
        {
        }

        protected override void dispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                _baseInstance.Dispose();
                _baseInstance = null;
            }
            base.dispose(disposeManagedResources);
        }
    }
}