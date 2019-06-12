#version 330

const int numColors = 8;
const int gridSnap = 10;

uniform sampler2D positionBuffer;
uniform sampler2D depthBuffer;

uniform float lightRange;
uniform float lightIntensity;
uniform vec3 lightColor;
uniform vec3 lightPosition;

in vec3 vPos;
in vec3 vWorldPos;

out vec4 oLight;

void main()
{
	vec3 worldPosSnapped = vWorldPos; //floor(vWorldPos*gridSnap)/gridSnap;
	
	float intensity = 1f-distance(worldPosSnapped,lightPosition)/(lightRange*0.5f);
	//intensity = round(intensity*numColors)/numColors;
	oLight = vec4(lightColor*lightIntensity,intensity);
}