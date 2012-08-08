#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: ObjectAbstractNode.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections.Generic;

#endregion Namespace Declarations

namespace Axiom.Scripting.Compiler.AST
{
    /// <summary>
    ///   This specific abstract node represents a script object
    /// </summary>
    public class ObjectAbstractNode : AbstractNode
    {
        #region Fields and Properties

        public string Name;

        public string Cls;

        public IList<string> Bases
        {
            get { return this._bases; }
        }

        private readonly List<string> _bases = new List<string>();

        public uint Id;

        public bool IsAbstract;

        public IList<AbstractNode> Children = new List<AbstractNode>();

        public IList<AbstractNode> Values = new List<AbstractNode>();

        /// <summary>
        ///   For use when processing object inheritance and overriding
        /// </summary>
        public IList<AbstractNode> Overrides
        {
            get { return this._overrides; }
        }

        private readonly List<AbstractNode> _overrides = new List<AbstractNode>();

        private Dictionary<String, String> _environment = new Dictionary<string, string>();

        public Dictionary<String, String> Variables
        {
            get { return this._environment; }
        }

        #endregion Fields and Properties

        public ObjectAbstractNode(AbstractNode parent)
            : base(parent)
        {
            this.IsAbstract = false;
        }

        #region Methods

        public void AddVariable(string name)
        {
            this._environment.Add(name, "");
        }

        public void SetVariable(string name, string value)
        {
            this._environment[name] = value;
        }

        public KeyValuePair<bool, string> GetVariable(string inName)
        {
            if (this._environment.ContainsKey(inName))
            {
                return new KeyValuePair<bool, string>(true, this._environment[inName]);
            }

            ObjectAbstractNode parentNode = (ObjectAbstractNode) Parent;
            while (parentNode != null)
            {
                if (parentNode._environment.ContainsKey(inName))
                {
                    return new KeyValuePair<bool, string>(true, parentNode._environment[inName]);
                }

                parentNode = (ObjectAbstractNode) parentNode.Parent;
            }

            return new KeyValuePair<bool, string>(false, string.Empty);
        }

        #endregion Methods

        #region AbstractNode Implementation

        /// <see cref="AbstractNode.Clone" />
        public override AbstractNode Clone()
        {
            ObjectAbstractNode node = new ObjectAbstractNode(Parent);
            node.File = File;
            node.Line = Line;
            node.Name = this.Name;
            node.Cls = this.Cls;
            node.Id = this.Id;
            node.IsAbstract = this.IsAbstract;
            foreach (AbstractNode an in this.Children)
            {
                AbstractNode newNode = (an.Clone());
                newNode.Parent = node;
                node.Children.Add(newNode);
            }
            foreach (AbstractNode an in this.Values)
            {
                AbstractNode newNode = (an.Clone());
                newNode.Parent = node;
                node.Values.Add(newNode);
            }
            node._environment = new Dictionary<string, string>(this._environment);
            return node;
        }

        /// <see cref="AbstractNode.Value" />
        public override string Value
        {
            get { return this.Cls; }
            set { this.Cls = value; }
        }

        #endregion AbstractNode Implementation
    }
}