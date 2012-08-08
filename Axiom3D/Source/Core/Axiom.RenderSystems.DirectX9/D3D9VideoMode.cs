#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: D3D9VideoMode.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using Axiom.Core;
using D3D = SharpDX.Direct3D9;

#endregion Namespace Declarations

namespace Axiom.RenderSystems.DirectX9
{
    /// <summary>
    ///   Summary description for D3DVideoMode.
    /// </summary>
    public class D3D9VideoMode : DisposableObject
    {
        #region Member variables

        private D3D.DisplayMode displayMode;
        private int modeNum;
        private static int modeCount;

        #endregion Member variables

        #region Constructors

        ///<summary>
        ///  Default constructor.
        ///</summary>
        [OgreVersion(1, 7, 2)]
        public D3D9VideoMode()
        {
            this.modeNum = ++modeCount;
            this.displayMode = new D3D.DisplayMode();
        }

        ///<summary>
        ///  Accepts a existing D3DVideoMode object.
        ///</summary>
        [OgreVersion(1, 7, 2)]
        public D3D9VideoMode(D3D9VideoMode videoMode)
        {
            this.modeNum = ++modeCount;
            this.displayMode = videoMode.displayMode;
        }

        ///<summary>
        ///  Accepts a existing Direct3D.DisplayMode object.
        ///</summary>
        public D3D9VideoMode(D3D.DisplayMode videoMode)
        {
            this.modeNum = ++modeCount;
            this.displayMode = videoMode;
        }

        #endregion Constructors

        [OgreVersion(1, 7, 2, "~D3D9VideoMode")]
        protected override void dispose(bool disposeManagedResources)
        {
            if (!IsDisposed)
            {
                if (disposeManagedResources)
                {
                    modeCount--;
                }
            }
            base.dispose(disposeManagedResources);
        }

        ///<summary>
        ///  Returns a string representation of this video mode.
        ///</summary>
        [OgreVersion(1, 7, 2, "getDescription")]
        public override string ToString()
        {
            return string.Format("{0} x {1} @ {2}-bit color", this.displayMode.Width, this.displayMode.Height,
                                 ColorDepth);
        }

        #region Properties

        ///<summary>
        ///  Width of this video mode.
        ///</summary>
        [OgreVersion(1, 7, 2)]
        public int Width
        {
            get { return this.displayMode.Width; }
        }

        ///<summary>
        ///  Height of this video mode.
        ///</summary>
        [OgreVersion(1, 7, 2)]
        public int Height
        {
            get { return this.displayMode.Height; }
        }

        ///<summary>
        ///  Format of this video mode.
        ///</summary>
        [OgreVersion(1, 7, 2)]
        public D3D.Format Format
        {
            get { return this.displayMode.Format; }
        }

        ///<summary>
        ///  Refresh rate of this video mode.
        ///</summary>
        [OgreVersion(1, 7, 2)]
        public int RefreshRate
        {
            [OgreVersion(1, 7, 2)]
            get { return this.displayMode.RefreshRate; }

            [OgreVersion(1, 7, 2, "increaseRefreshRate")]
            set { this.displayMode.RefreshRate = value; }
        }

        ///<summary>
        ///  Color depth of this video mode.
        ///</summary>
        [OgreVersion(1, 7, 2)]
        public int ColorDepth
        {
            get
            {
                int colorDepth = 16;

                if (this.displayMode.Format == D3D.Format.X8R8G8B8 || this.displayMode.Format == D3D.Format.A8R8G8B8 ||
                    this.displayMode.Format == D3D.Format.R8G8B8)
                {
                    colorDepth = 32;
                }

                return colorDepth;
            }
        }

        ///<summary>
        ///  Gets the Direct3D.DisplayMode object associated with this video mode.
        ///</summary>
        [OgreVersion(1, 7, 2)]
        public D3D.DisplayMode DisplayMode
        {
            get { return this.displayMode; }
        }

        /// <summary>
        ///   Returns a string representation of this video mode.
        /// </summary>
        [OgreVersion(1, 7, 2)]
        public string Description
        {
            get { return ToString(); }
        }

        #endregion Properties
    };
}