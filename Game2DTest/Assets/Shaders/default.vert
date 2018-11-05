#version 330

uniform mat4 world;
uniform mat4 worldViewProj;

in vec4 vertex;
in vec3 normal;
in vec4 color;
in vec2 uv0;
out vec2 uv;
out vec4 vertexColor;

void main(void)
{
	gl_Position=	worldViewProj*vertex;
	vertexColor=	color;
	uv=		 		uv0;
}