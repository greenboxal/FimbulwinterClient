#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: IControllerFunction.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;

#endregion Namespace Declarations

namespace Axiom.Controllers
{
    ///<summary>
    ///  Interface describing the required methods of a Controller Function.
    ///</summary>
    public interface IControllerFunction<T>
    {
        ///<summary>
        ///  Called by a controller every frame to have this function run and return on the supplied
        ///  source value and return the result.
        ///</summary>
        ///<param name="sourceValue"> </param>
        T Execute(T sourceValue);
    }
}