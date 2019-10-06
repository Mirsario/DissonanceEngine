#version 330 
 
uniform sampler2D depthBuffer;
uniform sampler2D colorBuffer;
uniform sampler2D normalBuffer;
uniform sampler2D positionBuffer;
uniform sampler2D emissionBuffer;
uniform sampler2D specularBuffer;
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
	
	//float depth = texture(depthBuffer,screenPos).r;
	//depth = 1f-((depth-nearClip)/(farClip-nearClip));
	
	vec4 iEmission = texture(emissionBuffer,screenPos).rgba;
	
	if(iEmission.a==1f) { //Rather idiotically discard the skybox. Would be more sane to use stencils instead later on.
		discard;
	}
	
	vec3 iPosition = texture(positionBuffer,screenPos).xyz;
	
	float distance = distance(iPosition,cameraPosition);
	
	float fogStart = 0f;
	float fogEnd = 256f;
	float fogFactor = 1f-(clamp((fogEnd-distance)/(fogEnd-fogStart),0f,1f));
	//fogFactor = max(0f,fogFactor-((iEmission.r+iEmission.g+iEmission.b)/3f));
	
	//oDiffuse = vec4(distance,distance,distance,1f); //texture(colorBuffer,screenPos).rgba;
	//oDiffuse = vec4(texture(depthBuffer,screenPos).rgb,1f);
	oColor = vec4(0.701,0.807,0.843,fogFactor);
	
	//if(depth>0.99f) {
	//	oDiffuse = vec4(1f,0.5f,0.5f,1f);
	//}
}
