#version 330

uniform sampler2D mainTex;
uniform vec4 color = vec4(1f,1f,1f,1f);

in vec2 iUv0;

out vec4 oColor;

void main()  
{
	oColor = texture(mainTex,iUv0);
	oColor *= color;
}