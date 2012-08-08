#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: PropertyAbstractNode.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections.Generic;

#endregion Namespace Declarations

namespace Axiom.Scripting.Compiler.AST
{
    /// <summary>
    ///   This abstract node represents a script property
    /// </summary>
    public class PropertyAbstractNode : AbstractNode
    {
        #region Fields and Properties

        public String Name;

        public uint Id;

        public IList<AbstractNode> Values = new List<AbstractNode>();

        #endregion Fields and Properties

        public PropertyAbstractNode(AbstractNode parent)
            : base(parent)
        {
            this.Id = 0;
        }

        #region AbstractNode Implementation

        /// <see cref="AbstractNode.Clone" />
        public override AbstractNode Clone()
        {
            PropertyAbstractNode node = new PropertyAbstractNode(Parent);
            node.File = File;
            node.Line = Line;
            node.Name = this.Name;
            node.Id = this.Id;
            foreach (AbstractNode an in this.Values)
            {
                AbstractNode newNode = (an.Clone());
                newNode.Parent = node;
                node.Values.Add(newNode);
            }
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