using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using ImagingPixelFormat = System.Drawing.Imaging.PixelFormat;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace GameEngine
{
	public class RenderTexture : Texture
	{
		internal FramebufferAttachment attachment;
		internal Framebuffer framebuffer;
		internal bool ownsFramebuffer;
		
		public RenderTexture(string name,int width,int height,FilterMode? filterMode = null,TextureWrapMode? wrapMode = null,bool useMipmaps = true,TextureFormat textureFormat = TextureFormat.RGBA8)
		: this(name,width,height,null,filterMode,wrapMode,useMipmaps,textureFormat) { }
		public RenderTexture(int width,int height,FilterMode? filterMode = null,TextureWrapMode? wrapMode = null,bool useMipmaps = true,TextureFormat textureFormat = TextureFormat.RGBA8)
		: this("RenderTexture",width,height,null,filterMode,wrapMode,useMipmaps,textureFormat) { }
		
		internal RenderTexture(string name,int width,int height,Framebuffer framebuffer,FilterMode? filterMode = null,TextureWrapMode? wrapMode = null,bool useMipmaps = false,TextureFormat textureFormat = TextureFormat.RGBA8,bool fbBinded = false,FramebufferAttachment? attachmentType = null)
		{
			Graphics.CheckGLErrors();
			Id = GL.GenTexture();
			Width = width;
			Height = height;
			this.name = name;

			/*var fillColor = new Pixel(255,255,255,255);
			var pixels = new Pixel[width,height];
			for(int y=0;y<height;y++) {
				for(int x=0;x<width;x++) {
					pixels[x,y] = fillColor;
				}
			}*/

			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D,Id);
			var (formatGeneral,formatInternal,pixelType,dataType)=	Graphics.textureFormatInfo[textureFormat];
			GL.TexImage2D(TextureTarget.Texture2D,0,formatInternal,width,height,0,formatGeneral,PixelType.UnsignedByte,IntPtr.Zero);
			SetupFiltering(filterMode,wrapMode,useMipmaps);//mipmaps not supported temporarily
			/*GL.TexImage2D(TextureTarget.Texture2D,0,formatInternal,width,height,0,formatGeneral,PixelType.UnsignedByte,IntPtr.Zero);
			GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureMinFilter,	(int)TextureMinFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureMagFilter,	(int)TextureMagFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureWrapS,		(int)OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToBorder);
			GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureWrapT,		(int)OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToBorder);*/
			
			if(ownsFramebuffer = framebuffer==null) {
				framebuffer = new Framebuffer($"RenderTexture_{name}");
			}
			this.framebuffer = framebuffer;
			attachment = attachmentType ?? FramebufferAttachment.ColorAttachment0;

			var prevFb = Framebuffer.activeBuffer;
			bool modifyFb = ownsFramebuffer && !fbBinded;
			if(modifyFb) {
				Framebuffer.Bind(framebuffer);
			}
			GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt,attachment,TextureTarget.Texture2D,Id,0);
			if(modifyFb) {
				Framebuffer.Bind(prevFb);
			}
			//Debug.Log($"RT {name}: ownsFramebuffer: {ownsFramebuffer},modifyFb: {modifyFb}");
			Graphics.CheckGLErrors();
		}

		public void GenerateMipmaps()
		{
			//var prevFb = Framebuffer.activeBuffer;
			//Framebuffer.Bind(framebuffer);
			//for(int i=0;i<5;i++) {
				//GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt,attachment,TextureTarget.Texture2D,Id,i);
				GL.ActiveTexture(TextureUnit.Texture0);
				GL.BindTexture(TextureTarget.Texture2D,Id);
				GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
			//}
			//Framebuffer.Bind(prevFb);
		}

		public override void Dispose()
		{
			base.Dispose();
			if(ownsFramebuffer) {
				framebuffer.Dispose();
			}
		}

		/*public RenderTexture(int width,int height,PixelInternalFormat internalFormat = PixelInternalFormat.Rgba32f,PixelFormat pixelFormat = PixelFormat.Rgba,PixelType pixelType = PixelType.UnsignedByte)
		{
			GL.GenTextures(1,out int id);
			Id = id;
			this.Width = width;
			this.Height = height;
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D,id);
			GL.TexImage2D(TextureTarget.Texture2D,0,internalFormat,width,height,0,pixelFormat,pixelType,IntPtr.Zero);
			GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureMinFilter,	(int)TextureMinFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureMagFilter,	(int)TextureMagFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureWrapS,		(int)OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToBorder);
			GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureWrapT,		(int)OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToBorder);
		}*/
	}
}