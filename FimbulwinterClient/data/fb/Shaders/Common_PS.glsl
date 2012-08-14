uniform sampler2D Texture;
uniform float Alpha;

varying vec3 Normal;

void main()
{
	vec4 color = texture2D(Texture, gl_TexCoord[0]);

	if (color.r == 1.0F && color.g == 0 && color.b == 1.0F)
		color.a = 0.0F;
	else
		color.a = Alpha;

	gl_FragColor = color;
}
