#version 330

uniform sampler2D mainTex;
uniform vec3 colorTop;
uniform vec3 colorBottom;

in vec2 uv;
in vec3 localPos;
in vec3 worldPos;
out vec4 oDiffuse;
out vec3 oNormal;
out vec3 oPosition;
out vec4 oEmission;

void main (void)  
{
	oDiffuse = vec4(mix(colorBottom,colorTop,min(1.0f,localPos.y*2.0)),1.0f);//*texture(mainTex,uv);
	oNormal = vec3(0f,0f,0f);
	oPosition = worldPos;
	oEmission = vec4(1.0,1.0,1.0,1.0);
}