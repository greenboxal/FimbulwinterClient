#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: CompositionTargetPassTranslator.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
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
        public class CompositionTargetPassTranslator : Translator
        {
            protected CompositionTargetPass _Target;

            public CompositionTargetPassTranslator()
            {
                this._Target = null;
            }

            #region Translator Implementation

            /// <see cref="Translator.CheckFor" />
            public override bool CheckFor(Keywords nodeId, Keywords parentId)
            {
                return (nodeId == Keywords.ID_TARGET || nodeId == Keywords.ID_TARGET_OUTPUT) &&
                       parentId == Keywords.ID_TECHNIQUE;
            }

            /// <see cref="Translator.Translate" />
            public override void Translate(ScriptCompiler compiler, AbstractNode node)
            {
                ObjectAbstractNode obj = (ObjectAbstractNode) node;

                CompositionTechnique technique = (CompositionTechnique) obj.Parent.Context;
                if (obj.Id == (uint) Keywords.ID_TARGET)
                {
                    this._Target = technique.CreateTargetPass();
                    if (!string.IsNullOrEmpty(obj.Name))
                    {
                        this._Target.OutputName = obj.Name;
                    }
                }
                else if (obj.Id == (uint) Keywords.ID_TARGET_OUTPUT)
                {
                    this._Target = technique.OutputTarget;
                }
                obj.Context = this._Target;

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
                                #region ID_INPUT

                            case Keywords.ID_INPUT:
                                if (prop.Values.Count == 0)
                                {
                                    compiler.AddError(CompileErrorCode.StringExpected, prop.File, prop.Line);
                                    return;
                                }
                                else if (prop.Values.Count > 1)
                                {
                                    compiler.AddError(CompileErrorCode.FewerParametersExpected, prop.File, prop.Line);
                                    return;
                                }
                                else
                                {
                                    if (prop.Values[0] is AtomAbstractNode)
                                    {
                                        AtomAbstractNode atom = (AtomAbstractNode) prop.Values[0];
                                        switch ((Keywords) atom.Id)
                                        {
                                            case Keywords.ID_NONE:
                                                this._Target.InputMode = CompositorInputMode.None;
                                                break;

                                            case Keywords.ID_PREVIOUS:
                                                this._Target.InputMode = CompositorInputMode.Previous;
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
                                break;

                                #endregion ID_INPUT

                                #region ID_ONLY_INITIAL

                            case Keywords.ID_ONLY_INITIAL:
                                if (prop.Values.Count == 0)
                                {
                                    compiler.AddError(CompileErrorCode.StringExpected, prop.File, prop.Line);
                                    return;
                                }
                                else if (prop.Values.Count > 1)
                                {
                                    compiler.AddError(CompileErrorCode.FewerParametersExpected, prop.File, prop.Line);
                                    return;
                                }
                                else
                                {
                                    bool val = false;
                                    if (getBoolean(prop.Values[0], out val))
                                    {
                                        this._Target.OnlyInitial = val;
                                    }
                                    else
                                    {
                                        compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line);
                                    }
                                }
                                break;

                                #endregion ID_ONLY_INITIAL

                                #region ID_VISIBILITY_MASK

                            case Keywords.ID_VISIBILITY_MASK:
                                if (prop.Values.Count == 0)
                                {
                                    compiler.AddError(CompileErrorCode.StringExpected, prop.File, prop.Line);
                                    return;
                                }
                                else if (prop.Values.Count > 1)
                                {
                                    compiler.AddError(CompileErrorCode.FewerParametersExpected, prop.File, prop.Line);
                                    return;
                                }
                                else
                                {
                                    uint val;
                                    if (getUInt(prop.Values[0], out val))
                                    {
                                        this._Target.VisibilityMask = val;
                                    }
                                    else
                                    {
                                        compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line);
                                    }
                                }
                                break;

                                #endregion ID_VISIBILITY_MASK

                                #region ID_LOD_BIAS

                            case Keywords.ID_LOD_BIAS:
                                if (prop.Values.Count == 0)
                                {
                                    compiler.AddError(CompileErrorCode.StringExpected, prop.File, prop.Line);
                                    return;
                                }
                                else if (prop.Values.Count > 1)
                                {
                                    compiler.AddError(CompileErrorCode.FewerParametersExpected, prop.File, prop.Line);
                                    return;
                                }
                                else
                                {
                                    float val;
                                    if (getFloat(prop.Values[0], out val))
                                    {
                                        this._Target.LodBias = val;
                                    }
                                    else
                                    {
                                        compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line);
                                    }
                                }
                                break;

                                #endregion ID_LOD_BIAS

                                #region ID_MATERIAL_SCHEME

                            case Keywords.ID_MATERIAL_SCHEME:
                                if (prop.Values.Count == 0)
                                {
                                    compiler.AddError(CompileErrorCode.StringExpected, prop.File, prop.Line);
                                    return;
                                }
                                else if (prop.Values.Count > 1)
                                {
                                    compiler.AddError(CompileErrorCode.FewerParametersExpected, prop.File, prop.Line);
                                    return;
                                }
                                else
                                {
                                    string val;
                                    if (getString(prop.Values[0], out val))
                                    {
                                        this._Target.MaterialScheme = val;
                                    }
                                    else
                                    {
                                        compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line);
                                    }
                                }
                                break;

                                #endregion ID_MATERIAL_SCHEME

                                #region ID_SHADOWS_ENABLED

                            case Keywords.ID_SHADOWS_ENABLED:
                                if (prop.Values.Count == 0)
                                {
                                    compiler.AddError(CompileErrorCode.StringExpected, prop.File, prop.Line);
                                    return;
                                }
                                else if (prop.Values.Count > 1)
                                {
                                    compiler.AddError(CompileErrorCode.FewerParametersExpected, prop.File, prop.Line);
                                    return;
                                }
                                else
                                {
                                    bool val;
                                    if (getBoolean(prop.Values[0], out val))
                                    {
                                        this._Target.ShadowsEnabled = val;
                                    }
                                    else
                                    {
                                        compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line);
                                    }
                                }
                                break;

                                #endregion ID_SHADOWS_ENABLED

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