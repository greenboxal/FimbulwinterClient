#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: IControllerValue.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;

#endregion Namespace Declarations

namespace Axiom.Controllers
{
    ///<summary>
    ///  Classes that will be controlled by any type of Controller should implement
    ///  this interface to define how the controller will modifiy it's local data.
    ///</summary>
    public interface IControllerValue<T>
    {
        T Value { get; set; }
    }
}