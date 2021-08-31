#version 330

uniform sampler2D mainTex;
#ifdef NORMALMAP
	uniform sampler2D normalMap;
#endif
#ifdef EMISSION
	uniform vec3 emission;
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

layout(location = 0) out vec4 oDiffuse;
layout(location = 1) out vec3 oNormal;
layout(location = 2) out vec3 oPosition;
layout(location = 3) out vec3 oEmission;
layout(location = 4) out float oSpecular;

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
		oEmission = texture(emissionMap, uv).rgb;
	#elif defined EMISSION
		oEmission = emission;
	#else
		oEmission = vec3(0f, 0f, 0f);
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