#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: VertexBufferLicense.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;

#endregion Namespace Declarations

namespace Axiom.Graphics
{
    ///<summary>
    ///  Structure holding details of a license to use a temporary shared buffer.
    ///</summary>
    public class VertexBufferLicense
    {
        #region Fields

        public HardwareVertexBuffer originalBuffer;
        public BufferLicenseRelease licenseType;
        public HardwareVertexBuffer buffer;
        public IHardwareBufferLicensee licensee;
        public int expiredDelay;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// </summary>
        public VertexBufferLicense(HardwareVertexBuffer originalBuffer, BufferLicenseRelease licenseType,
                                   int expiredDelay,
                                   HardwareVertexBuffer buffer, IHardwareBufferLicensee licensee)
        {
            this.originalBuffer = originalBuffer;
            this.licenseType = licenseType;
            this.expiredDelay = expiredDelay;
            this.buffer = buffer;
            this.licensee = licensee;
        }

        #endregion Constructor
    }
}