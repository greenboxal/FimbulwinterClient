#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: CompositionPassStencilTranslator.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using Axiom.Graphics;
using Axiom.Scripting.Compiler.AST;

#endregion Namespace Declarations

namespace Axiom.Scripting.Compiler
{
    public partial class ScriptCompiler
    {
        public class CompositionPassStencilTranslator : Translator
        {
            protected CompositionPass _Pass;

            public CompositionPassStencilTranslator()
            {
                this._Pass = null;
            }

            #region Translator Implementation

            /// <see cref="Translator.CheckFor" />
            public override bool CheckFor(Keywords nodeId, Keywords parentId)
            {
                return nodeId == Keywords.ID_STENCIL && parentId == Keywords.ID_PASS;
            }

            /// <see cref="Translator.Translate" />
            public override void Translate(ScriptCompiler compiler, AbstractNode node)
            {
                ObjectAbstractNode obj = (ObjectAbstractNode) node;

                this._Pass = (CompositionPass) obj.Parent.Context;

                // Should be no parameters, just children
                if (obj.Values.Count != 0)
                {
                    compiler.AddError(CompileErrorCode.UnexpectedToken, obj.File, obj.Line);
                }

                foreach (AbstractNode i in obj.Children)
                {
                    if (i is ObjectAbstractNode)
                    {
                        processNode(compiler, i);
                    }
                    else if (i is PropertyAbstractNode)
                    {
                        PropertyAbstractNode prop = (PropertyAbstractNode) i;
                        switch ((Keywords) prop.Id)
                        {
                                #region ID_CHECK

                            case Keywords.ID_CHECK:
                                {
                                    if (prop.Values.Count == 0)
                                    {
                                        compiler.AddError(CompileErrorCode.StringExpected, prop.File, prop.Line);
                                        return;
                                    }

                                    bool val = false;
                                    if (getBoolean(prop.Values[0], out val))
                                    {
                                        this._Pass.StencilCheck = val;
                                    }
                                    else
                                    {
                                        compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line);
                                    }
                                }
                                break;

                                #endregion ID_CHECK

                                #region ID_COMP_FUNC

                            case Keywords.ID_COMP_FUNC:
                                {
                                    if (prop.Values.Count == 0)
                                    {
                                        compiler.AddError(CompileErrorCode.StringExpected, prop.File, prop.Line);
                                        return;
                                    }

                                    CompareFunction func;
                                    if (getEnumeration(prop.Values[0], compiler, out func))
                                    {
                                        this._Pass.StencilFunc = func;
                                    }
                                    else
                                    {
                                        compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line);
                                    }
                                }
                                break;

                                #endregion ID_COMP_FUNC

                                #region ID_REF_VALUE

                            case Keywords.ID_REF_VALUE:
                                {
                                    if (prop.Values.Count == 0)
                                    {
                                        compiler.AddError(CompileErrorCode.NumberExpected, prop.File, prop.Line);
                                        return;
                                    }

                                    int val;
                                    if (getInt(prop.Values[0], out val))
                                    {
                                        this._Pass.StencilRefValue = val;
                                    }
                                    else
                                    {
                                        compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line);
                                    }
                                }
                                break;

                                #endregion ID_REF_VALUE

                                #region ID_MASK

                            case Keywords.ID_MASK:
                                {
                                    if (prop.Values.Count == 0)
                                    {
                                        compiler.AddError(CompileErrorCode.NumberExpected, prop.File, prop.Line);
                                        return;
                                    }
                                    int val;
                                    if (getInt(prop.Values[0], out val))
                                    {
                                        this._Pass.StencilMask = val;
                                    }
                                    else
                                    {
                                        compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line);
                                    }
                                }
                                break;

                                #endregion ID_MASK

                                #region ID_FAIL_OP

                            case Keywords.ID_FAIL_OP:
                                {
                                    if (prop.Values.Count == 0)
                                    {
                                        compiler.AddError(CompileErrorCode.StringExpected, prop.File, prop.Line);
                                        return;
                                    }

                                    StencilOperation val;
                                    if (getEnumeration(prop.Values[0], compiler, out val))
                                    {
                                        this._Pass.StencilFailOp = val;
                                    }
                                    else
                                    {
                                        compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line);
                                    }
                                }
                                break;

                                #endregion ID_FAIL_OP

                                #region ID_DEPTH_FAIL_OP

                            case Keywords.ID_DEPTH_FAIL_OP:
                                {
                                    if (prop.Values.Count == 0)
                                    {
                                        compiler.AddError(CompileErrorCode.StringExpected, prop.File, prop.Line);
                                        return;
                                    }

                                    StencilOperation val;
                                    if (getEnumeration(prop.Values[0], compiler, out val))
                                    {
                                        this._Pass.StencilDepthFailOp = val;
                                    }
                                    else
                                    {
                                        compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line);
                                    }
                                }
                                break;

                                #endregion ID_DEPTH_FAIL_OP

                                #region ID_PASS_OP

                            case Keywords.ID_PASS_OP:
                                {
                                    if (prop.Values.Count == 0)
                                    {
                                        compiler.AddError(CompileErrorCode.StringExpected, prop.File, prop.Line);
                                        return;
                                    }

                                    StencilOperation val;
                                    if (getEnumeration(prop.Values[0], compiler, out val))
                                    {
                                        this._Pass.StencilPassOp = val;
                                    }
                                    else
                                    {
                                        compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line);
                                    }
                                }
                                break;

                                #endregion ID_PASS_OP

                                #region ID_TWO_SIDED

                            case Keywords.ID_TWO_SIDED:
                                {
                                    if (prop.Values.Count == 0)
                                    {
                                        compiler.AddError(CompileErrorCode.StringExpected, prop.File, prop.Line);
                                        return;
                                    }

                                    bool val;
                                    if (getBoolean(prop.Values[0], out val))
                                    {
                                        this._Pass.StencilTwoSidedOperation = val;
                                    }
                                    else
                                    {
                                        compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line);
                                    }
                                }
                                break;

                                #endregion ID_TWO_SIDED

                            default:
                                compiler.AddError(CompileErrorCode.UnexpectedToken, prop.File, prop.Line,
                                                  "token \"" + prop.Name + "\" is not recognized");
                                break;
                        }
                    }
                }
            }

            #endregion Translator Implementation
        }
    }
}