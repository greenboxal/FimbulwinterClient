varying vec3 Normal;

void main()
{
	gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;

    Normal = gl_NormalMatrix * gl_Normal;

    gl_TexCoord[0] = gl_MultiTexCoord0;
}
