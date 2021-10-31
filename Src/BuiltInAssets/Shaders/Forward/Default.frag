#version 330

#ifdef TEXTURE
	uniform sampler2D mainTex;
#endif
#ifdef COLOR
	uniform vec4 color = vec4(1f, 1f, 1f, 1f);
#endif
#ifdef ALPHATEST
	uniform float cutoff = 1f;
#endif

#ifdef TEXTURE
	in vec2 vUV;
#endif
#ifdef VERTEXCOLOR
	in vec4 vColor;
#endif

layout(location = 0) out vec4 oDiffuse;

void main (void)  
{
	#ifdef TEXTURE
		oDiffuse = texture(mainTex, vUV);
	#else
		oDiffuse = vec4(1.0);
	#endif
	
	#ifdef ALPHATEST
		if (oDiffuse.a < cutoff) {
			discard;
		}
	#endif
	
	#ifdef COLOR
		oDiffuse *= color;
	#endif
	
	#ifdef VERTEXCOLOR
		oDiffuse *= vColor;
	#endif
}