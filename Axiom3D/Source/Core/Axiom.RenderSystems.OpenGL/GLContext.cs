#region SVN Version Information

// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <id value="$Id: GLContext.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;

#endregion Namespace Declarations

namespace Axiom.RenderSystems.OpenGL
{
    /// <summary>
    ///   Class that encapsulates an GL context. (IE a window/pbuffer). This is a
    ///   virtual base class which should be implemented in a GLSupport.
    ///   This object can also be used to cache renderstate if we decide to do so
    ///   in the future.
    /// </summary>
    internal abstract class GLContext : IDisposable
    {
        #region Fields and Properties

        #region Initialized Property

        ///<summary>
        ///</summary>
        public bool Initialized { get; set; }

        #endregion Initialized Property

        #region VSync Property

        ///<summary>
        ///</summary>
        public abstract bool VSync { get; set; }

        #endregion VSync Property

        #endregion Fields and Properties

        #region Construction and Destruction

        ~GLContext()
        {
            dispose(false);
        }

        #endregion Construction and Destruction

        #region Methods

        /// <summary>
        ///   Enable the context. All subsequent rendering commands will go here.
        /// </summary>
        public abstract void SetCurrent();

        /// <summary>
        ///   This is called before another context is made current. By default,
        ///   nothing is done here.
        /// </summary>
        public virtual void EndCurrent()
        {
        }

        public virtual void ReleaseContext()
        {
        }

        ///<summary>
        ///  Create a new context based on the same window/pbuffer as this
        ///  context - mostly useful for additional threads.
        ///</summary>
        ///<remarks>
        ///  The caller is responsible for deleting the returned context.
        ///</remarks>
        ///<returns> </returns>
        public abstract GLContext Clone();

        #endregion Methods

        #region IDisposable Implementation

        #region isDisposed Property

        /// <summary>
        ///   Determines if this instance has been disposed of already.
        /// </summary>
        protected bool isDisposed { get; set; }

        #endregion isDisposed Property

        ///<summary>
        ///  Class level dispose method
        ///</summary>
        ///<remarks>
        ///  When implementing this method in an inherited class the following template should be used;
        ///  protected override void dispose( bool disposeManagedResources )
        ///  {
        ///  if ( !isDisposed )
        ///  {
        ///  if ( disposeManagedResources )
        ///  {
        ///  // Dispose managed resources.
        ///  }
        ///
        ///  // There are no unmanaged resources to release, but
        ///  // if we add them, they need to be released here.
        ///  }
        ///
        ///  // If it is available, make the call to the
        ///  // base class's Dispose(Boolean) method
        ///  base.dispose( disposeManagedResources );
        ///  }
        ///</remarks>
        ///<param name="disposeManagedResources"> True if Unmanaged resources should be released. </param>
        protected virtual void dispose(bool disposeManagedResources)
        {
            if (!isDisposed)
            {
                if (disposeManagedResources)
                {
                    // Dispose managed resources.
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }
            isDisposed = true;
        }

        public void Dispose()
        {
            dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Implementation
    }
}