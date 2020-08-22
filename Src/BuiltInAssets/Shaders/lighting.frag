#version 330
 
uniform sampler2D positionBuffer;
uniform sampler2D normalBuffer;
uniform sampler2D specularBuffer;
uniform int screenWidth;
uniform int screenHeight;
uniform float lightIntensity;
uniform vec3 lightColor;
uniform vec3 cameraPosition;
#ifdef POINT
uniform vec3 lightPosition;
uniform float lightRange;
#endif
#ifdef DIRECTIONAL
uniform vec3 lightDirection;
#endif
//uniform vec3 cameraDirection;

out vec4 oColor;

void main()
{
	vec2 screenPos = vec2(
		gl_FragCoord.x/screenWidth,
		gl_FragCoord.y/screenHeight
	);
	
	vec3 surfacePosition = texture(positionBuffer,screenPos).xyz;
	vec3 normal = texture(normalBuffer,screenPos).xyz*2f-1f;
	
	#ifdef DIRECTIONAL
		vec3 lightDir = lightDirection;
	#else
		vec3 lightDir = surfacePosition-lightPosition;
		float distance = length(lightDir);
		
		if(distance>lightRange){
			discard;
		}
		
		lightDir = normalize(lightDir);
	#endif
	
	float intensity;
	
	if(normal.x==0.0 && normal.y==0.0 && normal.z==0.0) {
		#ifdef DIRECTIONAL
			intensity = 1.0;
		#else
			intensity = clamp(1.0-(distance/lightRange),0.0,1.0);
		#endif
	}else{
		float diffuse = clamp(dot(normal,-lightDir),0f,1f);
		
		#ifdef POINT
			diffuse *= clamp(1f-(distance/lightRange),0f,1f);
		#endif
		
		intensity = diffuse;
		
		float specularIntensity = texture(specularBuffer,screenPos).r;
		if(specularIntensity>0.1f) {
			vec3 eyeDirection = normalize(cameraPosition-surfacePosition);
			vec3 reflection = reflect(lightDir,normal);
			
			float specular = pow(max(dot(eyeDirection,reflection),0f),16f);
			
			#ifdef POINT
				specular /= distance*distance;
			#endif
			
			intensity += clamp(specular,0f,1f);
		}
	}
	
	//intensity = max(intensity,clamp(dot(-normal,lightToSurfaceDirection)*(1.0-(distance/lightRange)),0.0,1.0));	//< TODO: MAKE THIS ONLY BE CALLED FOR DUALSIDED SHADERS
	oColor = vec4(lightColor*intensity*lightIntensity,1.0);
}

/*float DepthToLinearZ(float dVal)
{
	return (farClip*nearClip)/(farClip-(dVal*(farClip-nearClip)));
}
vec4 screenToProj(vec2 iCoord)
{
	return vec4(2.0*vec2(iCoord.x,1.0-iCoord.y)-1,0.0,1.0);
}
vec3 depthToPosition(float fZ,vec2 uv)
{
	vec4 pos = projInverse*vec4( uv,fZ,1.0 );
	vec3 output = pos.xyz/pos.w;
	return output;
}
vec3 depthToPosition2(float depth,vec2 uv)
{
	float x = uv.x*2-1;
	float y = (1-uv.y)*2-1;
	vec4 vProjectedPos = vec4(x,y,depth,1.0);
	//Transform by the inverse projection matrix
	vec4 vPositionVS = projInverse*vProjectedPos;  
	//Divide by w to get the view-space position
	return vPositionVS.xyz/vPositionVS.w;
}
float getDepth(vec2 uv)
{
	float farNear = farClip/nearClip;
	return 1.0/(-farNear*texture(depthBuffer,uv).x+farNear);
}*/