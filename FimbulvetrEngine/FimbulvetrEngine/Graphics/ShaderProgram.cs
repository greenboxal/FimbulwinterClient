using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulvetrEngine.Content;
using OpenTK.Graphics.OpenGL;

namespace FimbulvetrEngine.Graphics
{
    public class ShaderProgram
    {
        public int Id { get; private set; }
        public List<Shader> Shaders { get; private set; }

        public static ShaderProgram LoadVSPSShader(string vs, string ps)
        {
            Shader vss = new Shader(ShaderType.VertexShader, ContentManager.Instance.Load<String>(vs));
            Shader pss = new Shader(ShaderType.VertexShader, ContentManager.Instance.Load<String>(ps));

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

        ~ShaderProgram()
        {
            GL.DeleteProgram(Id);
        }

        public void AttachShader(Shader shader)
        {
            GL.AttachShader(Id, shader.Id);
            Shaders.Add(shader);
        }

        public void Link()
        {
            GL.LinkProgram(Id);
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
