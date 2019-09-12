#version 330

uniform sampler2D colorBuffer;
uniform sampler2D emissionBuffer;
uniform sampler2D lightingBuffer;
uniform vec3 ambientColor;
uniform vec2 screenResolution;
uniform vec2 lightingResolution;
uniform vec2 lightingOffset;
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
	//vec2 lightingRes = ceil(screenResolution/zoom);
	vec2 upscaledLightingRes = lightingResolution*zoom;
	vec2 resDiffScale = upscaledLightingRes/screenResolution;
	
	//vec2 cameraTopLeftInPixels = (cameraPos-(screenResolution*0.5f/zoom))*10f;
	//vec2 offset = cameraTopLeftInPixels-floor(cameraTopLeftInPixels);
	//offset /= screenResolution/zoom;
	
	/*vec2 pixelOffset = mod(floor(cameraPos*10f),zoom);
	
	vec2 offset = pixelOffset/screenResolution;
	
	vec2 screenPixelPos = screenPos*screenResolution;
	if(screenPixelPos.x<64 && screenPixelPos.y<64) {
		oColor = vec3(
			float(pixelOffset.x)/zoom,
			0f, //float(pixelOffset.y)/zoom,
			0f
		);
		return;
	}*/
	
	vec3 lighting = clamp(texture2D(lightingBuffer,screenPos).rgb + texture2D(emissionBuffer,screenPos).rgb,0f,1f);
	
	vec4 color = texture2D(colorBuffer,screenPos);
	oColor = vec3(color.rgb*lighting);
}