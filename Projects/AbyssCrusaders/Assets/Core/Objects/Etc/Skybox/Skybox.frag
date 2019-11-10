#version 330

uniform vec3 colorTop;
uniform vec3 colorBottom;

in vec2 uv;
in vec3 localPos;
in vec3 worldPos;
out vec4 oColor;
out vec3 oEmission;

void main (void)  
{
	float step = clamp((worldPos.y+465f)/50f,0f,1f);
	
	oColor = vec4(mix(colorBottom,colorTop,step),1.0);
	oEmission = vec3(1f,1f,1f);
}