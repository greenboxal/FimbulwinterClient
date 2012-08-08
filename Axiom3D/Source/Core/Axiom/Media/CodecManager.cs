#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: CodecManager.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using Axiom.Collections;
using Axiom.Core;

#endregion Namespace Declarations

namespace Axiom.Media
{
    /// <summary>
    ///   Manages registering/fulfilling requests for codecs that handle various types of media.
    /// </summary>
    public sealed class CodecManager : DisposableObject
    {
        #region Fields

        /// <summary>
        ///   Singleton instance of this class.
        /// </summary>
        private static CodecManager _instance;

        /// <summary>
        ///   List of registered media codecs.
        /// </summary>
        private readonly AxiomCollection<Codec> _mapCodecs = new AxiomCollection<Codec>();

        #endregion Fields

        #region Properties

        /// <summary>
        ///   Gets the singleton instance of this class.
        /// </summary>
        public static CodecManager Instance
        {
            get { return _instance; }
        }

        /// <summary>
        ///   Gets the file extension list for the registered codecs.
        /// </summary>
        [OgreVersion(1, 7, 2)]
        public string[] Extensions
        {
            get
            {
                string[] res = new string[this._mapCodecs.Count];
                this._mapCodecs.Keys.CopyTo(res, 0);
                return res;
            }
        }

        #endregion Properties

        /// <summary>
        ///   Internal constructor. This class cannot be instantiated externally.
        /// </summary>
        internal CodecManager()
        {
            if (_instance == null)
            {
                _instance = this;
            }
        }

        protected override void dispose(bool disposeManagedResources)
        {
            if (_instance == this)
            {
                if (disposeManagedResources)
                {
                    _instance = null;
                }
            }

            base.dispose(disposeManagedResources);
        }

        /// <summary>
        ///   Registers a new codec in the database.
        /// </summary>
        [OgreVersion(1, 7, 2)]
        public void RegisterCodec(Codec codec)
        {
            if (this._mapCodecs.ContainsKey(codec.Type))
            {
                throw new AxiomException("{0} already has a registered codec.", codec.Type);
            }

            this._mapCodecs[codec.Type] = codec;
        }

        /// <summary>
        ///   Return whether a codec is registered already.
        /// </summary>
        [OgreVersion(1, 7, 2)]
        public bool IsCodecRegistered(string codecType)
        {
            return this._mapCodecs.ContainsKey(codecType);
        }

        /// <summary>
        ///   Unregisters a codec from the database.
        /// </summary>
        [OgreVersion(1, 7, 2)]
        public void UnregisterCodec(Codec codec)
        {
            this._mapCodecs.TryRemove(codec.Type);
        }

        /// <summary>
        ///   Gets the codec registered for the passed in file extension.
        /// </summary>
        [OgreVersion(1, 7, 2)]
        public Codec GetCodec(string extension)
        {
            string lwrcase = extension.ToLower();
            if (!this._mapCodecs.ContainsKey(lwrcase))
            {
                string formatStr = string.Empty;
                if (this._mapCodecs.Count == 0)
                {
                    formatStr = "There are no formats supported (no codecs registered).";
                }
                else
                {
                    formatStr = "Supported formats are: " + string.Join(", ", Extensions);
                }

                throw new AxiomException("Can not find codec for '{0}' image format.\n{1}", extension, formatStr);
            }

            return this._mapCodecs[lwrcase];
        }

        /// <summary>
        ///   Gets the codec that can handle the given 'magic' identifier.
        /// </summary>
        /// <param name="magicNumberBuf"> Pointer to a stream of bytes which should identify the file. <note>Note that this may be more than needed - each codec may be looking for 
        ///                                                                                              a different size magic number.</note> </param>
        /// <param name="maxBytes"> The number of bytes passed </param>
        [OgreVersion(1, 7, 2)]
        public Codec GetCodec(byte[] magicNumberBuf, int maxBytes)
        {
            foreach (Codec i in this._mapCodecs)
            {
                string ext = i.MagicNumberToFileExt(magicNumberBuf, maxBytes);
                if (!string.IsNullOrEmpty(ext))
                {
                    // check codec type matches
                    // if we have a single codec class that can handle many types, 
                    // and register many instances of it against different types, we
                    // can end up matching the wrong one here, so grab the right one
                    if (ext == i.Type)
                    {
                        return i;
                    }
                    else
                    {
                        return GetCodec(ext);
                    }
                }
            }

            return null;
        }
    };
}