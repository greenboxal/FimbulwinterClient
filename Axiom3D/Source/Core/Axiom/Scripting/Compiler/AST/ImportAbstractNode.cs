#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: ImportAbstractNode.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;

#endregion Namespace Declarations

namespace Axiom.Scripting.Compiler.AST
{
    /// <summary>
    ///   This abstract node represents an import statement
    /// </summary>
    internal class ImportAbstractNode : AbstractNode
    {
        public String Source;

        public string Target;

        public ImportAbstractNode()
            : base(null)
        {
        }

        #region AbstractNode Implementation

        /// <see cref="AbstractNode.Clone" />
        public override AbstractNode Clone()
        {
            ImportAbstractNode node = new ImportAbstractNode();
            node.File = File;
            node.Line = Line;
            node.Target = this.Target;
            node.Source = this.Source;
            return node;
        }

        /// <see cref="AbstractNode.Value" />
        public override string Value
        {
            get { return this.Target; }
            set { this.Target = value; }
        }

        #endregion AbstractNode Implementation
    }
}