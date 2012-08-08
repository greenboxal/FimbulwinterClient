#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: VariableGetAbstractNode.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

#endregion Namespace Declarations

namespace Axiom.Scripting.Compiler.AST
{
    /// <summary>
    ///   This abstract node represents a variable assignment
    /// </summary>
    internal class VariableGetAbstractNode : AbstractNode
    {
        public string Name;

        public VariableGetAbstractNode(AbstractNode parent)
            : base(parent)
        {
        }

        #region AbstractNode Implementation

        /// <see cref="AbstractNode.Clone" />
        public override AbstractNode Clone()
        {
            VariableGetAbstractNode node = new VariableGetAbstractNode(Parent);
            node.File = File;
            node.Line = Line;
            node.Name = this.Name;
            return node;
        }

        /// <see cref="AbstractNode.Value" />
        public override string Value
        {
            get { return this.Name; }
            set { this.Name = value; }
        }

        #endregion AbstractNode Implementation
    }
}