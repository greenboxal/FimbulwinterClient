#region SVN Version Information

// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <id value="$Id: ICompositorLogic.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections.Generic;

#endregion Namespace Declarations

namespace Axiom.Graphics
{
    /// <summary>
    ///   Interface for compositor logics, which can be automatically bound to compositors,
    ///   allowing per-compositor logic (such as attaching a relevant listener) to happen
    ///   automatically.
    /// </summary>
    public interface ICompositorLogic
    {
        /// <summary>
        ///   Called when a compositor instance has been created.
        /// </summary>
        /// <remarks>
        ///   This happens after its setup was finished, so the chain is also accessible.
        ///   This is an ideal method to automatically attach a compositor listener.
        /// </remarks>
        /// <param name="newInstance"> </param>
        void CompositorInstanceCreated(CompositorInstance newInstance);

        /// <summary>
        ///   Called when a compositor instance has been destroyed
        /// </summary>
        /// <remarks>
        ///   The chain that contained the compositor is still alive during this call.
        /// </remarks>
        /// <param name="destroyedInstance"> </param>
        void CompositorInstanceDestroyed(CompositorInstance destroyedInstance);
    }

    /// <summary>
    ///   Implementation base class for compositor logics, which can be automatically bound to compositors,
    ///   allowing per-compositor logic (such as attaching a relevant listener) to happen
    ///   automatically.
    /// </summary>
    /// <remarks>
    ///   All methods have empty implementations to not force an implementer into
    ///   extending all of them.
    /// </remarks>
    public class CompositorLogic : ICompositorLogic
    {
        #region Implementation of ICompositorLogic

        /// <summary>
        ///   Called when a compositor instance has been created.
        /// </summary>
        /// <remarks>
        ///   This happens after its setup was finished, so the chain is also accessible.
        ///   This is an ideal method to automatically attach a compositor listener.
        /// </remarks>
        /// <param name="newInstance"> </param>
        public virtual void CompositorInstanceCreated(CompositorInstance newInstance)
        {
        }

        /// <summary>
        ///   Called when a compositor instance has been destroyed
        /// </summary>
        /// <remarks>
        ///   The chain that contained the compositor is still alive during this call.
        /// </remarks>
        /// <param name="destroyedInstance"> </param>
        public virtual void CompositorInstanceDestroyed(CompositorInstance destroyedInstance)
        {
        }

        #endregion
    }
}