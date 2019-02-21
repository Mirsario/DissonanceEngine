#version 330

uniform mat4 worldViewProj;

in vec4 vertex;
in vec3 normal;
in vec4 color;
in vec2 uv0;

out vec2 vUV;

void main(void)
{
	vUV = uv0;
	gl_Position = worldViewProj*vertex;
}