#version 330
uniform sampler2D colorBuffer;
uniform sampler2D normalBuffer;
uniform sampler2D emissionBuffer;
uniform sampler2D lightingBuffer;
uniform sampler2D depthBuffer;
uniform vec3 ambientColor;

in vec2 screenPos;

out vec4 oColor;

void main()
{
	vec4 color = texture(colorBuffer,screenPos);
	vec3 normal = texture(normalBuffer,screenPos).xyz;
	
	if(normal.x==0f && normal.y==0f && normal.z==0f) {
		oColor = vec4(color.rgb,1f);
		return;
	}
	
	vec4 lighting = texture(lightingBuffer,screenPos)+texture(emissionBuffer,screenPos)+vec4(ambientColor,0.0);
	
	oColor = vec4(color.rgb*lighting.rgb,1.0);
}