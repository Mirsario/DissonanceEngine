#version 330

uniform sampler2D mainTex;
//uniform float chunkSize;

in vec2 vUV;

out float oLightingOcclusion;

vec2 Multiply01(vec2 vector,float value)
{
	return vec2(
		((vector.x*2f-1f)*value+1f)*0.5f,
		((vector.y*2f-1f)*value+1f)*0.5f
	);
}

void main (void)  
{
	oLightingOcclusion = texture(mainTex,Multiply01(vUV,64f/66f)).r;
}