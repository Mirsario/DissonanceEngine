#version 330

uniform mat4 viewProj;

in vec3 vertex;
in vec4 color;

out vec4 vColor;

void main(void)
{
	vColor = color;
	gl_Position = viewProj * vec4(vertex, 1.0);
}
