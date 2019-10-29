#version 330

uniform vec3 colorTop;
uniform vec3 colorBottom;

in vec2 uv;
in vec3 localPos;
out vec4 oColor;
out vec3 oEmission;

void main (void)  
{
	oColor = vec4(mix(colorBottom,colorTop,min(1.0,localPos.y*4.0)),1.0);
	oEmission = vec3(1f,1f,1f);
}