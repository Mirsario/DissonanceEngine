#version 330
uniform sampler2D colorBuffer;
uniform sampler2D lightingBuffer;
uniform vec3 ambientColor;

in vec2 screenPos;
out vec4 oColor;
void main()
{
	vec4 color = texture2D(colorBuffer,screenPos);
	vec4 lighting = texture2D(lightingBuffer,screenPos/4f);
	oColor = vec4(color.rgb*lighting.rgb,1.0);
}