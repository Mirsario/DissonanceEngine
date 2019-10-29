#version 330

uniform mat4 world;
uniform mat4 worldViewProj;

in vec4 vertex;
in vec2 uv0;
out vec2 uv;
out vec3 localPos;
//out vec3 worldPos;

void main(void)
{
	gl_Position = worldViewProj*vertex;
	localPos = vertex.xyz;
	uv = uv0;
	//worldPos = (world*vertex).xyz;
}