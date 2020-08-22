#version 330

uniform mat4 world;
uniform mat4 worldViewProj;
#ifdef TILING
uniform vec2 uvTiling = vec2(1f,1f);
#endif
#ifdef SKINNED
uniform mat4 boneMatrices[64];
#endif

in vec4 vertex;
in vec3 normal;
#ifdef NORMALMAP
in vec4 tangent;
#endif
#ifdef SKINNED
in vec4 boneIndices;
in vec4 boneWeights;
#endif
#ifdef VERTEXCOLOR
in vec4 color;
#endif
in vec2 uv0;

out vec2 uv;
#ifdef VERTEXCOLOR
out vec4 col;
#endif
out vec3 worldPos;
#ifdef NORMALMAP
out mat3 TBN;
#else
out vec3 N;
#endif

void main(void)
{
	#ifdef NORMALMAP
		vec3 binormal = cross(tangent.xyz,normal)*tangent.w;
		vec3 T = normalize((world*vec4(tangent.xyz,0.0)).xyz);
		vec3 B = normalize((world*vec4(binormal,0.0)).xyz);
		vec3 N = normalize((world*vec4(normal,0.0)).xyz);
		TBN = mat3(T,B,N);
	#else
		N = normalize(mat3(world)*normal)*0.5+0.5;
	#endif
	
	#ifdef SKINNED
		vec4 pos = vec4(0,0,0,0);
		mat4 matrix =
			boneMatrices[int(boneIndices.x)]*boneWeights.x+
			boneMatrices[int(boneIndices.y)]*boneWeights.y+
			boneMatrices[int(boneIndices.z)]*boneWeights.z+
			boneMatrices[int(boneIndices.w)]*boneWeights.w;
		
		gl_Position = worldViewProj*(vertex*matrix);
	#else
		gl_Position = worldViewProj*vertex;
	#endif
	
	#ifdef VERTEXCOLOR
		col = color;
	#endif
	
	#ifdef TILING
		uv = uv0*uvTiling;
	#else
		uv = uv0;
	#endif
	
	worldPos = (world*vertex).xyz;
}