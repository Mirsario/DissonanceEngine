#version 330

#ifdef TEXTURE
uniform sampler2D mainTex;
#endif
#ifdef COLOR
uniform vec4 color = vec4(1f,1f,1f,1f);
#endif

#ifdef TEXTURE
in vec2 vUV;
#endif
#ifdef VERTEXCOLOR
in vec4 vColor;
#endif

out vec4 oDiffuse;

void main (void)  
{
	//Color
	#ifdef TEXTURE
		#ifdef COLOR
			#ifdef VERTEXCOLOR
				oDiffuse = texture(mainTex,vUV)*color*vColor;
			#else
				oDiffuse = texture(mainTex,vUV)*color;
			#endif
		#else
			#ifdef VERTEXCOLOR
				oDiffuse = texture(mainTex,vUV)*vColor;
			#else
				oDiffuse = texture(mainTex,vUV);
			#endif
		#endif
	#else
		#ifdef COLOR
			#ifdef VERTEXCOLOR
				oDiffuse = color*vColor;
			#else
				oDiffuse = color;
			#endif
		#else
			#ifdef VERTEXCOLOR
				oDiffuse = vColor;
			#endif
		#endif
	#endif
}