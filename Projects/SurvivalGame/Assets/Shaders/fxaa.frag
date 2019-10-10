//From https://github.com/mattdesl/glsl-fxaa/blob/master/fxaa.glsl

#version 330

uniform sampler2D colorBuffer;
uniform sampler2D normalBuffer;
uniform sampler2D positionBuffer;
uniform sampler2D emissionBuffer;
uniform vec3 ambientColor;

uniform int screenWidth;
uniform int screenHeight;

#ifndef FXAA_REDUCE_MIN
    #define FXAA_REDUCE_MIN (1.0/128.0)
#endif
#ifndef FXAA_REDUCE_MUL
    #define FXAA_REDUCE_MUL (1.0/8.0)
#endif
#ifndef FXAA_SPAN_MAX
    #define FXAA_SPAN_MAX 8.0
#endif

vec4 fxaa(sampler2D tex,vec2 fragCoord,vec2 resolution,vec4 rgbaM,vec3 rgbNW,vec3 rgbNE,vec3 rgbSW,vec3 rgbSE)
{
    vec4 color;
    mediump vec2 inverseVP = vec2(1.0/resolution.x,1.0/resolution.y);
    vec3 rgbM = rgbaM.rgb;
    vec3 luma = vec3(0.299,0.587,0.114);
    float lumaNW = dot(rgbNW,luma);
    float lumaNE = dot(rgbNE,luma);
    float lumaSW = dot(rgbSW,luma);
    float lumaSE = dot(rgbSE,luma);
    float lumaM = dot(rgbM,luma);
    float lumaMin = min(lumaM,min(min(lumaNW,lumaNE),min(lumaSW,lumaSE)));
    float lumaMax = max(lumaM,max(max(lumaNW,lumaNE),max(lumaSW,lumaSE)));
    
    mediump vec2 dir;
    dir.x = -((lumaNW+lumaNE)-(lumaSW+lumaSE));
    dir.y = ((lumaNW+lumaSW)-(lumaNE+lumaSE));
    
    float dirReduce = max((lumaNW+lumaNE+lumaSW+lumaSE)*(0.25*FXAA_REDUCE_MUL),FXAA_REDUCE_MIN);
    
    float rcpDirMin = 1.0/(min(abs(dir.x),abs(dir.y))+dirReduce);
    dir = min(vec2(FXAA_SPAN_MAX,FXAA_SPAN_MAX),max(vec2(-FXAA_SPAN_MAX,-FXAA_SPAN_MAX),dir*rcpDirMin))*inverseVP;
    
    vec3 rgbA = 0.5*(texture2D(tex,fragCoord*inverseVP+dir*(1.0/3.0-0.5)).rgb+texture2D(tex,fragCoord*inverseVP+dir*(2.0/3.0-0.5)).rgb);
    vec3 rgbB = rgbA*0.5+0.25*(texture2D(tex,fragCoord*inverseVP+dir*-0.5).rgb+texture2D(tex,fragCoord*inverseVP+dir*0.5).rgb);

    float lumaB = dot(rgbB,luma);
	
    if((lumaB < lumaMin) || (lumaB > lumaMax)) {
        color = vec4(rgbA,rgbaM.a);
	}else{
        color = vec4(rgbB,rgbaM.a);
	}
	
    return color;
}

in vec2 screenPos;
out vec4 oDiffuse;
out vec3 oNormal;
out vec3 oPosition;
out vec4 oEmission;
void main()
{
	vec2 resolution = vec2(screenWidth,screenHeight);
	vec2 inverseVP = 1.0/resolution;
	
	oDiffuse = fxaa(
		colorBuffer, //tex
		screenPos*resolution, //fragCoord
		resolution, //resolution
		texture2D(colorBuffer,screenPos), //v_rgbM
		texture2D(colorBuffer,screenPos+vec2(-inverseVP.x,-inverseVP.y)).rgb, //v_rgbNW
		texture2D(colorBuffer,screenPos+vec2( inverseVP.x,-inverseVP.y)).rgb, //v_rgbNE
		texture2D(colorBuffer,screenPos+vec2(-inverseVP.x, inverseVP.y)).rgb, //v_rgbSW
		texture2D(colorBuffer,screenPos+vec2( inverseVP.x, inverseVP.y)).rgb //v_rgbSE
	);
	
	oNormal = texture2D(normalBuffer,screenPos).xyz;
	oPosition = texture2D(positionBuffer,screenPos).xyz;
	oEmission = texture2D(emissionBuffer,screenPos);
}