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
    public class GroundShaderProgram : ShaderProgram
    {
        private readonly int _texturePosition;
        private readonly int _lightmapPosition;

        public GroundShaderProgram()
        {
            AttachShader(new Shader(ShaderType.VertexShader, ContentManager.Instance.Load<String>(@"data\fb\Shaders\Ground_VS.glsl")));
            AttachShader(new Shader(ShaderType.FragmentShader, ContentManager.Instance.Load<String>(@"data\fb\Shaders\Ground_PS.glsl")));

            Link();

            _texturePosition = GL.GetUniformLocation(Id, "Texture");
            _lightmapPosition = GL.GetUniformLocation(Id, "Lightmap");
        }

        public void SetTexture(Texture2D texture)
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            texture.Bind();
            GL.Uniform1(_texturePosition, 0);
        }

        public void SetLightmap(Texture2D lightmap)
        {
            GL.ActiveTexture(TextureUnit.Texture1);
            lightmap.Bind();
            GL.Uniform1(_lightmapPosition, 1);
        }
    }
}
