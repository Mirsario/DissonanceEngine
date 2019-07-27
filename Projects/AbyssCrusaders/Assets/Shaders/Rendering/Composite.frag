#version 330
uniform sampler2D colorBuffer;
uniform sampler2D emissionBuffer;
uniform sampler2D lightingBuffer;
uniform vec3 ambientColor;
uniform vec2 screenResolution;
uniform vec2 cameraPos;
uniform float zoom;

in vec2 screenPos;
out vec3 oColor;

/*vec4 blurSampler(in sampler2D sampler,in vec2 fragCoord,in vec2 resolution)
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
}*/

void main()
{
	vec2 cameraTopLeftInPixels = (cameraPos-(screenResolution*0.5f/zoom))*10f;
	vec2 offset = cameraTopLeftInPixels-floor(cameraTopLeftInPixels);
	vec4 lighting = texture2D(lightingBuffer,screenPos+offset/(screenResolution/zoom));
	
	vec4 color = texture2D(colorBuffer,screenPos);
	oColor = vec3(color.rgb*lighting.rgb);
}