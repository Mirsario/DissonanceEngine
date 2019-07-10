using System;
using OpenTK.Graphics.OpenGL;

namespace GameEngine.Graphics
{
	public class RenderTexture : Texture
	{
		//internal FramebufferAttachment attachment;
		//internal Framebuffer framebuffer;
		//internal bool ownsFramebuffer;
		public readonly Func<Vector2Int> TargetSize;
		public readonly TextureFormat TextureFormat;
		
		public RenderTexture(string name,Func<Vector2Int> targetSize,FilterMode? filterMode = null,TextureWrapMode? wrapMode = null,bool useMipmaps = true,TextureFormat textureFormat = TextureFormat.RGBA8)
			: this(name,targetSize().x,targetSize().y,filterMode,wrapMode,useMipmaps,textureFormat)
		{
			TargetSize = targetSize;
		}
		public RenderTexture(string name,int width,int height,FilterMode? filterMode = null,TextureWrapMode? wrapMode = null,bool useMipmaps = true,TextureFormat textureFormat = TextureFormat.RGBA8)
		{
			Id = GL.GenTexture();
			Width = width;
			Height = height;
			TextureFormat = textureFormat;

			this.name = name;
			this.filterMode = filterMode ?? defaultFilterMode;
			this.wrapMode = wrapMode ?? defaultWrapMode;
			this.useMipmaps = useMipmaps;

			Debug.Log($"Created [{Width},{Height}] RenderTexture");

			SetupTexture();
		}
		
		public void GenerateMipmaps()
		{
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D,Id);
			GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
		}
		public void Resize(int width,int height)
		{
			Width = width;
			Height = height;

			Debug.Log($"Resizing to [{width},{height}]");

			SetupTexture();
		}
		public override void Dispose()
		{
			GL.DeleteTexture(Id);
		}

		internal bool UpdateSize()
		{
			if(TargetSize==null) {
				return false;
			}

			var vec = TargetSize();

			if(vec.x<=0) {
				throw new InvalidOperationException("Texture's width can't equal or be less than zero.");
			}
			if(vec.y<=0) {
				throw new InvalidOperationException("Texture's height can't equal or be less than zero.");
			}

			if(vec.x==Width && vec.y==Height) {
				return false;
			}

			Resize(vec.x,vec.y);

			return true;
		}

		private void SetupTexture()
		{
			var (formatGeneral,formatInternal,_,_) = Rendering.textureFormatInfo[TextureFormat];

			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D,Id);
			GL.TexImage2D(TextureTarget.Texture2D,0,formatInternal,Width,Height,0,formatGeneral,PixelType.UnsignedByte,IntPtr.Zero);

			SetupFiltering(null,null,false);
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