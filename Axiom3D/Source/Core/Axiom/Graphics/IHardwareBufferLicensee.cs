#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: IHardwareBufferLicensee.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;

#endregion Namespace Declarations

namespace Axiom.Graphics
{
    /// <summary>
    ///   Interface representing a 'licensee' of a hardware buffer copy.
    /// </summary>
    /// <remarks>
    ///   Often it's useful to have temporary buffers which are used for working
    ///   but are not necessarily needed permanently. However, creating and 
    ///   destroying buffers is expensive, so we need a way to share these 
    ///   working areas, especially those based on existing fixed buffers. 
    ///   Classes implementing this interface represent a licensee of one of those 
    ///   temporary buffers, and must be implemented by any user of a temporary buffer 
    ///   if they wish to be notified when the license is expired.
    /// </remarks>
    public interface IHardwareBufferLicensee
    {
        #region Methods

        /// <summary>
        ///   This method is called when the buffer license is expired and is about
        ///   to be returned to the shared pool.
        /// </summary>
        /// <param name="buffer"> </param>
        void LicenseExpired(HardwareBuffer buffer);

        #endregion Methods
    }
}