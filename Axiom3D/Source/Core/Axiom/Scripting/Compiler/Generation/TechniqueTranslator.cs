#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: TechniqueTranslator.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
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
        public class TechniqueTranslator : Translator
        {
            protected Technique _technique;

            public TechniqueTranslator()
            {
                this._technique = null;
            }

            #region Translator Implementation

            /// <see cref="Translator.CheckFor" />
            public override bool CheckFor(Keywords nodeId, Keywords parentId)
            {
                return nodeId == Keywords.ID_TECHNIQUE && parentId == Keywords.ID_MATERIAL;
            }

            /// <see cref="Translator.Translate" />
            public override void Translate(ScriptCompiler compiler, AbstractNode node)
            {
                ObjectAbstractNode obj = (ObjectAbstractNode) node;

                // Create the technique from the material
                Material material = (Material) obj.Parent.Context;
                this._technique = material.CreateTechnique();
                obj.Context = this._technique;

                // Get the name of the technique
                if (!string.IsNullOrEmpty(obj.Name))
                {
                    this._technique.Name = obj.Name;
                }

                // Set the properties for the technique
                foreach (AbstractNode i in obj.Children)
                {
                    if (i is PropertyAbstractNode)
                    {
                        PropertyAbstractNode prop = (PropertyAbstractNode) i;

                        switch ((Keywords) prop.Id)
                        {
                                #region ID_SCHEME

                            case Keywords.ID_SCHEME:
                                if (prop.Values.Count == 0)
                                {
                                    compiler.AddError(CompileErrorCode.StringExpected, prop.File, prop.Line);
                                }
                                else if (prop.Values.Count > 1)
                                {
                                    compiler.AddError(CompileErrorCode.FewerParametersExpected, prop.File, prop.Line,
                                                      "scheme only supports 1 argument");
                                }
                                else
                                {
                                    string scheme;
                                    if (getString(prop.Values[0], out scheme))
                                    {
                                        this._technique.Scheme = scheme;
                                    }
                                    else
                                    {
                                        compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line,
                                                          "scheme must have 1 string argument");
                                    }
                                }
                                break;

                                #endregion ID_SCHEME

                                #region ID_LOD_INDEX

                            case Keywords.ID_LOD_INDEX:
                                if (prop.Values.Count == 0)
                                {
                                    compiler.AddError(CompileErrorCode.StringExpected, prop.File, prop.Line);
                                }
                                else if (prop.Values.Count > 1)
                                {
                                    compiler.AddError(CompileErrorCode.FewerParametersExpected, prop.File, prop.Line,
                                                      "lod_index only supports 1 argument");
                                }
                                else
                                {
                                    int val;
                                    if (getInt(prop.Values[0], out val))
                                    {
                                        this._technique.LodIndex = val;
                                    }
                                    else
                                    {
                                        compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line,
                                                          "lod_index cannot accept argument \"" + prop.Values[0].Value +
                                                          "\"");
                                    }
                                }
                                break;

                                #endregion ID_LOD_INDEX

                                #region ID_SHADOW_CASTER_MATERIAL

                            case Keywords.ID_SHADOW_CASTER_MATERIAL:
                                if (prop.Values.Count == 0)
                                {
                                    compiler.AddError(CompileErrorCode.StringExpected, prop.File, prop.Line);
                                }
                                else if (prop.Values.Count > 1)
                                {
                                    compiler.AddError(CompileErrorCode.FewerParametersExpected, prop.File, prop.Line,
                                                      "shadow_caster_material only accepts 1 argument");
                                }
                                else
                                {
                                    string matName;
                                    if (getString(prop.Values[0], out matName))
                                    {
                                        string evtMatName = string.Empty;

                                        ScriptCompilerEvent evt =
                                            new ProcessResourceNameScriptCompilerEvent(
                                                ProcessResourceNameScriptCompilerEvent.ResourceType.Material,
                                                matName);

                                        compiler._fireEvent(ref evt);
                                        evtMatName = ((ProcessResourceNameScriptCompilerEvent) evt).Name;
                                        this._technique.ShadowCasterMaterial =
                                            (Material) MaterialManager.Instance[evtMatName];
                                        // Use the processed name
                                    }
                                    else
                                    {
                                        compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line,
                                                          "shadow_caster_material cannot accept argument \"" +
                                                          prop.Values[0].Value + "\"");
                                    }
                                }
                                break;

                                #endregion ID_SHADOW_CASTER_MATERIAL

                                #region ID_SHADOW_RECEIVER_MATERIAL

                            case Keywords.ID_SHADOW_RECEIVER_MATERIAL:
                                if (prop.Values.Count == 0)
                                {
                                    compiler.AddError(CompileErrorCode.StringExpected, prop.File, prop.Line);
                                }
                                else if (prop.Values.Count > 1)
                                {
                                    compiler.AddError(CompileErrorCode.FewerParametersExpected, prop.File, prop.Line,
                                                      "shadow_receiver_material only accepts 1 argument");
                                }
                                else
                                {
                                    AbstractNode i0 = getNodeAt(prop.Values, 0);
                                    string matName = string.Empty;
                                    if (getString(i0, out matName))
                                    {
                                        string evtName = string.Empty;

                                        ScriptCompilerEvent evt =
                                            new ProcessResourceNameScriptCompilerEvent(
                                                ProcessResourceNameScriptCompilerEvent.ResourceType.Material,
                                                matName);

                                        compiler._fireEvent(ref evt);
                                        evtName = ((ProcessResourceNameScriptCompilerEvent) evt).Name;
                                        this._technique.ShadowReceiverMaterial =
                                            (Material) MaterialManager.Instance[evtName];
                                    }
                                    else
                                    {
                                        compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line,
                                                          "shadow_receiver_material_name cannot accept argument \"" +
                                                          i0.Value + "\"");
                                    }
                                }
                                break;

                                #endregion ID_SHADOW_RECEIVER_MATERIAL

                                #region ID_GPU_VENDOR_RULE

                            case Keywords.ID_GPU_VENDOR_RULE:
                                if (prop.Values.Count < 2)
                                {
                                    compiler.AddError(CompileErrorCode.StringExpected, prop.File, prop.Line,
                                                      "gpu_vendor_rule must have 2 arguments");
                                }
                                else if (prop.Values.Count > 2)
                                {
                                    compiler.AddError(CompileErrorCode.FewerParametersExpected, prop.File, prop.Line,
                                                      "gpu_vendor_rule must have 2 arguments");
                                }
                                else
                                {
                                    AbstractNode i0 = getNodeAt(prop.Values, 0);
                                    AbstractNode i1 = getNodeAt(prop.Values, 1);

                                    Technique.GPUVendorRule rule = new Technique.GPUVendorRule();
                                    if (i0 is AtomAbstractNode)
                                    {
                                        AtomAbstractNode atom0 = (AtomAbstractNode) i0;
                                        Keywords atom0Id = (Keywords) atom0.Id;

                                        if (atom0Id == Keywords.ID_INCLUDE)
                                        {
                                            rule.Include = true;
                                        }
                                        else if (atom0Id == Keywords.ID_EXCLUDE)
                                        {
                                            rule.Include = false;
                                        }
                                        else
                                        {
                                            compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line,
                                                              "gpu_vendor_rule cannot accept \"" + i0.Value +
                                                              "\" as first argument");
                                        }

                                        string vendor = string.Empty;
                                        if (!getString(i1, out vendor))
                                        {
                                            compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line,
                                                              "gpu_vendor_rule cannot accept \"" + i1.Value +
                                                              "\" as second argument");
                                        }

                                        rule.Vendor = RenderSystemCapabilities.VendorFromString(vendor);

                                        if (rule.Vendor != GPUVendor.Unknown)
                                        {
                                            this._technique.AddGPUVenderRule(rule);
                                        }
                                    }
                                    else
                                    {
                                        compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line,
                                                          "gpu_vendor_rule cannot accept \"" + i0.Value +
                                                          "\" as first argument");
                                    }
                                }
                                break;

                                #endregion ID_GPU_VENDOR_RULE

                                #region ID_GPU_DEVICE_RULE

                            case Keywords.ID_GPU_DEVICE_RULE:
                                if (prop.Values.Count < 2)
                                {
                                    compiler.AddError(CompileErrorCode.StringExpected, prop.File, prop.Line,
                                                      "gpu_device_rule must have at least 2 arguments");
                                }
                                else if (prop.Values.Count > 3)
                                {
                                    compiler.AddError(CompileErrorCode.FewerParametersExpected, prop.File, prop.Line,
                                                      "gpu_device_rule must have at most 3 arguments");
                                }
                                else
                                {
                                    AbstractNode i0 = getNodeAt(prop.Values, 0);
                                    AbstractNode i1 = getNodeAt(prop.Values, 1);

                                    Technique.GPUDeviceNameRule rule = new Technique.GPUDeviceNameRule();
                                    if (i0 is AtomAbstractNode)
                                    {
                                        AtomAbstractNode atom0 = (AtomAbstractNode) i0;
                                        Keywords atom0Id = (Keywords) atom0.Id;

                                        if (atom0Id == Keywords.ID_INCLUDE)
                                        {
                                            rule.Include = true;
                                        }
                                        else if (atom0Id == Keywords.ID_EXCLUDE)
                                        {
                                            rule.Include = false;
                                        }
                                        else
                                        {
                                            compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line,
                                                              "gpu_device_rule cannot accept \"" + i0.Value +
                                                              "\" as first argument");
                                        }

                                        if (!getString(i1, out rule.DevicePattern))
                                        {
                                            compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line,
                                                              "gpu_device_rule cannot accept \"" + i1.Value +
                                                              "\" as second argument");
                                        }

                                        if (prop.Values.Count == 3)
                                        {
                                            AbstractNode i2 = getNodeAt(prop.Values, 2);
                                            if (!getBoolean(i2, out rule.CaseSensitive))
                                            {
                                                compiler.AddError(CompileErrorCode.InvalidParameters, prop.File,
                                                                  prop.Line,
                                                                  "gpu_device_rule third argument must be \"true\", \"false\", \"yes\", \"no\", \"on\", or \"off\"");
                                            }
                                        }

                                        this._technique.AddGPUDeviceNameRule(rule);
                                    }
                                    else
                                    {
                                        compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line,
                                                          "gpu_device_rule cannot accept \"" + i0.Value +
                                                          "\" as first argument");
                                    }
                                }
                                break;

                                #endregion ID_GPU_DEVICE_RULE

                            default:
                                compiler.AddError(CompileErrorCode.UnexpectedToken, prop.File, prop.Line,
                                                  "token \"" + prop.Name + "\" is not recognized");
                                break;
                        } //end of switch statement
                    } // end of if ( i is PropertyAbstractNode )
                    else if (i is ObjectAbstractNode)
                    {
                        processNode(compiler, i);
                    }
                }
            }

            #endregion Translator Implementation
        }
    }
}