#version 330

const int numColors = 16;

uniform sampler2D lightingBuffer;
uniform sampler2D emissionBuffer;
uniform vec3 ambientColor;

in vec2 screenPos;

out vec4 oLight;

void main()
{
	vec4 lighting = texture2D(lightingBuffer,screenPos);
	vec4 emission = texture2D(emissionBuffer,screenPos);
	oLight = vec4(lighting.rgb+emission.rgb+ambientColor,lighting.a);
	oLight.rgb = vec3(
		ceil(oLight.r*numColors)/numColors,
		ceil(oLight.g*numColors)/numColors,
		ceil(oLight.b*numColors)/numColors
	);
}