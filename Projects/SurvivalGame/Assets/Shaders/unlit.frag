#version 330

uniform sampler2D mainTex;

in vec2 uv;
in vec4 col;
in vec3 worldPos;
out vec4 oDiffuse;
out vec3 oNormal;
out vec3 oPosition;
out vec4 oEmission;

void main (void)  
{
	oDiffuse=	texture(mainTex,uv)*col;
	oNormal=	vec3(1.0,1.0,1.0);
	oPosition=	worldPos;
	oEmission=	vec4(1.0,1.0,1.0,1.0);
}