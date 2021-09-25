#version 330

in vec4 vertex;
#ifdef UV
in vec2 uv0;
#endif

out vec2 screenPos;

void main()
{
	gl_Position = vertex;
	
	#ifdef UV
		screenPos = uv0;
	#else
		vec4 pos = vertex;
		
		pos.xyz /= pos.w;
		
		screenPos = (pos.xy + vec2(1.0, 1.0)) / 2.0;
	#endif
}
