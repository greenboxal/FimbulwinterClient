#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: FloatGpuParamControllerValue.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using Axiom.Graphics;
using Axiom.Math;

#endregion Namespace Declarations

namespace Axiom.Controllers.Canned
{
    /// <summary>
    ///   Predefined controller value for setting a single floating-
    ///   point value in a constant paramter of a vertex or fragment program.
    /// </summary>
    /// <remarks>
    ///   Any value is accepted, it is propagated into the 'x'
    ///   component of the constant register identified by the index. If you
    ///   need to use named parameters, retrieve the index from the param
    ///   object before setting this controller up.
    ///   <p />
    ///   Note: Retrieving a value from the program parameters is not currently 
    ///   supported, therefore do not use this controller value as a source,
    ///   only as a target.
    /// </remarks>
    public class FloatGpuParamControllerValue : IControllerValue<Real>
    {
        #region Fields

        /// <summary>
        ///   Gpu parameters to access.
        /// </summary>
        protected GpuProgramParameters parms;

        /// <summary>
        ///   The constant register index of the parameter to set.
        /// </summary>
        protected int index;

        /// <summary>
        ///   Member level Vector to use for returning.
        /// </summary>
        protected Vector4 vec4 = new Vector4(0, 0, 0, 0);

        #endregion Fields

        #region Constructor

        /// <summary>
        ///   Constructor.
        /// </summary>
        /// <param name="parms"> Params to set. </param>
        /// <param name="index"> Index of the parameter to set. </param>
        public FloatGpuParamControllerValue(GpuProgramParameters parms, int index)
        {
            this.parms = parms;
            this.index = index;
        }

        #endregion Constructor

        #region IControllerValue Members

        /// <summary>
        ///   Gets or Sets the value of the GPU parameter
        /// </summary>
        public Real Value
        {
            get { return this.parms.GetFloatConstant(this.index); }
            set
            {
                // set the x component, since this is a single value only
                this.vec4.x = value;

                // send the vector along to the gpu program params
                this.parms.SetConstant(this.index, this.vec4);
            }
        }

        #endregion
    }
}