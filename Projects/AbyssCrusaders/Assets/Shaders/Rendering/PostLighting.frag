#version 330

const int numColors = 16;

uniform sampler2D lightingBuffer;
uniform sampler2D emissionBuffer;
uniform vec2 screenResolution;
uniform vec3 ambientColor;

in vec2 screenPos;

out vec4 oLight;

float normpdf(in float x,in float sigma)
{
	return 0.39894*exp(-0.5*x*x/(sigma*sigma))/sigma;
}
vec4 blurSampler(in sampler2D sampler,in vec2 fragCoord,in vec2 resolution)
{
	//declare stuff
	const int mSize = 8;
	const int kSize = (mSize-1)/2;
	float kernel[mSize];

	const float sigma = 100.0;
	const float sqrSigma = 10000.0;
	for(int i = 0;i<=kSize;i++) {
		kernel[kSize+i] = kernel[kSize-i] = 0.39894*exp(-0.5*float(i*i)/sqrSigma)/sigma;
	}

	float Z = 0.0;
	for(int i = 0;i < mSize;i++) {
		Z += kernel[i];
	}
	
	vec3 final_colour = vec3(0.0);
	for(int x = -kSize;x<=kSize;x++) {
		for (int y = -kSize;y<=kSize;y++) {
			final_colour += kernel[kSize+y]*kernel[kSize+x]*texture(sampler,vec2((fragCoord.x+float(x))/resolution.x,(fragCoord.y+float(y))/resolution.y)).rgb;
		}
	}

	return vec4(final_colour/(Z*Z),1.0);  
}


void main()
{
	vec4 lighting = texture2D(lightingBuffer,screenPos);
	vec4 emission = texture2D(emissionBuffer,screenPos);
	vec4 blurredEmission = blurSampler(emissionBuffer,screenPos*screenResolution,screenResolution);
	emission = vec4(
		max(emission.r,blurredEmission.r),
		max(emission.g,blurredEmission.g),
		max(emission.b,blurredEmission.b),
		1f
	);
	
	oLight = vec4(lighting.rgb+/*emission.rgb+*/ambientColor,lighting.a);
	oLight.rgb = vec3(
		ceil(oLight.r*numColors)/numColors,
		ceil(oLight.g*numColors)/numColors,
		ceil(oLight.b*numColors)/numColors
	);
}