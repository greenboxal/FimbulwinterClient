#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: TexCoordModifierControllerValue.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using Axiom.Graphics;
using Axiom.Math;

#endregion Namespace Declarations

namespace Axiom.Controllers.Canned
{
    /// <summary>
    ///   Summary description for TexCoordModifierControllerValue.
    /// </summary>
    public class TexCoordModifierControllerValue : IControllerValue<Real>
    {
        #region Member variables

        protected bool transU;
        protected bool transV;
        protected bool scaleU;
        protected bool scaleV;
        protected bool rotate;
        protected TextureUnitState texUnit;

        #endregion

        public TexCoordModifierControllerValue(TextureUnitState texUnit)
        {
            this.texUnit = texUnit;
        }

        public TexCoordModifierControllerValue(TextureUnitState texUnit, bool scrollU)
            : this(texUnit, scrollU, false)
        {
        }

        public TexCoordModifierControllerValue(TextureUnitState texUnit, bool scrollU, bool scrollV)
        {
            this.texUnit = texUnit;
            this.transU = scrollU;
            this.transV = scrollV;
        }

        public TexCoordModifierControllerValue(TextureUnitState texUnit, bool scrollU, bool scrollV, bool scaleU,
                                               bool scaleV,
                                               bool rotate)
        {
            this.texUnit = texUnit;
            this.transU = scrollU;
            this.transV = scrollV;
            this.scaleU = scaleU;
            this.scaleV = scaleV;
            this.rotate = rotate;
        }

        #region IControllerValue Members

        public Real Value
        {
            get
            {
                Matrix4 trans = this.texUnit.TextureMatrix;

                if (this.transU)
                {
                    return trans.m03;
                }
                else if (this.transV)
                {
                    return trans.m13;
                }
                else if (this.scaleU)
                {
                    return trans.m00;
                }
                else if (this.scaleV)
                {
                    return trans.m11;
                }

                // should never get here
                return 0.0f;
            }
            set
            {
                if (this.transU)
                {
                    this.texUnit.SetTextureScrollU(value);
                }

                if (this.transV)
                {
                    this.texUnit.SetTextureScrollV(value);
                }

                if (this.scaleU)
                {
                    if (value >= 0)
                    {
                        this.texUnit.SetTextureScaleU(1 + value);
                    }
                    else
                    {
                        this.texUnit.SetTextureScaleU(1/-value);
                    }
                }

                if (this.scaleV)
                {
                    if (value >= 0)
                    {
                        this.texUnit.SetTextureScaleV(1 + value);
                    }
                    else
                    {
                        this.texUnit.SetTextureScaleV(1/-value);
                    }
                }

                if (this.rotate)
                {
                    this.texUnit.SetTextureRotate(value*360);
                }
            }
        }

        #endregion
    }
}