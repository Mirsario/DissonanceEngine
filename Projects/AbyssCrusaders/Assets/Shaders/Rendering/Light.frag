#version 330

//const int numColors = 8;
//const int gridSnap = 10;

uniform sampler2D positionBuffer;
uniform sampler2D depthBuffer;

uniform float lightRange;
uniform float lightIntensity;
uniform vec3 lightColor;
uniform vec3 lightPosition;

in vec3 vPos;
in vec3 vWorldPos;

out vec3 oLight;

float Snap(float value,float step)
{
	return floor(value/step)*step;
}
vec2 Snap(vec2 value,float step)
{
	return vec2(
		Snap(value.x,step),
		Snap(value.y,step)
	);
}

void main()
{
	const float Step = 0.1f;
	
	float intensity = max(0f,1f-distance(Snap(vWorldPos.xy,Step),Snap(lightPosition.xy,Step))/(lightRange*0.5f));
	
	intensity = intensity*intensity;
	
	oLight = vec3(lightColor*lightIntensity*intensity);
}