#version 330

uniform mat4 world;
uniform mat4 worldViewProj;
#ifdef TILING
uniform vec2 uvTiling = vec2(1f,1f);
#endif

in vec4 vertex;
in vec3 normal;
#ifdef NORMALMAP
in vec4 tangent;
#endif
in vec4 color;
in vec2 uv0;
out vec2 uv;
out vec4 col;
out vec3 worldPos;
#ifdef NORMALMAP
out mat3 TBN;
#else
out vec3 N;
#endif

void main(void)
{
	vec3 n = vec3(0f,1f,0f);
	
	#ifdef NORMALMAP
		vec3 binormal = cross(tangent.xyz,n)*tangent.w;
		vec3 T = normalize((world*vec4(tangent.xyz,0.0)).xyz);
		vec3 B = normalize((world*vec4(binormal,0.0)).xyz);
		vec3 N = normalize((world*vec4(n,0.0)).xyz);
		TBN = mat3(T,B,N);
	#else
		N = normalize(mat3(world)*n)*0.5+0.5;
	#endif
	
	gl_Position = worldViewProj*vertex;
	
	col = color;
	
	#ifdef TILING
		uv = uv0*uvTiling;
	#else
		uv = uv0;
	#endif
	
	worldPos = (world*vertex).xyz;
}