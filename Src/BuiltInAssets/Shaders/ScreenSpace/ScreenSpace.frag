#version 330

#ifdef TEXTURE
	uniform sampler2D mainTex;
#endif
#ifdef COLOR
	uniform vec4 color = vec4(1f, 1f, 1f, 1f);
#endif

#ifdef TEXTURE
	in vec2 vUV;
#endif
#ifdef VERTEXCOLOR
	in vec4 vColor;
#endif

layout(location = 0) out vec4 oColor;

void main (void)  
{
	#ifdef TEXTURE
		oColor = texture(mainTex, vUV);
	#else
		oColor = vec4(1.0);
	#endif
	
	#ifdef COLOR
		oColor *= color;
	#endif
	
	#ifdef VERTEXCOLOR
		oColor *= vColor;
	#endif
}