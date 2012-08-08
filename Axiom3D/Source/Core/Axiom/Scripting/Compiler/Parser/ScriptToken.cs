#region SVN Version Information

// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <id value="$Id: ScriptToken.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections.Generic;
using System.Text;

#endregion Namespace Declarations

namespace Axiom.Scripting.Compiler.Parser
{
    public struct ScriptToken
    {
        /// This is the lexeme for this token
        public String lexeme, file;

        /// This is the id associated with the lexeme, which comes from a lexeme-token id mapping
        public Tokens type;

        /// This holds the line number of the input stream where the token was found.
        public uint line;
    }
}