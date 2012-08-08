#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: Structures.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;

#endregion Namespace Declarations

namespace Axiom.RenderSystems.OpenGL.ATI
{
    /// <summary>
    ///   Structure used to build rule paths.
    /// </summary>
    public struct TokenRule
    {
        public OperationType operation;
        public Symbol tokenID;
        public string symbol;
        public int errorID;

        public TokenRule(OperationType op)
        {
            this.operation = op;
            this.tokenID = 0;
            this.symbol = "";
            this.errorID = 0;
        }

        public TokenRule(OperationType op, Symbol tokenID)
        {
            this.operation = op;
            this.tokenID = tokenID;
            this.symbol = "";
            this.errorID = 0;
        }

        public TokenRule(OperationType op, Symbol tokenID, string symbol)
        {
            this.operation = op;
            this.tokenID = tokenID;
            this.symbol = symbol;
            this.errorID = 0;
        }
    }

    /// <summary>
    ///   Structure used to build Symbol Type library.
    /// </summary>
    public struct SymbolDef
    {
        /// <summary>
        ///   Token ID which is the index into the Token Type library.
        /// </summary>
        public Symbol ID;

        /// <summary>
        ///   Data used by pass 2 to build native instructions.
        /// </summary>
        public int pass2Data;

        /// <summary>
        ///   Context key to fit the Active Context.
        /// </summary>
        public uint contextKey;

        /// <summary>
        ///   New pattern to set for Active Context bits.
        /// </summary>
        public uint contextPatternSet;

        /// <summary>
        ///   Contexts bits to clear Active Context bits.
        /// </summary>
        public uint contextPatternClear;

        /// <summary>
        ///   Index into text table for default name : set at runtime.
        /// </summary>
        public int defTextID;

        /// <summary>
        ///   Index into Rule database for non-terminal toke rulepath.
        ///   Note: If RuleID is zero the token is terminal.
        /// </summary>
        public int ruleID;

        public SymbolDef(Symbol symbol, int glEnum, ContextKeyPattern ckp)
        {
            this.ID = symbol;
            this.pass2Data = glEnum;
            this.contextKey = (uint) ckp;
            this.contextPatternSet = 0;
            this.contextPatternClear = 0;
            this.defTextID = 0;
            this.ruleID = 0;
        }

        public SymbolDef(Symbol symbol, int glEnum, ContextKeyPattern ckp, uint cps)
        {
            this.ID = symbol;
            this.pass2Data = glEnum;
            this.contextKey = (uint) ckp;
            this.contextPatternSet = cps;
            this.contextPatternClear = 0;
            this.defTextID = 0;
            this.ruleID = 0;
        }

        public SymbolDef(Symbol symbol, int glEnum, ContextKeyPattern ckp, ContextKeyPattern cps)
        {
            this.ID = symbol;
            this.pass2Data = glEnum;
            this.contextKey = (uint) ckp;
            this.contextPatternSet = (uint) cps;
            this.contextPatternClear = 0;
            this.defTextID = 0;
            this.ruleID = 0;
        }
    }

    /// <summary>
    ///   Structure for Token instructions.
    /// </summary>
    public struct TokenInstruction
    {
        /// <summary>
        ///   Non-Terminal Token Rule ID that generated Token.
        /// </summary>
        public Symbol NTTRuleID;

        /// <summary>
        ///   Token ID.
        /// </summary>
        public Symbol ID;

        /// <summary>
        ///   Line number in source code where Token was found
        /// </summary>
        public int line;

        /// <summary>
        ///   Character position in source where Token was found
        /// </summary>
        public int pos;

        public TokenInstruction(Symbol symbol, Symbol ID)
        {
            this.NTTRuleID = symbol;
            this.ID = ID;
            this.line = 0;
            this.pos = 0;
        }
    }

    public struct TokenInstType
    {
        public string Name;
        public int ID;
    }

    public struct RegisterUsage
    {
        public bool Phase1Write;
        public bool Phase2Write;
    }

    /// <summary>
    ///   Structure used to keep track of arguments and instruction parameters.
    /// </summary>
    internal struct OpParam
    {
        public int Arg; // type of argument
        public bool Filled; // has it been filled yet
        public uint MaskRep; // Mask/Replicator flags
        public int Mod; // argument modifier
    }

    internal struct RegModOffset
    {
        public int MacroOffset;
        public int RegisterBase;
        public int OpParamsIndex;

        public RegModOffset(int offset, Symbol regBase, int index)
        {
            this.MacroOffset = offset;
            this.RegisterBase = (int) regBase;
            this.OpParamsIndex = index;
        }
    }

    internal struct MacroRegModify
    {
        public TokenInstruction[] Macro;
        public int MacroSize;
        public RegModOffset[] RegMods;
        public int RegModSize;

        public MacroRegModify(TokenInstruction[] tokens, RegModOffset[] offsets)
        {
            this.Macro = tokens;
            this.MacroSize = tokens.Length;
            this.RegMods = offsets;
            this.RegModSize = offsets.Length;
        }
    }
}