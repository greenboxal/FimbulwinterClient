#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: TextureFrameControllerValue.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using Axiom.Graphics;
using Axiom.Math;

#endregion Namespace Declarations

namespace Axiom.Controllers.Canned
{
    /// <summary>
    ///   Predefined controller value for getting/setting the frame number of a texture unit.
    /// </summary>
    public class TextureFrameControllerValue : IControllerValue<Real>
    {
        #region Fields

        /// <summary>
        ///   Reference to the texture unit state to target for the animation.
        /// </summary>
        protected TextureUnitState texUnit;

        #endregion Fields

        #region Constructor

        /// <summary>
        ///   Constructor.
        /// </summary>
        /// <param name="textureUnit"> Reference to the texture unit state to target for the animation. </param>
        public TextureFrameControllerValue(TextureUnitState textureUnit)
        {
            this.texUnit = textureUnit;
        }

        #endregion Constructor

        #region IControllerValue Members

        /// <summary>
        ///   Gets/Sets the frame of animation for a texture unit.
        /// </summary>
        /// <remarks>
        ///   Value is a parametric value in the range [0,1].
        /// </remarks>
        public Real Value
        {
            get { return this.texUnit.CurrentFrame/this.texUnit.NumFrames; }
            set { this.texUnit.CurrentFrame = (int) (value*this.texUnit.NumFrames); }
        }

        #endregion IControllerValue Members
    }
}