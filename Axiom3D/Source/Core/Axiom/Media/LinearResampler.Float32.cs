#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id:"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

#endregion Namespace Declarations

namespace Axiom.Media
{
    partial class LinearResampler
    {
        /// <summary>
        ///   float32 linear resampler, converts FLOAT32_RGB/FLOAT32_RGBA only. avoids overhead of pixel unpack/repack function calls
        /// </summary>
        public class Float32
        {
            private int _count;

            public Float32()
                : this(1)
            {
            }

            public Float32(int count)
            {
                this._count = count;
            }

            public void Scale(PixelBox src, PixelBox dst)
            {
            }
        }
    }
}