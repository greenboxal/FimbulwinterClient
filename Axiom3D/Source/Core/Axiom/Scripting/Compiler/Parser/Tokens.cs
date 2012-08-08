#region SVN Version Information

// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <id value="$Id: Tokens.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections.Generic;
using System.Text;

#endregion Namespace Declarations

namespace Axiom.Scripting.Compiler.Parser
{
    /// <summary>
    ///   These codes represent token IDs which are numerical translations of
    ///   specific lexemes. Specific compilers using the lexer can register their
    ///   own token IDs which are given precedence over these built-in ones
    /// </summary>
    public enum Tokens
    {
        LeftBrace = 0, // {
        RightBrace, // }
        Colon, // :
        Variable, // $...
        Word, // *
        Quote, // "*"
        Newline, // \n
        Unknown,
        End
    }
}