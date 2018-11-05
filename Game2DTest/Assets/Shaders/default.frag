#version 330

uniform sampler2D mainTex;
#ifdef COLOR
uniform vec3 color;
#endif
#ifdef ALPHATEST
uniform float cutoff = 1.0f;
#endif

in vec2 uv;
in vec4 vertexColor;
out vec4 oColor;

void main (void)  
{
	#ifdef ALPHATEST
		oColor=	texture(mainTex,uv);
		if(oColor.a<cutoff) {
			discard;
		}
		#ifdef COLOR
			oColor*=	vertexColor*vec4(color,1.0);
		#else
			oColor*=	vertexColor;
		#endif
	#else
		#ifdef COLOR
			oColor=	texture(mainTex,uv)*vertexColor*vec4(color,1.0);
		#else
			oColor=	texture(mainTex,uv)*vertexColor;
		#endif
	#endif
}