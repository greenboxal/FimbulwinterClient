#region SVN Version Information

// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <id value="$Id: CompileError.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections.Generic;
using System.Text;

#endregion Namespace Declarations

namespace Axiom.Scripting.Compiler
{
    public partial class ScriptCompiler
    {
        /// <summary>
        /// </summary>
        public enum CompileErrorCode
        {
            [ScriptEnum("Unknown error")] UnknownError = 0,

            [ScriptEnum("String expected")] StringExpected,

            [ScriptEnum("Number expected")] NumberExpected,

            [ScriptEnum("Fewer parameters expected")] FewerParametersExpected,

            [ScriptEnum("Variable expected")] VariableExpected,

            [ScriptEnum("Undefined variable")] UndefinedVariable,

            [ScriptEnum("Object name expected")] ObjectNameExpected,

            [ScriptEnum("Object allocation error")] ObjectAllocationError,

            [ScriptEnum("Invalid parameters")] InvalidParameters,

            [ScriptEnum("Duplicate override")] DuplicateOverride,

            [ScriptEnum("Unexpected token")] UnexpectedToken,

            [ScriptEnum("Object base not found")] ObjectBaseNotFound,

            [ScriptEnum("Unsupported by RenderSystem")] UnsupportedByRenderSystem,

            [ScriptEnum("Reference to a non existing object")] ReferenceToaNonExistingObject
        }

        public struct CompileError
        {
            public CompileError(CompileErrorCode code, string file, uint line, string msg)
                : this()
            {
                Code = code;
                File = file;
                Line = line;
                Message = msg;
            }

            public string File { get; private set; }
            public string Message { get; private set; }
            public uint Line { get; private set; }
            public CompileErrorCode Code { get; private set; }
        }
    }
}