#version 330

in vec4 vertex;
in vec2 uv0;
out vec2 uv;

void main(void)
{
	gl_Position = vertex;
	uv = uv0;
}