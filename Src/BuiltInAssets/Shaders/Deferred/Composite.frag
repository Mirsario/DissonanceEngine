#version 330

uniform sampler2D colorBuffer;
uniform sampler2D normalBuffer;
uniform sampler2D emissionBuffer;
uniform sampler2D lightingDiffuseBuffer;
uniform sampler2D lightingSpecularBuffer;
uniform vec3 ambientColor;

in vec2 screenPos;

layout(location = 0) out vec4 oColor;

void main()
{
	vec4 color = texture(colorBuffer, screenPos);
	vec3 normal = texture(normalBuffer, screenPos).xyz;
	
	// For now, [0,0,0] normals mean that the pixel should be unlit.
	//TODO: Use stencils for this instead?
	if (normal.x == 0f && normal.y == 0f && normal.z == 0f) {
		oColor = vec4(color.rgb, 1f);

		return;
	}
	
	vec3 multiplicativeLighting = ambientColor + texture(lightingDiffuseBuffer, screenPos).rgb;
	vec3 additiveLighting = texture(lightingSpecularBuffer, screenPos).rgb + texture(emissionBuffer, screenPos).rgb;
	
	oColor = vec4((color.rgb * multiplicativeLighting) + additiveLighting, 1.0);
}