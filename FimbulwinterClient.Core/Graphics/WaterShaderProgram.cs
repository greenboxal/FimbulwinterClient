using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FimbulvetrEngine.Content;
using FimbulvetrEngine.Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FimbulwinterClient.Core.Graphics
{
    public class WaterShaderProgram : ShaderProgram
    {
        private readonly int _texturePosition;
        private readonly int _alphaPosition;

        public WaterShaderProgram()
        {
            AttachShader(new Shader(ShaderType.VertexShader, ContentManager.Instance.Load<String>(@"data\fb\Shaders\Water_VS.glsl")));
            AttachShader(new Shader(ShaderType.FragmentShader, ContentManager.Instance.Load<String>(@"data\fb\Shaders\Water_PS.glsl")));

            Link();

            _texturePosition = GL.GetUniformLocation(Id, "Texture");
            _alphaPosition = GL.GetUniformLocation(Id, "Alpha");
        }

        public void SetTexture(Texture2D texture)
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            texture.Bind();
            GL.Uniform1(_texturePosition, 0);
        }

        public void SetAlpha(float alpha)
        {
            GL.Uniform1(_alphaPosition, alpha);
        }
    }
}
