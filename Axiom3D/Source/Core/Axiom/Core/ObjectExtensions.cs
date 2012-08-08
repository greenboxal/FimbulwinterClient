#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: ObjectExtensions.cs 3308 2012-05-31 15:13:54Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;

#endregion Namespace Declarations

namespace Axiom.Core
{
    public static class ObjectExtensions
    {
        /// <summary>
        ///   Extension method to safely dispose any object.
        /// </summary>
        /// <param name="disposable"> The object being disposed </param>
        /// <remarks>
        ///   Does nothing if the object is null or does not implement IDisposable.
        /// </remarks>
        [AxiomHelper(0, 9)]
        public static void SafeDispose(this object disposable)
        {
            // first sanity check on disposable
            if (null == disposable || !(disposable is IDisposable))
            {
                return;
            }

            // Only call Dispose if the DisposableObject has not been Disposed 
            // or the object simply implements IDisposable
            if (!(disposable is DisposableObject) || !((DisposableObject) disposable).IsDisposed)
            {
                ((IDisposable) disposable).Dispose();
            }
        }
    };
}