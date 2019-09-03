#version 330

const int numColors = 32;

uniform sampler2D lightingBuffer;
uniform sampler2D emissionBuffer;
uniform vec2 screenResolution;
uniform vec3 ambientColor;
uniform float zoom;

in vec2 screenPos;

out vec3 oLight;

vec2 Divide01Vec(vec2 vector,float value)
{
	return vec2(
		((vector.x*2f-1f)/value+1f)*0.5f,
		((vector.y*2f-1f)/value+1f)*0.5f
	);
}

void main()
{
	vec4 lighting = texture2D(lightingBuffer,screenPos);
	//vec4 emission = vec4(screenPos,0f,1f); //texture2D(emissionBuffer,screenPos); //vec4(0.5f,0.5f,0.5f,1.0); //
	//vec4 emission = texture2D(emissionBuffer,screenPos);
	vec4 emission = texture2D(emissionBuffer,screenPos);
	
	/*vec4 blurredEmission = blurSampler(emissionBuffer,uv*screenResolution,screenResolution);
	emission = vec4(
		max(emission.r,blurredEmission.r),
		max(emission.g,blurredEmission.g),
		max(emission.b,blurredEmission.b),
		1f
	);*/
	
	oLight = (emission.a==-1f ? lighting.rgb*emission.rgb : clamp(lighting.rgb+emission.rgb,0f,1f))+ambientColor;
	
	/*oLight.rgb = vec3(
		ceil(oLight.r*numColors)/numColors,
		ceil(oLight.g*numColors)/numColors,
		ceil(oLight.b*numColors)/numColors
	);*/
}