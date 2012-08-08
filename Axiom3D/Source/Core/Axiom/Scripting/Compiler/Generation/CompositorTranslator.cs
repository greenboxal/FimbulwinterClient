#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: CompositorTranslator.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
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
        public class CompositorTranslator : Translator
        {
            protected Compositor _Compositor;

            public CompositorTranslator()
            {
                this._Compositor = null;
            }

            #region Translator Implementation

            public override bool CheckFor(Keywords nodeId, Keywords parentId)
            {
                return nodeId == Keywords.ID_COMPOSITOR;
            }

            /// <see cref="Translator.Translate" />
            public override void Translate(ScriptCompiler compiler, AbstractNode node)
            {
                ObjectAbstractNode obj = (ObjectAbstractNode) node;

                if (obj != null)
                {
                    if (string.IsNullOrEmpty(obj.Name))
                    {
                        compiler.AddError(CompileErrorCode.ObjectNameExpected, obj.File, obj.Line);
                        return;
                    }
                }
                else
                {
                    compiler.AddError(CompileErrorCode.ObjectNameExpected, obj.File, obj.Line);
                    return;
                }

                // Create the compositor
                object compObject;
                ScriptCompilerEvent evt = new CreateCompositorScriptCompilerEvent(obj.File, obj.Name,
                                                                                  compiler.ResourceGroup);
                bool processed = compiler._fireEvent(ref evt, out compObject);

                if (!processed)
                {
                    //TODO
                    // The original translated implementation of this code block was simply the following:
                    // _Compositor = (Compositor)CompositorManager.Instance.Create( obj.Name, compiler.ResourceGroup );
                    // but sometimes it generates an excepiton due to a duplicate resource.
                    // In order to avoid the above mentioned exception, the implementation was changed, but
                    // it need to be checked when ResourceManager._add will be updated to the lastest version

                    Compositor checkForExistingComp = (Compositor) CompositorManager.Instance.GetByName(obj.Name);

                    if (checkForExistingComp == null)
                    {
                        this._Compositor =
                            (Compositor) CompositorManager.Instance.Create(obj.Name, compiler.ResourceGroup);
                    }
                    else
                    {
                        this._Compositor = checkForExistingComp;
                    }
                }
                else
                {
                    this._Compositor = (Compositor) compObject;
                }

                if (this._Compositor == null)
                {
                    compiler.AddError(CompileErrorCode.ObjectAllocationError, obj.File, obj.Line);
                    return;
                }

                // Prepare the compositor
                this._Compositor.RemoveAllTechniques();
                this._Compositor.Origin = obj.File;
                obj.Context = this._Compositor;

                foreach (AbstractNode i in obj.Children)
                {
                    if (i is ObjectAbstractNode)
                    {
                        processNode(compiler, i);
                    }
                    else
                    {
                        compiler.AddError(CompileErrorCode.UnexpectedToken, i.File, i.Line, "token not recognized");
                    }
                }
            }

            #endregion Translator Implementation
        }
    }
}