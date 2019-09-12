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

void main()
{
	float intensity = max(0f,1f-distance(vWorldPos,lightPosition)/(lightRange*0.5f));
	intensity = intensity*intensity;
	intensity = 1f;
	oLight = vec3(lightColor*lightIntensity*intensity);
}