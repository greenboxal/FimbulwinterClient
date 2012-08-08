#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: BaseControllerFunction.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using Axiom.Math;

#endregion Namespace Declarations

namespace Axiom.Controllers
{
    ///<summary>
    ///  Subclasses of this class are responsible for performing a function on an input value for a Controller.
    ///</summary>
    ///<remarks>
    ///  This abstract class provides the interface that needs to be supported for a custom function which
    ///  can be 'plugged in' to a Controller instance, which controls some object value based on an input value.
    ///  For example, the WaveControllerFunction class provided by Ogre allows you to use various waveforms to
    ///  translate an input value to an output value.
    ///  <p />
    ///  This base class implements IControllerFunction, but leaves the implementation up to the subclasses.
    ///</remarks>
    public abstract class BaseControllerFunction : IControllerFunction<Real>
    {
        #region Member variables

        ///<summary>
        ///  If true, function will add input values together and wrap at 1.0 before evaluating.
        ///</summary>
        protected bool useDeltaInput;

        ///<summary>
        ///  Value to be added during evaluation.
        ///</summary>
        protected Real deltaCount;

        #endregion

        #region Constructors

        public BaseControllerFunction(bool useDeltaInput)
        {
            this.useDeltaInput = useDeltaInput;
            //deltaCount = 0; //[FXCop Optimization : Do not initialize unnecessarily], Defaults to 0, left here for clarity
        }

        #endregion

        #region Methods

        ///<summary>
        ///  Adjusts the input value by a delta.
        ///</summary>
        ///<param name="input"> </param>
        ///<returns> </returns>
        protected virtual Real AdjustInput(Real input)
        {
            if (this.useDeltaInput)
            {
                // wrap the value if it went past 1
                this.deltaCount = (this.deltaCount + input)%1.0f;

                // return the adjusted input value
                return this.deltaCount;
            }
            else
            {
                // return the input value as is
                return input;
            }
        }

        #endregion

        #region IControllerFunction methods

        public abstract Real Execute(Real sourceValue);

        #endregion
    }
}