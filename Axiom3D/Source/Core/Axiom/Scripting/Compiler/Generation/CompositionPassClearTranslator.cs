#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: CompositionPassClearTranslator.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using Axiom.Core;
using Axiom.Graphics;
using Axiom.Math;
using Axiom.Scripting.Compiler.AST;

#endregion Namespace Declarations

namespace Axiom.Scripting.Compiler
{
    public partial class ScriptCompiler
    {
        public class CompositionPassClearTranslator : Translator
        {
            protected CompositionPass _Pass;

            #region Translator Implementation

            /// <see cref="Translator.CheckFor" />
            public override bool CheckFor(Keywords nodeId, Keywords parentId)
            {
                return nodeId == Keywords.ID_CLEAR && parentId == Keywords.ID_PASS;
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
                                #region ID_BUFFERS

                            case Keywords.ID_BUFFERS:
                                {
                                    FrameBufferType buffers = 0;
                                    foreach (AbstractNode k in prop.Values)
                                    {
                                        if (k is AtomAbstractNode)
                                        {
                                            switch ((Keywords) ((AtomAbstractNode) k).Id)
                                            {
                                                case Keywords.ID_COLOUR:
                                                    buffers |= FrameBufferType.Color;
                                                    break;

                                                case Keywords.ID_DEPTH:
                                                    buffers |= FrameBufferType.Depth;
                                                    break;

                                                case Keywords.ID_STENCIL:
                                                    buffers |= FrameBufferType.Stencil;
                                                    break;

                                                default:
                                                    compiler.AddError(CompileErrorCode.InvalidParameters, prop.File,
                                                                      prop.Line);
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line);
                                        }
                                    }
                                    this._Pass.ClearBuffers = buffers;
                                }
                                break;

                                #endregion ID_BUFFERS

                                #region ID_COLOUR_VALUE

                            case Keywords.ID_COLOUR_VALUE:
                                {
                                    if (prop.Values.Count == 0)
                                    {
                                        compiler.AddError(CompileErrorCode.NumberExpected, prop.File, prop.Line);
                                        return;
                                    }

                                    ColorEx val = ColorEx.White;
                                    if (getColor(prop.Values, 0, out val))
                                    {
                                        this._Pass.ClearColor = val;
                                    }
                                    else
                                    {
                                        compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line);
                                    }
                                }
                                break;

                                #endregion ID_COLOUR_VALUE

                                #region ID_DEPTH_VALUE

                            case Keywords.ID_DEPTH_VALUE:
                                {
                                    if (prop.Values.Count == 0)
                                    {
                                        compiler.AddError(CompileErrorCode.NumberExpected, prop.File, prop.Line);
                                        return;
                                    }
                                    Real val = 0;
                                    if (getReal(prop.Values[0], out val))
                                    {
                                        this._Pass.ClearDepth = val;
                                    }
                                    else
                                    {
                                        compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line);
                                    }
                                }
                                break;

                                #endregion ID_DEPTH_VALUE

                                #region ID_STENCIL_VALUE

                            case Keywords.ID_STENCIL_VALUE:
                                {
                                    if (prop.Values.Count == 0)
                                    {
                                        compiler.AddError(CompileErrorCode.NumberExpected, prop.File, prop.Line);
                                        return;
                                    }

                                    int val = 0;
                                    if (getInt(prop.Values[0], out val))
                                    {
                                        this._Pass.ClearStencil = val;
                                    }
                                    else
                                    {
                                        compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line);
                                    }
                                }
                                break;

                                #endregion ID_STENCIL_VALUE

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