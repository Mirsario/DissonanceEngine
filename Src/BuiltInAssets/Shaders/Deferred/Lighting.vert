#version 330

#ifndef DIRECTIONAL
	uniform mat4 worldViewProj;
#endif

in vec3 vertex;

void main(void)
{
	vec4 pos = vec4(vertex, 1.0);
	
	gl_Position = pos;

	#ifndef DIRECTIONAL
		gl_Position = worldViewProj * gl_Position;
	#endif
}