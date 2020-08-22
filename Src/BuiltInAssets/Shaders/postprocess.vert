#version 330

in vec4 vertex;

out vec2 screenPos;

void main()
{
	vec4 pos = vertex;
	
	gl_Position = vertex;
	
	pos.xyz /= pos.w;
	
	screenPos = (pos.xy+vec2(1f,1f))/2f;
}
