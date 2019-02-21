#version 330

uniform mat4 world;
uniform mat4 worldViewProj;

in vec4 vertex;
in vec3 normal;
in vec4 color;
in vec2 uv0;

out vec3 vPos;
out vec3 vWorldPos;

void main(void)
{
	vPos = vertex.xyz;
	vWorldPos = (world*vertex).xyz;
	gl_Position = worldViewProj*vertex;
}