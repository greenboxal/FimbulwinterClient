#region SVN Version Information

// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <id value="$Id: ICustomCompositionPass.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections.Generic;

#endregion Namespace Declarations

namespace Axiom.Graphics
{
    /// <summary>
    ///   Interface for custom composition passes, allowing custom operations (in addition to
    ///   the quad, scene and clear operations) in composition passes.
    ///   <seealso cref="CompositorManager.RegisterCustomCompositorPass" />
    /// </summary>
    public interface ICustomCompositionPass
    {
        /// <summary>
        ///   Create a custom composition operation.
        /// </summary>
        /// <param name="instance"> The compositor instance that this operation will be performed in </param>
        /// <param name="pass"> The CompositionPass that triggered the request </param>
        /// <returns> </returns>
        /// <remarks>
        ///   This call only happens once during creation. The CompositeRenderSystemOperation will
        ///   get called each render.
        /// </remarks>
        CompositeRenderSystemOperation CreateOperation(CompositorInstance instance, CompositionPass pass);
    }
}