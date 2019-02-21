#version 330

in vec4 vertex;
in vec2 uv0;
in vec3 color;
out vec2 uv;
out vec3 col;

void main(void)
{
	gl_Position = vertex;
	uv = uv0;
	col = color;
}