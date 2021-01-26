#version 330

in vec4 vertex;
in vec2 uv0;

out vec2 iUv0;

void main()
{
	gl_Position = vec4(vertex.x * 2.0 - 1.0, vertex.y * 2.0 - 1.0, vertex.zw);
	
	iUv0 = uv0;
}