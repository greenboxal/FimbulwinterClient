#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: Enums.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using Axiom.Scripting;

#endregion Namespace Declarations

namespace Axiom.Overlays
{
    ///<summary>
    ///  Describes how the position / size of an element is to be treated.
    ///</summary>
    public enum MetricsMode
    {
        /// <summary>
        ///   'left', 'top', 'height' and 'width' are parametrics from 0.0 to 1.0
        /// </summary>
        [ScriptEnum("relative")] Relative,

        /// <summary>
        ///   Positions &amp; sizes are in absolute pixels.
        /// </summary>
        [ScriptEnum("pixels")] Pixels,

        /// <summary>
        ///   Positions &amp; sizes are in virtual pixels
        /// </summary>
        [ScriptEnum("relative_aspect_adjusted")] Relative_Aspect_Adjusted
    }

    ///<summary>
    ///  Describes where '0' is in relation to the parent in the horizontal dimension.  Affects how 'left' is interpreted.
    ///</summary>
    public enum HorizontalAlignment
    {
        /// <summary>
        /// </summary>
        [ScriptEnum("left")] Left,

        /// <summary>
        /// </summary>
        [ScriptEnum("center")] Center,

        /// <summary>
        /// </summary>
        [ScriptEnum("right")] Right
    }

    ///<summary>
    ///  Describes where '0' is in relation to the parent in the vertical dimension.  Affects how 'top' is interpreted.
    ///</summary>
    public enum VerticalAlignment
    {
        /// <summary>
        /// </summary>
        [ScriptEnum("top")] Top,

        /// <summary>
        /// </summary>
        [ScriptEnum("center")] Center,

        ///<summary>
        ///</summary>
        [ScriptEnum("bottom")] Bottom
    }
}