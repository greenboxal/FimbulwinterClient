#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: Codec.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System.IO;
using Axiom.Math;

#endregion Namespace Declarations

namespace Axiom.Media
{
    /// <summary>
    ///   Abstract class that defines a 'codec'.
    /// </summary>
    /// <remarks>
    ///   A codec class works like a two-way filter for data - data entered on one end (the decode end) gets processed and transformed into easily usable data while data passed the other way around codes it back. @par The codec concept is a pretty generic one - you can easily understand how it can be used for images, sounds, archives, even compressed data.
    /// </remarks>
    public abstract class Codec
    {
        #region Nested Types

        public class CodecData
        {
            /// <summary>
            ///   Returns the type of the data.
            /// </summary>
            [OgreVersion(1, 7, 2)]
            public virtual string DataType
            {
                get { return "CodecData"; }
            }
        };

        /// <summary>
        ///   Result of a decoding; both a decoded data stream and CodecData metadata
        /// </summary>
        public class DecodeResult
        {
            private readonly Tuple<Stream, CodecData> _tuple;

            public Stream First
            {
                get { return this._tuple.First; }
            }

            public CodecData Second
            {
                get { return this._tuple.Second; }
            }

            public DecodeResult(Stream s, CodecData data)
            {
                this._tuple = new Tuple<Stream, CodecData>(s, data);
            }
        };

        #endregion Nested Types

        /// <summary>
        ///   Returns the type of the codec as a String
        /// </summary>
        [OgreVersion(1, 7, 2)]
        public abstract string Type { get; }

        /// <summary>
        ///   Returns the type of the data that supported by this codec as a String
        /// </summary>
        [OgreVersion(1, 7, 2)]
        public abstract string DataType { get; }

        /// <summary>
        ///   Codes the data in the input stream and saves the result in the output stream.
        /// </summary>
        [OgreVersion(1, 7, 2)]
        public abstract Stream Encode(Stream input, CodecData data);

        /// <summary>
        ///   Codes the data in the input chunk and saves the result in the output filename provided. Provided for efficiency since coding to memory is progressive therefore memory required is unknown leading to reallocations.
        /// </summary>
        /// <param name="input"> The input data </param>
        /// <param name="outFileName"> The filename to write to </param>
        /// <param name="data"> Extra information to be passed to the codec (codec type specific) </param>
        [OgreVersion(1, 7, 2)]
        public abstract void EncodeToFile(Stream input, string outFileName, CodecData data);

        /// <summary>
        ///   Codes the data from the input chunk into the output chunk.
        /// </summary>
        /// <param name="input"> Stream containing the encoded data </param>
        [OgreVersion(1, 7, 2)]
        public abstract DecodeResult Decode(Stream input);

        /// <summary>
        ///   Returns whether a magic number header matches this codec.
        /// </summary>
        /// <param name="magicNumberBuf"> Pointer to a stream of bytes which should identify the file. <note>Note that this may be more than needed - each codec may be looking for 
        ///                                                                                              a different size magic number.</note> </param>
        /// <param name="maxBytes"> The number of bytes passed </param>
        [OgreVersion(1, 7, 2)]
        public virtual bool MagicNumberMatch(byte[] magicNumberBuf, int maxBytes)
        {
            return !string.IsNullOrEmpty(MagicNumberToFileExt(magicNumberBuf, maxBytes));
        }

        /// <summary>
        ///   Maps a magic number header to a file extension, if this codec recognises it.
        /// </summary>
        /// <param name="magicNumberBuf"> Pointer to a stream of bytes which should identify the file. Note that this may be more than needed - each codec may be looking for a different size magic number. </param>
        /// <param name="maxBytes"> The number of bytes passed </param>
        /// <returns> A blank string if the magic number was unknown, or a file extension. </returns>
        [OgreVersion(1, 7, 2)]
        public abstract string MagicNumberToFileExt(byte[] magicNumberBuf, int maxBytes);
    };
}