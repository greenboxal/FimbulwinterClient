#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: ParserCommandAttribute.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Reflection;
using System.Text;

#endregion Namespace Declarations

namespace Axiom.Scripting
{
    ///<summary>
    ///  Custom attribute to mark methods as handling the parsing for a material script attribute.
    ///</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class ParserCommandAttribute : Attribute
    {
        private readonly string attributeName;
        private readonly string parserType;

        public ParserCommandAttribute(string name, string parserType)
        {
            this.attributeName = name;
            this.parserType = parserType;
        }

        public string Name
        {
            get { return this.attributeName; }
        }

        public string ParserType
        {
            get { return this.parserType; }
        }
    }
}