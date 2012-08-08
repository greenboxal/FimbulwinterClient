#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: MouseEventArgs.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;

#endregion Namespace Declarations

namespace Axiom.Input
{
    ///<summary>
    ///  Events args for mouse input events.
    ///</summary>
    public class MouseEventArgs : InputEventArgs
    {
        #region Fields

        ///<summary>
        ///  X coordinate of the mouse.
        ///</summary>
        protected float x;

        ///<summary>
        ///  Y coordinate of the mouse.
        ///</summary>
        protected float y;

        ///<summary>
        ///  Z coordinate of the mouse.
        ///</summary>
        protected float z;

        ///<summary>
        ///  Relative X coordinate of the mouse.
        ///</summary>
        protected float relativeX;

        ///<summary>
        ///  Relative Y coordinate of the mouse.
        ///</summary>
        protected float relativeY;

        ///<summary>
        ///  Relative Z coordinate of the mouse.
        ///</summary>
        protected float relativeZ;

        ///<summary>
        ///  Mouse button pressed during this event.
        ///</summary>
        protected MouseButtons button;

        #endregion Fields

        #region Constructors

        ///<summary>
        ///  Constructor.
        ///</summary>
        ///<param name="button"> Mouse button pressed. </param>
        ///<param name="modifiers"> Any modifier keys that are down. </param>
        ///<param name="x"> Mouse X position. </param>
        ///<param name="y"> Mouse Y position. </param>
        ///<param name="z"> Mouse Z position. </param>
        public MouseEventArgs(MouseButtons button, ModifierKeys modifiers, float x, float y, float z)
            : this(button, modifiers, x, y, z, 0, 0, 0)
        {
        }

        ///<summary>
        ///  Constructor.
        ///</summary>
        ///<param name="button"> Mouse button pressed. </param>
        ///<param name="modifiers"> Any modifier keys that are down. </param>
        ///<param name="x"> Mouse X position. </param>
        ///<param name="y"> Mouse Y position. </param>
        ///<param name="z"> Mouse Z position. </param>
        ///<param name="relX"> Relative mouse X position. </param>
        ///<param name="relY"> Relative mouse Y position. </param>
        ///<param name="relZ"> Relative mouse Z position. </param>
        public MouseEventArgs(MouseButtons button, ModifierKeys modifiers, float x, float y, float z, float relX,
                              float relY,
                              float relZ)
            : base(modifiers)
        {
            this.button = button;
            this.x = x;
            this.y = y;
            this.z = z;
            this.relativeX = relX;
            this.relativeY = relY;
            this.relativeZ = relZ;
        }

        #endregion Constructors

        #region Properties

        ///<summary>
        ///  Mouse button pressed during this event.
        ///</summary>
        public MouseButtons Button
        {
            get { return this.button; }
        }

        ///<summary>
        ///  Mouse X coordinate.
        ///</summary>
        public float X
        {
            get { return this.x; }
        }

        ///<summary>
        ///  Mouse Y coordinate.
        ///</summary>
        public float Y
        {
            get { return this.y; }
        }

        ///<summary>
        ///  Mouse Z coordinate.
        ///</summary>
        public float Z
        {
            get { return this.z; }
        }

        ///<summary>
        ///  Relative mouse X coordinate.
        ///</summary>
        public float RelativeX
        {
            get { return this.relativeX; }
        }

        ///<summary>
        ///  Relative mouse Y coordinate.
        ///</summary>
        public float RelativeY
        {
            get { return this.relativeY; }
        }

        ///<summary>
        ///  Relative mouse Z coordinate.
        ///</summary>
        public float RelativeZ
        {
            get { return this.relativeZ; }
        }

        #endregion Properties
    }
}