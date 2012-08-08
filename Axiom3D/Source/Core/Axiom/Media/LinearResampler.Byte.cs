﻿#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id:"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;

#endregion Namespace Declarations

namespace Axiom.Media
{
    partial class LinearResampler
    {
        /// <summary>
        ///   byte linear resampler, does not do any format conversions.
        /// </summary>
        /// <remarks>
        ///   only handles pixel formats that use 1 byte per color channel. 2D only; punts 3D pixelboxes to default <see
        ///    cref="LinearResampler" /> (slow). templated on bytes-per-pixel to allow compiler optimizations, such as unrolling loops and replacing multiplies with bitshifts
        /// </remarks>
        public class Byte
        {
            private readonly int _channels;

            public Byte()
                : this(1)
            {
            }

            public Byte(int channels)
            {
                this._channels = channels;
            }

            public void Scale(PixelBox src, PixelBox dst)
            {
                // assert(src.format == dst.format);

                // only optimized for 2D
                if (src.Depth > 1 || dst.Depth > 1)
                {
                    (new LinearResampler()).Scale(src, dst);
                    return;
                }

#if !AXIOM_SAFE_ONLY
                unsafe
#endif
                {
                    // srcdata stays at beginning of slice, pdst is a moving pointer
                    byte* srcdata = src.Data.ToBytePointer();
                    byte* dstData = dst.Data.ToBytePointer();
                    int pdst = 0;

                    // sx_48,sy_48 represent current position in source
                    // using 16/48-bit fixed precision, incremented by steps
                    ulong stepx = (UInt64) ((src.Width << 48)/dst.Width);
                    ulong stepy = (UInt64) ((src.Height << 48)/dst.Height);

                    // bottom 28 bits of temp are 16/12 bit fixed precision, used to
                    // adjust a source coordinate backwards by half a pixel so that the
                    // integer bits represent the first sample (eg, sx1) and the
                    // fractional bits are the blend weight of the second sample
                    uint temp;

                    ulong sy_48 = (stepy >> 1) - 1;
                    for (uint y = (uint) dst.Top; y < dst.Bottom; y++, sy_48 += stepy)
                    {
                        temp = (uint) (sy_48 >> 36);
                        temp = (temp > 0x800) ? temp - 0x800 : 0;
                        uint syf = temp & 0xFFF;
                        uint sy1 = temp >> 12;
                        uint sy2 = (uint) System.Math.Min(sy1 + 1, src.Bottom - src.Top - 1);
                        uint syoff1 = (uint) (sy1*src.RowPitch);
                        uint syoff2 = (uint) (sy2*src.RowPitch);

                        ulong sx_48 = (stepx >> 1) - 1;
                        for (uint x = (uint) dst.Left; x < dst.Right; x++, sx_48 += stepx)
                        {
                            temp = (uint) (sx_48 >> 36);
                            temp = (temp > 0x800) ? temp - 0x800 : 0;
                            uint sxf = temp & 0xFFF;
                            uint sx1 = temp >> 12;
                            uint sx2 = (uint) System.Math.Min(sx1 + 1, src.Right - src.Left - 1);

                            uint sxfsyf = sxf*syf;
                            for (uint k = 0; k < this._channels; k++)
                            {
                                uint accum =
                                    (uint)
                                    (srcdata[(int) ((sx1 + syoff1)*this._channels + k)]*
                                     (char) (0x1000000 - (sxf << 12) - (syf << 12) + sxfsyf) +
                                     srcdata[(int) ((sx2 + syoff1)*this._channels + k)]*(char) ((sxf << 12) - sxfsyf) +
                                     srcdata[(int) ((sx1 + syoff2)*this._channels + k)]*(char) ((syf << 12) - sxfsyf) +
                                     srcdata[(int) ((sx2 + syoff2)*this._channels + k)]*(char) sxfsyf);
                                // accum is computed using 8/24-bit fixed-point math
                                // (maximum is 0xFF000000; rounding will not cause overflow)
                                dstData[pdst++] = (byte) ((accum + 0x800000) >> 24);
                            }
                        }
                        pdst += this._channels*dst.RowSkip;
                    }
                }
            }
        }
    }
}