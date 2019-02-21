using System;
using OpenTK.Graphics.OpenGL;

namespace GameEngine.Graphics
{
	public class RenderTexture : Texture
	{
		//internal FramebufferAttachment attachment;
		//internal Framebuffer framebuffer;
		//internal bool ownsFramebuffer;

		public readonly TextureFormat textureFormat;
		
		public RenderTexture(string name,int width,int height,FilterMode? filterMode = null,TextureWrapMode? wrapMode = null,bool useMipmaps = true,TextureFormat textureFormat = TextureFormat.RGBA8)
		{
			Id = GL.GenTexture();
			Width = width;
			Height = height;
			this.name = name;
			this.textureFormat = textureFormat;

			var (formatGeneral,formatInternal,_,_) = Rendering.textureFormatInfo[textureFormat];
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D,Id);
			GL.TexImage2D(TextureTarget.Texture2D,0,formatInternal,width,height,0,formatGeneral,PixelType.UnsignedByte,IntPtr.Zero);
			SetupFiltering(filterMode,wrapMode,useMipmaps);
		}
		
		public void GenerateMipmaps()
		{
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D,Id);
			GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
		}
		public void Resize(int newWidth,int newHeight)
		{
			var (formatGeneral,formatInternal,_,_) = Rendering.textureFormatInfo[textureFormat];

			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D,Id);
			GL.TexImage2D(TextureTarget.Texture2D,0,formatInternal,newWidth,newHeight,0,formatGeneral,PixelType.UnsignedByte,IntPtr.Zero);
		}
		public override void Dispose()
		{
			GL.DeleteTexture(Id);
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