#version 330

const int numColors = 32;

uniform sampler2D lightingBuffer;
uniform sampler2D emissionBuffer;
uniform vec2 screenResolution;
uniform vec3 ambientColor;
uniform float zoom;

in vec2 screenPos;

out vec3 oLight;

void main()
{
	vec4 lighting = texture2D(lightingBuffer,screenPos);
	vec4 emission = texture2D(emissionBuffer,screenPos); //((screenPos*2f-vec2(1f,1f))/zoom)*0.5f+vec2(0.5f,0.5f));
	
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