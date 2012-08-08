#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: MultiplyControllerFunction.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using Axiom.Math;

#endregion Namespace Declarations

namespace Axiom.Controllers.Canned
{
    /// <summary>
    ///   Summary description for MultiplyControllerValue.
    /// </summary>
    public class MultipyControllerFunction : BaseControllerFunction
    {
        private readonly Real rate = 10.0f;

        public MultipyControllerFunction(Real rate)
            : base(false)
        {
            this.rate = rate;
        }

        public MultipyControllerFunction(Real rate, bool useDelta)
            : base(useDelta)
        {
            this.rate = rate;
        }

        public override Real Execute(Real sourceValue)
        {
            return AdjustInput(sourceValue*this.rate);
        }
    }
}