#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: ParticleAffectorTranslator.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using Axiom.ParticleSystems;
using Axiom.Scripting.Compiler.AST;

#endregion Namespace Declarations

namespace Axiom.Scripting.Compiler
{
    public partial class ScriptCompiler
    {
        public class ParticleAffectorTranslator : Translator
        {
            protected ParticleAffector _Affector;

            public ParticleAffectorTranslator()
            {
                this._Affector = null;
            }

            #region Translator Implementation

            /// <see cref="Translator.CheckFor" />
            public override bool CheckFor(Keywords nodeId, Keywords parentId)
            {
                return nodeId == Keywords.ID_AFFECTOR;
            }

            /// <see cref="Translator.Translate" />
            public override void Translate(ScriptCompiler compiler, AbstractNode node)
            {
                ObjectAbstractNode obj = (ObjectAbstractNode) node;

                // Must have a type as the first value
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

                ParticleSystem system = (ParticleSystem) obj.Parent.Context;
                this._Affector = system.AddAffector(type);

                foreach (AbstractNode i in obj.Children)
                {
                    if (i is PropertyAbstractNode)
                    {
                        PropertyAbstractNode prop = (PropertyAbstractNode) i;
                        string value = string.Empty;

                        // Glob the values together
                        foreach (AbstractNode it in prop.Values)
                        {
                            if (it is AtomAbstractNode)
                            {
                                if (string.IsNullOrEmpty(value))
                                {
                                    value = (it).Value;
                                }
                                else
                                {
                                    value = value + " " + (it).Value;
                                }
                            }
                            else
                            {
                                compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line);
                                break;
                            }
                        }

                        if (!this._Affector.SetParam(prop.Name, value))
                        {
                            compiler.AddError(CompileErrorCode.InvalidParameters, prop.File, prop.Line);
                        }
                    }
                    else
                    {
                        processNode(compiler, i);
                    }
                }
            }

            #endregion Translator Implementation
        }
    }
}