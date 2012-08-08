#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: AbstractNode.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;

#endregion Namespace Declarations

namespace Axiom.Scripting.Compiler.AST
{
    /// <summary>
    ///   the types of the possible abstract nodes
    /// </summary>
    public enum AbstractNodeType
    {
        Unknown,
        Atom,
        Object,
        Property,
        Import,
        VariableSet,
        VariableGet
    }

    /// <summary>
    ///   base node type for the AST
    /// </summary>
    public abstract class AbstractNode : ICloneable
    {
        public string File;

        public uint Line;

        public AbstractNode Parent;

        /// <summary>
        ///   An holder for translation context data
        /// </summary>
        public object Context;

        /// <summary>
        ///   Constructor
        /// </summary>
        /// <param name="parent"> the parent AbstractNode in the tree </param>
        protected AbstractNode(AbstractNode parent)
        {
            this.Parent = parent;
            this.Line = 0;
        }

        /// <summary>
        ///   Returns a string value depending on the type of the AbstractNode.
        /// </summary>
        public abstract string Value { get; set; }

        #region ICloneable Implementation

        object ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        ///   Returns a new AbstractNode which is a replica of this one
        /// </summary>
        /// <returns> a new AbstractNode </returns>
        public abstract AbstractNode Clone();

        #endregion ICloneable Implementation

        #region System.Object Implementation

        public override bool Equals(object obj)
        {
            return GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return this.File.GetHashCode() | this.Line.GetHashCode();
        }

        #endregion System.Object Implementation
    }
}