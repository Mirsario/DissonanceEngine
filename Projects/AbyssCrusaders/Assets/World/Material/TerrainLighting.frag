#version 330

uniform sampler2D occlusionMap;
uniform sampler2D emissionMap;

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
	vec2 uv = Multiply01(vUV,64f/66f);
	
	oLightingData = vec2(
		texture(occlusionMap,uv).r,
		texture(emissionMap,uv).r
	);
}