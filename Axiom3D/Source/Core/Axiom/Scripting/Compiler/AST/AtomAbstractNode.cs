#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: AtomAbstractNode.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Globalization;

#endregion Namespace Declarations

namespace Axiom.Scripting.Compiler.AST
{
    /// <summary>
    ///   This is an abstract node which cannot be broken down further
    /// </summary>
    public class AtomAbstractNode : AbstractNode
    {
        #region Fields and Properties

        private readonly CultureInfo _culture = new CultureInfo("en-US");

        private NumberStyles _parseStyle = NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite |
                                           NumberStyles.AllowTrailingWhite | NumberStyles.AllowDecimalPoint;


        private bool _parsed;
        private string _value;

        public uint Id;

        private bool _isNumber;

        public bool IsNumber
        {
            get
            {
                if (!this._parsed)
                {
                    _parse();
                }
                return this._isNumber;
            }
        }

        private float _number;

        public float Number
        {
            get
            {
                if (!this._parsed)
                {
                    _parse();
                }
                return this._number;
            }
        }

        #endregion Fields and Properties

        public AtomAbstractNode(AbstractNode parent)
            : base(parent)
        {
        }

        private void _parse()
        {
            this._isNumber = float.TryParse(this._value, this._parseStyle, this._culture, out this._number);
            this._parsed = true;
        }

        #region AbstractNode Implementation

        /// <see cref="AbstractNode.Clone" />
        public override AbstractNode Clone()
        {
            AtomAbstractNode node = new AtomAbstractNode(Parent);
            node.File = File;
            node.Line = Line;
            node.Id = this.Id;
            node._value = Value;
            return node;
        }

        /// <see cref="AbstractNode.Value" />
        public override string Value
        {
            get { return this._value; }

            set { this._value = value; }
        }

        #endregion AbstractNode Implementation
    }
}