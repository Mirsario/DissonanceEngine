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

out vec3 oDiffuse;
out vec3 oSpecular;

void main()
{
	vec2 screenPos = vec2(
		gl_FragCoord.x / screenWidth,
		gl_FragCoord.y / screenHeight
	);
	
	vec3 surfacePosition = texture(positionBuffer, screenPos).xyz;
	vec3 normal = texture(normalBuffer, screenPos).xyz * 2f - 1f;
	
	#ifdef DIRECTIONAL
		vec3 lightDir = lightDirection;
	#else
		vec3 lightDir = surfacePosition - lightPosition;
		float distance = length(lightDir);
		
		if(distance > lightRange){
			discard;
		}
		
		lightDir = normalize(lightDir);
	#endif
	
	float diffuse;
	
	if(normal.x == 0.0 && normal.y == 0.0 && normal.z == 0.0) {
		#ifdef DIRECTIONAL
			diffuse = 1.0;
		#else
			diffuse = clamp(1.0 - (distance / lightRange), 0.0, 1.0);
		#endif
	} else {
		diffuse = clamp(dot(normal, -lightDir), 0f, 1f);
		
		#ifdef POINT
			diffuse *= clamp(1f - (distance / lightRange), 0f, 1f);
		#endif
		
		float specularIntensity = texture(specularBuffer, screenPos).r;

		if(specularIntensity > 0.0f) {
			vec3 eyeDirection = normalize(cameraPosition - surfacePosition);
			vec3 reflection = reflect(lightDir, normal);
			float specular = pow(max(dot(eyeDirection, reflection), 0f), 16f);
			
			#ifdef POINT
				specular /= distance * distance;
			#endif
			
			oSpecular = lightColor * specular * specularIntensity * lightIntensity;
		}
	}

	oDiffuse = lightColor * diffuse * lightIntensity;
}
