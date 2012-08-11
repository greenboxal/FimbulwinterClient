using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulvetrEngine.Content;
using OpenTK.Graphics.OpenGL;

namespace FimbulvetrEngine.Graphics
{
    public class ShaderProgram : ThreadBoundDisposable
    {
        public int Id { get; private set; }
        public List<Shader> Shaders { get; private set; }

        public static ShaderProgram LoadVSPSShader(string vs, string ps)
        {
            Shader vss = new Shader(ShaderType.VertexShader, ContentManager.Instance.Load<String>(vs));
            Shader pss = new Shader(ShaderType.FragmentShader, ContentManager.Instance.Load<String>(ps));

            ShaderProgram program = new ShaderProgram();
            program.AttachShader(vss);
            program.AttachShader(pss);

            return program;
        }

        public ShaderProgram()
        {
            Id = GL.CreateProgram();
            Shaders = new List<Shader>();
        }

        protected override void GCUnmanagedFinalize()
        {
            if (Id != 0)
                GL.DeleteProgram(Id);
        }

        public void AttachShader(Shader shader)
        {
            GL.AttachShader(Id, shader.Id);

            // We should maitain a reference to it or the GC may reclaim the Shader object
            Shaders.Add(shader);
        }

        public void Link()
        {
            GL.LinkProgram(Id);

            int success;
            GL.GetProgram(Id, ProgramParameter.LinkStatus, out success);

            if (success != 0)
                return;

            int logLen;
            GL.GetShader(Id, ShaderParameter.CompileStatus, out logLen);

            if (success == 0)
            {
                throw new Exception("Error linking ShaderProgram:\n" + GL.GetProgramInfoLog(Id));
            }
        }

        public void Begin()
        {
            GL.UseProgram(Id);
        }

        public void End()
        {
            GL.UseProgram(0);
        }
    }
}
