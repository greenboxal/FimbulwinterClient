uniform sampler2D Texture;
uniform sampler2D Lightmap;

varying vec3 Normal;

void main()
{
	vec4 color = texture2D(Texture, gl_TexCoord[0]);
	vec4 lmap = texture2D(Lightmap, gl_TexCoord[1]);
	
	color.rgb *= lmap.a;
	color.rgb += lmap.rgb;

	gl_FragColor = color;
}
