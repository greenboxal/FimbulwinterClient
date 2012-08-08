#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: Factories.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using Axiom.Core;

#endregion Namepsace Declarations

namespace Axiom.Overlays.Elements
{
    /// <summary>
    ///   Summary description for BorderPanelFactory.
    /// </summary>
    public class BorderPanelFactory : IOverlayElementFactory
    {
        #region IOverlayElementFactory Members

        public OverlayElement Create(string name)
        {
            return new BorderPanel(name);
        }

        public string Type
        {
            get { return "BorderPanel"; }
        }

        #endregion
    }

    /// <summary>
    ///   Summary description for PanelFactory.
    /// </summary>
    public class PanelFactory : IOverlayElementFactory
    {
        #region IOverlayElementFactory Members

        public OverlayElement Create(string name)
        {
            return new Panel(name);
        }

        public string Type
        {
            get { return "Panel"; }
        }

        #endregion
    }

    /// <summary>
    ///   Summary description for TextAreaFactory.
    /// </summary>
    public class TextAreaFactory : IOverlayElementFactory
    {
        #region IOverlayElementFactory Members

        public OverlayElement Create(string name)
        {
            return new TextArea(name);
        }

        public string Type
        {
            get { return "TextArea"; }
        }

        #endregion
    }
}