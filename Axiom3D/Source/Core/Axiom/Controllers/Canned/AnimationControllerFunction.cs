#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: AnimationControllerFunction.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using Axiom.Math;

#endregion Namespace Declarations

namespace Axiom.Controllers.Canned
{
    /// <summary>
    ///   Predefined controller function for dealing with animation.
    /// </summary>
    public class AnimationControllerFunction : IControllerFunction<Real>
    {
        #region Fields

        /// <summary>
        ///   The amount of time in seconds it takes to loop through the whole animation sequence.
        /// </summary>
        protected Real sequenceTime;

        /// <summary>
        ///   The offset in seconds at which to start (default is start at 0).
        /// </summary>
        protected Real time;

        #endregion Fields

        #region Constructor

        /// <summary>
        ///   Constructor.
        /// </summary>
        /// <param name="sequenceTime"> The amount of time in seconds it takes to loop through the whole animation sequence. </param>
        public AnimationControllerFunction(Real sequenceTime)
            : this(sequenceTime, 0.0f)
        {
        }

        /// <summary>
        ///   Constructor.
        /// </summary>
        /// <param name="sequenceTime"> The amount of time in seconds it takes to loop through the whole animation sequence. </param>
        /// <param name="timeOffset"> The offset in seconds at which to start. </param>
        public AnimationControllerFunction(Real sequenceTime, Real timeOffset)
        {
            this.sequenceTime = sequenceTime;
            this.time = timeOffset;
        }

        #endregion

        #region ControllerFunction Members

        /// <summary>
        /// </summary>
        /// <param name="sourceValue"> </param>
        /// <returns> </returns>
        public Real Execute(Real sourceValue)
        {
            // assuming source if the time since the last update
            this.time += sourceValue;

            // wrap
            while (this.time >= this.sequenceTime)
            {
                this.time -= this.sequenceTime;
            }

            // return parametric
            return this.time/this.sequenceTime;
        }

        #endregion ControllerFunction Members
    }
}