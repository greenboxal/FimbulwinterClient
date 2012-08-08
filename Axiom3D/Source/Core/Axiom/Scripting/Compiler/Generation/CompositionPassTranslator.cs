#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: CompositionPassTranslator.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using Axiom.Core;
using Axiom.Graphics;
using Axiom.Scripting.Compiler.AST;

#endregion Namespace Declarations

namespace Axiom.Scripting.Compiler
{
    public partial class ScriptCompiler
    {
        public class CompositionPassTranslator : Translator
        {
            protected CompositionPass _Pass;

            public CompositionPassTranslator()
            {
                this._Pass = null;
            }

            #region Translator Implementation

            /// <see cref="Translator.CheckFor" />
            public override bool CheckFor(Keywords nodeId, Keywords parentId)
            {
                return nodeId == Keywords.ID_PASS &&
                       (parentId == Keywords.ID_TARGET || parentId == Keywords.ID_TARGET_OUTPUT);
            }

            /// <see cref="Translator.Translate" />
            public override void Translate(ScriptCompiler compiler, AbstractNode node)
            {
                ObjectAbstractNode obj = (ObjectAbstractNode) node;

                CompositionTargetPass target = (CompositionTargetPass) obj.Parent.Context;
                this._Pass = target.CreatePass();
                obj.Context = this._Pass;

                // The name is the type of the pass
                if (obj.Values.Count == 0)
                {
                    compiler.AddError(CompileErrorCode.StringExpected, obj.File, obj.Line);
                    return;
                }
                string type = string.Empty;
                if (!getString(obj.Values[0], out type))
                {
                    compiler.AddError(CompileErrorCode.InvalidParameters, obj.File, obj.Line);
                    return;
                }

                this._Pass.Type = (CompositorPassType) ScriptEnumAttribute.Lookup(type, typeof (CompositorPassType));
                if (this._Pass.Type == CompositorPassType.RenderCustom)
                {
                    string customType = string.Empty;
                    //This is the ugly one liner for safe access to the second parameter.
                    if (obj.Values.Count < 2 || !getString(obj.Values[1], out customType))
                    {
                        compiler.AddError(CompileErrorCode.StringExpected, obj.File, obj.Line);
                        return;
                    }
                    this._Pass.CustomType = customType;
                }
                else
                {
                    compiler.AddError(CompileErrorCode.InvalidParameters, obj.File, obj.Line,
                                      "pass types must be \"clear\", \"stencil\", \"render_quad\", \"render_scene\" or \"render_custom\".");
                    return;
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
                                #region ID_MATERIAL

                            case Keywords.ID_MATERIAL:
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
                                    string val = string.Empty;
                                    if (getString(prop.Values[0], out val))
                                    {
                                        ScriptCompilerEvent evt =
                                            new ProcessResourceNameScriptCompilerEvent(
                                                ProcessResourceNameScriptCompilerEvent.ResourceType.Material, val);

                                        compiler._fireEvent(ref evt);
                                        this._Pass.MaterialName = ((ProcessResourceNameScriptCompilerEvent) evt).Name;
                                    }
                                    else
                                    {
                                        compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line);
                                    }
                                }
                                break;

                                #endregion ID_MATERIAL

                                #region ID_INPUT

                            case Keywords.ID_INPUT:
                                if (prop.Values.Count < 2)
                                {
                                    compiler.AddError(CompileErrorCode.StringExpected, prop.File, prop.Line);
                                    return;
                                }
                                else if (prop.Values.Count > 3)
                                {
                                    compiler.AddError(CompileErrorCode.FewerParametersExpected, prop.File, prop.Line);
                                    return;
                                }
                                else
                                {
                                    AbstractNode i0 = getNodeAt(prop.Values, 0),
                                                 i1 = getNodeAt(prop.Values, 1),
                                                 i2 = getNodeAt(prop.Values, 2);
                                    int id = 0;
                                    string name = string.Empty;
                                    if (getInt(i0, out id) && getString(i1, out name))
                                    {
                                        int index = 0;

                                        if (!getInt(i2, out index))
                                        {
                                            compiler.AddError(CompileErrorCode.NumberExpected, prop.File, prop.Line);
                                            return;
                                        }

                                        this._Pass.SetInput(id, name, index);
                                    }
                                    else
                                    {
                                        compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line);
                                    }
                                }
                                break;

                                #endregion ID_INPUT

                                #region ID_IDENTIFIER

                            case Keywords.ID_IDENTIFIER:
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
                                        this._Pass.Identifier = val;
                                    }
                                    else
                                    {
                                        compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line);
                                    }
                                }
                                break;

                                #endregion ID_IDENTIFIER

                                #region ID_FIRST_RENDER_QUEUE

                            case Keywords.ID_FIRST_RENDER_QUEUE:
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
                                        this._Pass.FirstRenderQueue = (RenderQueueGroupID) val;
                                    }
                                    else
                                    {
                                        compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line);
                                    }
                                }
                                break;

                                #endregion ID_FIRST_RENDER_QUEUE

                                #region ID_LAST_RENDER_QUEUE

                            case Keywords.ID_LAST_RENDER_QUEUE:
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
                                        this._Pass.LastRenderQueue = (RenderQueueGroupID) val;
                                    }
                                    else
                                    {
                                        compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line);
                                    }
                                }
                                break;

                                #endregion ID_LAST_RENDER_QUEUE

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
                                        this._Pass.MaterialScheme = val;
                                    }
                                    else
                                    {
                                        compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line);
                                    }
                                }
                                break;

                                #endregion ID_MATERIAL_SCHEME

                                #region ID_QUAD_NORMALS

                            case Keywords.ID_QUAD_NORMALS:
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
                                        if (atom.Id == (uint) Keywords.ID_CAMERA_FAR_CORNERS_VIEW_SPACE)
                                        {
                                            this._Pass.SetQuadFarCorners(true, true);
                                        }
                                        else if (atom.Id == (uint) Keywords.ID_CAMERA_FAR_CORNERS_WORLD_SPACE)
                                        {
                                            this._Pass.SetQuadFarCorners(true, false);
                                        }
                                        else
                                        {
                                            compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line);
                                        }
                                    }
                                    else
                                    {
                                        compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line);
                                    }
                                }
                                break;

                                #endregion ID_QUAD_NORMALS

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