#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: CommandAttribute.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;

#endregion Namespace Declarations

namespace Axiom.Scripting
{
    /// <summary>
    ///   Summary description for CommandAttribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class CommandAttribute : Attribute
    {
        #region Fields

        /// <summary>
        ///   Name of the command the target class will be registered to handle.
        /// </summary>
        private readonly string name;

        /// <summary>
        ///   Description of what this command does.
        /// </summary>
        private readonly string description;

        /// <summary>
        ///   Target type this class is meant to handle commands for.
        /// </summary>
        private readonly Type target;

        #endregion Fields

        #region Constructors

        /// <summary>
        ///   Constructor.
        /// </summary>
        /// <param name="name"> </param>
        /// <param name="description"> </param>
        /// <param name="target"> </param>
        public CommandAttribute(string name, string description, Type target)
        {
            this.name = name;
            this.description = description;
            this.target = target;
        }

        /// <summary>
        ///   Constructor.
        /// </summary>
        /// <param name="name"> </param>
        /// <param name="description"> </param>
        public CommandAttribute(string name, string description)
        {
            this.name = name;
            this.description = description;
        }

        /// <summary>
        ///   Constructor.
        /// </summary>
        /// <param name="name"> </param>
        public CommandAttribute(string name)
        {
            this.name = name;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        ///   Name of this command.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        ///   Optional description of what this command does.
        /// </summary>
        public string Description
        {
            get { return this.description; }
        }

        /// <summary>
        ///   Optional target to specify what object type this command affects.
        /// </summary>
        public Type Target
        {
            get { return this.target; }
        }

        #endregion Properties
    }
}