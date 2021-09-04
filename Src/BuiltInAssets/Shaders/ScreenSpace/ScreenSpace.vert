#version 330

in vec4 vertex;
#ifdef UV
in vec2 uv0;
#endif

out vec2 vUV;

void main()
{
	gl_Position = vertex;
	
	#ifdef UV
		vUV = uv0;
	#else
		vec4 pos = vertex;
		
		pos.xyz /= pos.w;
		
		vUV = (pos.xy + vec2(1f, 1f)) / 2f;
	#endif
}
