#version 330 
 
uniform sampler2D depthBuffer;
uniform sampler2D normalBuffer;
uniform sampler2D positionBuffer;
uniform vec3 cameraPosition;
uniform float farClip;
uniform float nearClip;
uniform int screenWidth;
uniform int screenHeight;

in vec4 vert;
out vec4 oColor;

void main()
{
	vec2 screenPos = vec2(gl_FragCoord.x/screenWidth,gl_FragCoord.y/screenHeight);
	
	vec3 iNormal = texture(normalBuffer,screenPos).xyz;
	
	if(iNormal.x==0f && iNormal.y==0f && iNormal.z==0f) { //Rather idiotically discard the skybox. Would be more sane to use stencils instead later on.
		discard;
	}
	
	vec3 iPosition = texture(positionBuffer,screenPos).xyz;
	//vec4 iEmission = texture(emissionBuffer,screenPos).rgba;
	
	float distance = distance(iPosition,cameraPosition);
	
	float fogStart = 0f;
	float fogEnd = 256f;
	float fogFactor = 1f-(clamp((fogEnd-distance)/(fogEnd-fogStart),0f,1f));
	//fogFactor = max(0f,fogFactor-((iEmission.r+iEmission.g+iEmission.b)/3f));
	
	oColor = vec4(0.701,0.807,0.843,fogFactor);
}
