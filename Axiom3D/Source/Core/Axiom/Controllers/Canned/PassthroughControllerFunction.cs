#region SVN Version Information

// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <id value="$Id: PassthroughControllerFunction.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using Axiom.Math;

#endregion Namespace Declarations

namespace Axiom.Controllers.Canned
{
    public class PassthroughControllerFunction : BaseControllerFunction
    {
        public PassthroughControllerFunction()
            : base(false)
        {
        }

        public PassthroughControllerFunction(bool deltaInput)
            : base(deltaInput)
        {
        }

        public override Real Execute(Real source)
        {
            return AdjustInput(source);
        }
    }
}