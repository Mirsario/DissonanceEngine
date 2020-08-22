#version 330

#ifdef TEXTURE
in vec2 uv0;
#endif
#ifdef VERTEXCOLOR
in vec4 color;
#endif
in vec4 vertex;

#ifdef TEXTURE
out vec2 vUV;
#endif
#ifdef VERTEXCOLOR
out vec4 vColor;
#endif

void main(void)
{
	#ifdef TEXTURE
	vUV = uv0;
	#endif
	#ifdef VERTEXCOLOR
	vColor = color;
	#endif
	
	gl_Position = vertex;
}