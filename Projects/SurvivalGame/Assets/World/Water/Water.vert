#version 330

uniform float time;
uniform mat4 world;
uniform mat4 worldViewProj;

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

float getDisplacement(vec3 pos,vec2 texUv)
{
	return sin(time+(pos.x+pos.z)*0.1f)*0.5f+0.5f;
}

void main(void)
{
	vec4 newVertex = vertex;
	
	worldPos = (world*newVertex).xyz;
	
	newVertex.y += mix(-2f,2f,getDisplacement(worldPos,uv0));
	
	worldPos = (world*newVertex).xyz;
	
	gl_Position = worldViewProj*newVertex;
	
	#ifdef NORMALMAP
		vec3 binormal = cross(tangent.xyz,normal)*tangent.w;
		vec3 T = normalize((world*vec4(tangent.xyz,0.0)).xyz);
		vec3 B = normalize((world*vec4(binormal,0.0)).xyz);
		vec3 N = normalize((world*vec4(normal,0.0)).xyz);
		TBN = mat3(T,B,N);
	#else
		//N = normalize(mat3(world)*normal)*0.5+0.5;
		float depthDeltaX = (getDisplacement(vec3(newVertex.x-1f,newVertex.yz),uv0)-getDisplacement(vec3(newVertex.x+1f,newVertex.yz),uv0))*0.5;
		float depthDeltaZ = (getDisplacement(vec3(newVertex.xy,newVertex.z-1f),uv0)-getDisplacement(vec3(newVertex.xy,newVertex.z+1f),uv0))*0.5;
		
		N = normalize(vec3(depthDeltaX,0.5f,depthDeltaZ))*0.5+0.5;
	#endif
	
	col = color;
	uv = uv0;
}