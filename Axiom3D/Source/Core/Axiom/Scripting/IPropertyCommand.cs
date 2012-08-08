#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: IPropertyCommand.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

#endregion Namespace Declarations

namespace Axiom.Scripting
{
    /// <summary>
    ///   Specialization of the IPropertyCommand using object
    /// </summary>
    public interface IPropertyCommand : IPropertyCommand<object>
    {
    };

    /// <summary>
    ///   Provides an interface for setting object properties via a Command Pattern.
    /// </summary>
    /// <typeparam name="TObjectType"> Type of the object to operate on. </typeparam>
    public interface IPropertyCommand<TObjectType>
    {
        /// <summary>
        ///   Gets the value for this command from the target object.
        /// </summary>
        /// <param name="target"> </param>
        /// <returns> </returns>
        string Get(TObjectType target);

        /// <summary>
        ///   Sets the value for this command on the target object.
        /// </summary>
        /// <param name="target"> </param>
        /// <param name="val"> </param>
        void Set(TObjectType target, string val);
    };
}