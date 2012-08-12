uniform sampler2D Texture;
uniform float Alpha;

varying vec3 Normal;

void main()
{
	vec4 color = texture2D(Texture, gl_TexCoord[0]);

	if (color.r == 255 && color.g == 0 && color.b == 0)
		color.a = 0.0F;
	else
		color.a = Alpha;

	gl_FragColor = color;
}
