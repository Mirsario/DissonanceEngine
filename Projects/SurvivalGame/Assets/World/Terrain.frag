#version 330

uniform sampler2D tileMap;
uniform sampler2D tileAtlas;
uniform int chunkSize = 16;
uniform vec4[30] tileUVs;

in vec2 uv;
out vec4 color;

float lerp(float a,float b,float time)
{
	return a+(b-a)*time;
}

int GetIdAt(vec2 pos)
{
	return int(texture(tileMap,pos).r*255f);
}

void main(void)  
{
	vec2 uvTile = uv*chunkSize;
	uvTile -= floor(uvTile);
	
	int texSize = chunkSize+2;
	float onePixel = 1f/texSize;
	vec2 fixedUv = (uv*(float(chunkSize)/texSize))+vec2(1f/texSize,1f/texSize);
	vec4 checkRect = vec4(
		uvTile.x<=0.5f ? fixedUv.x-onePixel : fixedUv.x,
		uvTile.y<=0.5f ? fixedUv.y-onePixel : fixedUv.y,
		uvTile.x<=0.5f ? fixedUv.x : fixedUv.x+onePixel,
		uvTile.y<=0.5f ? fixedUv.y : fixedUv.y+onePixel
	);
	ivec4 ids = ivec4(
		GetIdAt(vec2(checkRect.x,checkRect.y)),
		GetIdAt(vec2(checkRect.z,checkRect.y)),
		GetIdAt(vec2(checkRect.x,checkRect.w)),
		GetIdAt(vec2(checkRect.z,checkRect.w))
	);
	vec4 idUV = tileUVs[ids[0]];
		vec4 cTopLeft = texture(tileAtlas,vec2(lerp(idUV.x,idUV.z,uvTile.x),lerp(idUV.y,idUV.w,uvTile.y)));
	idUV = tileUVs[ids[1]];
		vec4 cTopRight = texture(tileAtlas,vec2(lerp(idUV.x,idUV.z,uvTile.x),lerp(idUV.y,idUV.w,uvTile.y)));
	idUV = tileUVs[ids[2]];
		vec4 cBottomLeft = texture(tileAtlas,vec2(lerp(idUV.x,idUV.z,uvTile.x),lerp(idUV.y,idUV.w,uvTile.y)));
	idUV = tileUVs[ids[3]];
		vec4 cBottomRight = texture(tileAtlas,vec2(lerp(idUV.x,idUV.z,uvTile.x),lerp(idUV.y,idUV.w,uvTile.y)));
	
	vec2 lerpTime = vec2(
		uvTile.x<=0.5f ? uvTile.x+0.5f : uvTile.x-0.5f,
		uvTile.y<=0.5f ? uvTile.y+0.5f : uvTile.y-0.5f
	);
	
	color = mix(mix(cTopLeft,cTopRight,lerpTime.x),mix(cBottomLeft,cBottomRight,lerpTime.x),lerpTime.y);
}