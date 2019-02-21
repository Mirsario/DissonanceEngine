#version 330

uniform sampler2D mainTex;
#ifdef EMISSIONMAP
uniform sampler2D emissionMap;
#endif

#ifdef COLOR
uniform vec4 color = vec4(1f,1f,1f,1f);
#endif

in vec2 vUV;

out vec4 oDiffuse;
out vec4 oEmission;

void main (void)  
{
	//Color
	oDiffuse = texture(mainTex,vUV);
	if(oDiffuse.a==0f) {
		discard;
	}
	#ifdef COLOR
		oDiffuse *= color;
	#endif
	
	//Emission
	#ifdef EMISSIONMAP
		oEmission = texture(emissionMap,vUV);
	#else
		oEmission = vec4(0.0,0.0,0.0,0.0);
	#endif
}