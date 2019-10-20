#version 330

const int numColors = 16;

uniform sampler2D lightingBuffer;
uniform sampler2D terrainLightingDataBuffer;
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
	//vec4 emission = vec4(screenPos,0f,1f); //texture2D(emissionBuffer,screenPos); //vec4(0.5f,0.5f,0.5f,1.0); //
	//vec4 emission = texture2D(emissionBuffer,screenPos);
	//vec4 emission = texture2D(emissionBuffer,screenPos);
	
	/*vec4 blurredEmission = blurSampler(emissionBuffer,uv*screenResolution,screenResolution);
	emission = vec4(
		max(emission.r,blurredEmission.r),
		max(emission.g,blurredEmission.g),
		max(emission.b,blurredEmission.b),
		1f
	);*/
	
	vec4 lightingData = texture2D(terrainLightingDataBuffer,screenPos);
	
	oLight = texture2D(lightingBuffer,screenPos).rgb+lightingData.y;
	oLight *= 1f-lightingData.x;
	oLight = clamp(oLight+ambientColor,0f,1f);
	
	/*oLight.rgb = vec3(
		round(oLight.r*numColors)/numColors,
		round(oLight.g*numColors)/numColors,
		round(oLight.b*numColors)/numColors
	);*/
}