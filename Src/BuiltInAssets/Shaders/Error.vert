#version 330

uniform mat4 worldViewProj;
	
in vec4 vertex;

void main(void)
{
	gl_Position = worldViewProj * vertex;
}