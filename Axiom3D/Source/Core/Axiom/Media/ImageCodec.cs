#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: ImageCodec.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

#endregion Namespace Declarations

namespace Axiom.Media
{
    /// <summary>
    ///   Codec specialized in images.
    /// </summary>
    /// <remarks>
    ///   The users implementing subclasses of ImageCodec are required to return a valid pointer to a ImageData class from the decode(...) function.
    /// </remarks>
    public abstract class ImageCodec : Codec
    {
        [OgreVersion(1, 7, 2)]
        public override string DataType
        {
            get { return "ImageData"; }
        }

        /// <summary>
        ///   Codec return class for images. Has information about the size and the pixel format of the image.
        /// </summary>
        public class ImageData : CodecData
        {
            [OgreVersion(1, 7, 2)]
            public override string DataType
            {
                get { return "ImageData"; }
            }

            public int width;
            public int height;
            public int depth = 1;
            public int size;
            public ImageFlags flags;
            public int numMipMaps;
            public PixelFormat format;
        };
    };
}