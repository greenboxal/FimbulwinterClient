#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: GpuProgramSharedParametersTranslator.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using Axiom.Graphics;
using Axiom.Scripting.Compiler.AST;

#endregion Namespace Declarations

namespace Axiom.Scripting.Compiler
{
    public partial class ScriptCompiler
    {
        public class SharedParametersTranslator : Translator
        {
            #region Translator Implementation

            public override bool CheckFor(Keywords nodeId, Keywords parentId)
            {
                return nodeId == Keywords.ID_SHARED_PARAMS;
            }

            /// <see cref="Translator.Translate" />
            [OgreVersion(1, 7, 2)]
            public override void Translate(ScriptCompiler compiler, AbstractNode node)
            {
                ObjectAbstractNode obj = (ObjectAbstractNode) node;

                // Must have a name
                if (string.IsNullOrEmpty(obj.Name))
                {
                    compiler.AddError(CompileErrorCode.ObjectNameExpected, obj.File, obj.Line,
                                      "shared_params must be given a name");
                    return;
                }

                object paramsObj;
                GpuProgramParameters.GpuSharedParameters sharedParams;
                ScriptCompilerEvent evt = new CreateGpuSharedParametersScriptCompilerEvent(obj.File, obj.Name,
                                                                                           compiler.ResourceGroup);
                bool processed = compiler._fireEvent(ref evt, out paramsObj);

                if (!processed)
                {
                    sharedParams = GpuProgramManager.Instance.CreateSharedParameters(obj.Name);
                }
                else
                {
                    sharedParams = (GpuProgramParameters.GpuSharedParameters) paramsObj;
                }

                if (sharedParams == null)
                {
                    compiler.AddError(CompileErrorCode.ObjectAllocationError, obj.File, obj.Line);
                    return;
                }

                foreach (AbstractNode i in obj.Children)
                {
                    if (!(i is PropertyAbstractNode))
                    {
                        continue;
                    }

                    PropertyAbstractNode prop = (PropertyAbstractNode) i;

                    switch ((Keywords) prop.Id)
                    {
                            #region ID_SHARED_PARAM_NAMED

                        case Keywords.ID_SHARED_PARAM_NAMED:
                            {
                                if (prop.Values.Count < 2)
                                {
                                    compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line,
                                                      "shared_param_named - expected 2 or more arguments");
                                    continue;
                                }

                                AbstractNode i0 = getNodeAt(prop.Values, 0);
                                AbstractNode i1 = getNodeAt(prop.Values, 1);

                                if (!(i0 is AtomAbstractNode) || !(i1 is AtomAbstractNode))
                                {
                                    compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line,
                                                      "name and parameter type expected");
                                    continue;
                                }

                                AtomAbstractNode atom0 = (AtomAbstractNode) i0;
                                string pName = atom0.Value;
                                GpuProgramParameters.GpuConstantType constType;
                                int arraySz = 1;
                                if (!getConstantType(i1, out constType))
                                {
                                    compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line,
                                                      "invalid parameter type");
                                    continue;
                                }

                                bool isFloat = GpuProgramParameters.GpuConstantDefinition.IsFloatConst(constType);

                                GpuProgramParameters.FloatConstantList mFloats =
                                    new GpuProgramParameters.FloatConstantList();
                                GpuProgramParameters.IntConstantList mInts = new GpuProgramParameters.IntConstantList();

                                for (int otherValsi = 2; otherValsi < prop.Values.Count; ++otherValsi)
                                {
                                    if (!(prop.Values[otherValsi] is AtomAbstractNode))
                                    {
                                        continue;
                                    }

                                    AtomAbstractNode atom = (AtomAbstractNode) prop.Values[otherValsi];

                                    if (atom.Value[0] == '[' && atom.Value[atom.Value.Length - 1] == ']')
                                    {
                                        string arrayStr = atom.Value.Substring(1, atom.Value.Length - 2);
                                        if (!int.TryParse(arrayStr, out arraySz))
                                        {
                                            compiler.AddError(CompileErrorCode.NumberExpected, prop.File, prop.Line,
                                                              "invalid array size");
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        float floatVal = 0.0f;
                                        int intVal = 0;
                                        bool parseRes = false;

                                        if (isFloat)
                                        {
                                            parseRes = float.TryParse(atom.Value, out floatVal);
                                        }
                                        else
                                        {
                                            parseRes = int.TryParse(atom.Value, out intVal);
                                        }

                                        if (!parseRes)
                                        {
                                            compiler.AddError(CompileErrorCode.NumberExpected, prop.File, prop.Line,
                                                              atom.Value +
                                                              " invalid - extra parameters to shared_param_named must be numbers");
                                            continue;
                                        }
                                        if (isFloat)
                                        {
                                            mFloats.Add(floatVal);
                                        }
                                        else
                                        {
                                            mInts.Add(intVal);
                                        }
                                    }
                                } // each extra param

                                // define constant entry
                                try
                                {
                                    sharedParams.AddConstantDefinition(pName, constType, arraySz);
                                }
                                catch (Exception e)
                                {
                                    compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line,
                                                      e.Message);
                                    continue;
                                }

                                // initial values
                                int elemsExpected =
                                    GpuProgramParameters.GpuConstantDefinition.GetElementSize(constType, false)*arraySz;
                                int elemsFound = isFloat ? mFloats.Count : mInts.Count;
                                if (elemsFound > 0)
                                {
                                    if (elemsExpected != elemsFound)
                                    {
                                        compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line,
                                                          "Wrong number of values supplied for parameter type");
                                        continue;
                                    }

                                    if (isFloat)
                                    {
                                        sharedParams.SetNamedConstant(pName, mFloats.Data);
                                    }
                                    else
                                    {
                                        sharedParams.SetNamedConstant(pName, mInts.Data);
                                    }
                                }
                            }
                            break;

                            #endregion ID_SHARED_PARAM_NAMED

                        default:
                            break;
                    }
                }
            }

            #endregion Translator Implementation
        }
    };
}