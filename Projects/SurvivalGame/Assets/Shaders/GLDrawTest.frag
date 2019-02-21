#version 330

uniform sampler2D mainTex;
uniform sampler2D secondTex;

in vec2 uv;
in vec3 col;
out vec4 color;

void main(void)  
{
	color = vec4(mix(texture(mainTex,uv).rgb,texture(secondTex,uv).rgb,0.5f)*col,1f);
}