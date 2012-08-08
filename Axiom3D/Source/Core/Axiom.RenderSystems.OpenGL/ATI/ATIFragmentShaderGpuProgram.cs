#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: ATIFragmentShaderGpuProgram.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections.Generic;
using Axiom.Core;
using Axiom.Graphics;
using Axiom.RenderSystems.OpenGL;
using Tao.OpenGl;
using ResourceHandle = System.UInt64;

#endregion Namespace Declarations

namespace Axiom.RenderSystems.OpenGL.ATI
{
    /// <summary>
    ///   Summary description for ATIFragmentShaderGpuProgram.
    /// </summary>
    public class ATIFragmentShaderGpuProgram : GLGpuProgram
    {
        public ATIFragmentShaderGpuProgram(ResourceManager parent, string name, ResourceHandle handle, string group,
                                           bool isManual, IManualResourceLoader loader)
            : base(parent, name, handle, group, isManual, loader)
        {
            throw new AxiomException("This needs upgrading");
            programType = Gl.GL_FRAGMENT_SHADER_ATI;
            programId = Gl.glGenFragmentShadersATI(1);
        }

        #region Implementation of GpuProgram

        protected override void LoadFromSource()
        {
            PixelShader assembler = new PixelShader();

            //bool testError = assembler.RunTests();

            bool error = !assembler.Compile(Source);

            if (!error)
            {
                Gl.glBindFragmentShaderATI(programId);
                Gl.glBeginFragmentShaderATI();

                // Compile and issue shader commands
                error = !assembler.BindAllMachineInstToFragmentShader();

                Gl.glEndFragmentShaderATI();
            }
            else
            {
            }
        }

        public override void Unload()
        {
            base.Unload();

            // delete the fragment shader for good
            Gl.glDeleteFragmentShaderATI(programId);
        }

        #endregion Implementation of GpuProgram

        #region Implementation of GLGpuProgram

        public override void Bind()
        {
            Gl.glEnable(programType);
            Gl.glBindFragmentShaderATI(programId);
        }

        [OgreVersion(1, 7, 2)]
        public override void BindProgramParameters(GpuProgramParameters parms,
                                                   GpuProgramParameters.GpuParamVariability mask)
        {
            // only supports float constants
            GpuProgramParameters.GpuLogicalBufferStruct floatStruct = parms.FloatLogicalBufferStruct;

            foreach (KeyValuePair<int, GpuProgramParameters.GpuLogicalIndexUse> i in floatStruct.Map)
            {
                if ((i.Value.Variability & mask) != 0)
                {
                    int logicalIndex = i.Key;
                    BufferBase pFloat = parms.GetFloatPointer(i.Value.PhysicalIndex).Pointer;
                    // Iterate over the params, set in 4-float chunks (low-level)
                    for (int j = 0; j < i.Value.CurrentSize; j += 4)
                    {
                        Gl.glSetFragmentShaderConstantATI(Gl.GL_CON_0_ATI + logicalIndex, pFloat.Pin());
                        pFloat.UnPin();
                        pFloat += 4;
                        ++logicalIndex;
                    }
                }
            }
        }

        public override void Unbind()
        {
            Gl.glDisable(programType);
        }

        #endregion Implementation of GLGpuProgram
    }

    /// <summary>
    /// </summary>
    public class ATIFragmentShaderFactory : IOpenGLGpuProgramFactory
    {
        #region IOpenGLGpuProgramFactory Members

        public GLGpuProgram Create(ResourceManager parent, string name, ResourceHandle handle, string group,
                                   bool isManual,
                                   IManualResourceLoader loader, GpuProgramType type, string syntaxCode)
        {
            // creates and returns a new ATI fragment shader implementation
            GLGpuProgram ret = new ATIFragmentShaderGpuProgram(parent, name, handle, group, isManual, loader);
            ret.Type = type;
            ret.SyntaxCode = syntaxCode;
            return ret;
        }

        #endregion
    }
}