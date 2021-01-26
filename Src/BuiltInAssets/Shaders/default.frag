#version 330

uniform sampler2D mainTex;
#ifdef NORMALMAP
	uniform sampler2D normalMap;
#endif
#ifdef EMISSION
	uniform float emission = 1f;
#endif
#ifdef EMISSIONMAP
	uniform sampler2D emissionMap;
#endif
#ifdef SPECULAR
	uniform float specular = 1f;
#endif
#ifdef SPECULARMAP
	uniform sampler2D specularMap;
#endif
#ifdef COLOR
	uniform vec4 color = vec4(1f, 1f, 1f, 1f);
#endif
#ifdef ALPHATEST
	uniform float cutoff = 1f;
#endif

in vec2 uv;
in vec3 worldPos;
#if VERTEXCOLOR
	in vec4 col;
#endif
#ifdef NORMALMAP
	in mat3 TBN;
#else
	in vec3 N;
#endif

out vec4 oDiffuse;
out vec3 oNormal;
out vec3 oPosition;
out vec4 oEmission;
out float oSpecular;

void main(void)
{
	//Color
	
	oDiffuse = texture(mainTex, uv);
	
	#ifdef ALPHATEST
		if(oDiffuse.a < cutoff) {
			discard;
		}
	#endif
	
	#ifdef VERTEXCOLOR
		oDiffuse *= col;
	#endif
	
	#ifdef COLOR
		oDiffuse *= color;
	#endif

	//Normals
	
	#ifdef NORMALMAP
		oNormal = normalize(TBN * (texture(normalMap, uv).rgb * 2f - 1f)) * 0.5f + 0.5f;
	#else
		oNormal = N;
	#endif

	//Emission
	
	#ifdef EMISSIONMAP
		oEmission = texture(emissionMap, uv);
		
		#ifdef EMISSION
			oEmission = clamp(oEmission + emission, 0f, 1f);
		#endif
	#else
		#ifdef EMISSION
			oEmission = vec4(emission, emission, emission, emission);
		#else
			oEmission = vec4(0f, 0f, 0f, 0f);
		#endif
	#endif

	//Specular
	
	#ifdef SPECULARMAP
		oSpecular = texture(specularMap, uv).r;
		
		#ifdef SPECULAR
			oSpecular = clamp(oSpecular + specular, 0f, 1f);
		#endif
	#else
		#ifdef SPECULAR
			oSpecular = specular;
		#else
			oSpecular = 0f;
		#endif
	#endif

	//Position
	
	oPosition = worldPos;
}