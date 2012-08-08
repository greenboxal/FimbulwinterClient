#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: RenderTexture.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using Axiom.Core;
using Axiom.Media;

#endregion Namespace Declarations

namespace Axiom.Graphics
{
    /// <summary>
    ///   Custom RenderTarget that allows for rendering a scene to a texture.
    /// </summary>
    public abstract class RenderTexture : RenderTarget
    {
        #region Fields

        protected HardwarePixelBuffer pixelBuffer;
        protected int zOffset = 0;

        #endregion Fields

        #region Constructors

        [OgreVersion(1, 7, 2)]
        public RenderTexture(HardwarePixelBuffer buffer, int zOffset)
        {
            this.pixelBuffer = buffer;
            this.zOffset = zOffset;
            Priority = RenderTargetPriority.RenderToTexture;
            width = buffer.Width;
            height = buffer.Height;
            colorDepth = PixelUtil.GetNumElemBits(buffer.Format);
        }

        #endregion Constructors

        #region Methods

        public override void CopyContentsToMemory(PixelBox dst, RenderTarget.FrameBuffer buffer)
        {
            if (buffer == FrameBuffer.Auto)
            {
                buffer = FrameBuffer.Front;
            }
            if (buffer != FrameBuffer.Front)
            {
                throw new Exception("Invalid buffer.");
            }

            this.pixelBuffer.BlitToMemory(dst);
        }

        public override PixelFormat SuggestPixelFormat()
        {
            return this.pixelBuffer.Format;
        }

        /// <summary>
        ///   Ensures texture is destroyed.
        /// </summary>
        protected override void dispose(bool disposeManagedResources)
        {
            if (!IsDisposed)
            {
                if (disposeManagedResources)
                {
                    this.pixelBuffer.ClearSliceRTT(0);
                }
            }

            // If it is available, make the call to the
            // base class's Dispose(Boolean) method
            base.dispose(disposeManagedResources);
        }

        #endregion Methods
    }
}