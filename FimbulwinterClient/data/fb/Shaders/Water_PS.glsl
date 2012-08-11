uniform sampler2D Texture;
uniform float Alpha;

varying vec3 Normal;

void main()
{
	vec4 color = texture2D(Texture, gl_TexCoord[0]);

	color.a = Alpha;

	gl_FragColor = color;
}
