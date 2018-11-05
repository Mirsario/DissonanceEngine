using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using ImagingPixelFormat = System.Drawing.Imaging.PixelFormat;
using PrimitiveTypeGL = OpenTK.Graphics.OpenGL.PrimitiveType;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace GameEngine
{
	internal class PostProcessPass : RenderPass
	{
		public override string Id => "postprocess";
		
		public override void Render()
		{
			GL.Viewport(0,0,Graphics.ScreenWidth,Graphics.ScreenHeight);
			Framebuffer.Bind(framebuffer);
			if(framebuffer!=null) {
				GL.DrawBuffers(framebuffer.drawBuffers.Length,framebuffer.drawBuffers);
			}//else{
			//GL.DrawBuffers(1,nullDrawBuffers);
			//GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
			//}

			Shader.SetShader(passShader);//Render composite
			OpenTK.Vector3 ambientCol = Graphics.ambientColor;
			GL.Uniform3(GL.GetUniformLocation(passShader.program,"ambientColor"),ref ambientCol);
			
			for(int i=0;i<textures.Length;i++) {
				GL.ActiveTexture((TextureUnit)((int)TextureUnit.Texture0+i));
				GL.BindTexture(TextureTarget.Texture2D,textures[i].Id);
				if(passShader!=null) {
					GL.Uniform1(GL.GetUniformLocation(passShader.program,textures[i].name),i);
				}
			}
			GL.Begin(PrimitiveTypeGL.Quads);
			GL.Vertex2(	-1.0f,1.0f);
			GL.TexCoord2(0.0f,1.0f);
			GL.Vertex2(	-1.0f,-1.0f);
			GL.TexCoord2(0.0f,0.0f);
			GL.Vertex2(	1.0f,-1.0f);
			GL.TexCoord2(1.0f,0.0f);
			GL.Vertex2(	1.0f,1.0f);
			GL.TexCoord2(1.0f,1.0f);
			GL.End();
		}
	}
}