#version 330

uniform sampler2D mainTex;
uniform float resolutionScale;

in vec2 vUV;

out vec2 oLightingData;

vec2 Multiply01(vec2 vector,float value)
{
	return vec2(
		((vector.x*2f-1f)*value+1f)*0.5f,
		((vector.y*2f-1f)*value+1f)*0.5f
	);
}

void main (void)
{
	vec2 uv = Multiply01(vUV,resolutionScale);
	
	oLightingData = vec2(texture(mainTex,uv).xy);
}