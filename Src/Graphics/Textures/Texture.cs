using System;
using Dissonance.Engine.Graphics.Enums;
using Dissonance.Engine.IO;
using Dissonance.Engine.Structures;
using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics.Textures
{
	public class Texture : Asset
	{
		protected const PixelType DefaultPixelType = PixelType.UnsignedByte;
		protected const PixelFormat DefaultPixelFormat = PixelFormat.Rgba;
		protected const PixelInternalFormat DefaultPixelInternalFormat = PixelInternalFormat.Rgba;

		public static FilterMode defaultFilterMode = FilterMode.Trilinear;
		public static TextureWrapMode defaultWrapMode = TextureWrapMode.Repeat;

		public string name = "";

		protected FilterMode filterMode;
		protected TextureWrapMode wrapMode;
		protected bool useMipmaps;

		public uint Id { get; protected set; }
		public int Width { get; protected set; }
		public int Height { get; protected set; }
		public PixelType PixelType { get; protected set; }
		public PixelFormat PixelFormat { get; protected set; }
		public PixelInternalFormat PixelInternalFormat { get; protected set; }

		public Vector2Int Size => new Vector2Int(Width,Height);

		protected Texture() { }

		public Texture(string name,int width,int height,FilterMode? filterMode = null,TextureWrapMode? wrapMode = null,bool useMipmaps = true,TextureFormat format = TextureFormat.RGBA8)
			: this(width,height,filterMode,wrapMode,useMipmaps,format)
		{
			this.name = name;
		}
		public Texture(int width,int height,FilterMode? filterMode = null,TextureWrapMode? wrapMode = null,bool useMipmaps = true,TextureFormat format = TextureFormat.RGBA8)
		{
			Id = GL.GenTexture();
			Width = width;
			Height = height;

			this.filterMode = filterMode ?? defaultFilterMode;
			this.wrapMode = wrapMode ?? defaultWrapMode;
			this.useMipmaps = useMipmaps;

			var fillColor = new Pixel(255,255,255,255);

			int length = width*height;

			var pixels = new Pixel[length];

			for(int i = 0;i<length;i++) {
				pixels[i] = fillColor;
			}

			SetPixels(pixels);
		}

		internal Texture(uint id,int width,int height)
		{
			Id = id;
			Width = width;
			Height = height;
		}

		public override void Dispose() => GL.DeleteTexture(Id);

		public T[,] GetPixels<T>() where T : unmanaged
		{
			var pixels = new T[Width,Height];

			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D,Id);

			//TODO: Ensure that this works.
			unsafe {
				fixed(T* ptr = &(pixels!=null && pixels.Length!=0 ? ref pixels[0,0] : ref *(T*)null)) {
					GL.GetTexImage(TextureTarget.Texture2D,0,PixelFormat,PixelType,(IntPtr)ptr);
				}
			}

			GL.BindTexture(TextureTarget.Texture2D,0);

			return pixels;
		}

		public void SetPixels(IntPtr pixels,PixelType pixelType = DefaultPixelType,PixelFormat pixelFormat = DefaultPixelFormat,PixelInternalFormat pixelInternalFormat = DefaultPixelInternalFormat)
			=> SetPixelsInternal(() => GL.TexImage2D(TextureTarget.Texture2D,0,PixelInternalFormat,Width,Height,0,PixelFormat,PixelType,pixels),pixelType,pixelFormat,pixelInternalFormat);

		public void SetPixels<T>(T[] pixels,PixelType pixelType = DefaultPixelType,PixelFormat pixelFormat = DefaultPixelFormat,PixelInternalFormat pixelInternalFormat = DefaultPixelInternalFormat) where T : unmanaged
			=> SetPixelsInternal(() => GL.TexImage2D(TextureTarget.Texture2D,0,PixelInternalFormat,Width,Height,0,PixelFormat,PixelType,pixels),pixelType,pixelFormat,pixelInternalFormat);

		public void SetPixels<T>(T[,] pixels,PixelType pixelType = DefaultPixelType,PixelFormat pixelFormat = DefaultPixelFormat,PixelInternalFormat pixelInternalFormat = DefaultPixelInternalFormat) where T : unmanaged
			=> SetPixelsInternal(() => GL.TexImage2D(TextureTarget.Texture2D,0,PixelInternalFormat,Width,Height,0,PixelFormat,PixelType,pixels),pixelType,pixelFormat,pixelInternalFormat);

		public void SetPixels<T>(T[,,] pixels,PixelType pixelType = DefaultPixelType,PixelFormat pixelFormat = DefaultPixelFormat,PixelInternalFormat pixelInternalFormat = DefaultPixelInternalFormat) where T : unmanaged
			=> SetPixelsInternal(() => GL.TexImage2D(TextureTarget.Texture2D,0,PixelInternalFormat,Width,Height,0,PixelFormat,PixelType,pixels),pixelType,pixelFormat,pixelInternalFormat);

		protected void SetPixelsInternal(Action setter,PixelType pixelType,PixelFormat pixelFormat,PixelInternalFormat pixelInternalFormat)
		{
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D,Id);

			PixelType = pixelType;
			PixelFormat = pixelFormat;
			PixelInternalFormat = pixelInternalFormat;

			setter();

			SetupFiltering(filterMode,wrapMode,useMipmaps);
		}

		internal static void SetupFiltering(FilterMode? filterMode = null,TextureWrapMode? wrapMode = null,bool useMipmaps = true)
		{
			wrapMode ??= defaultWrapMode;
			filterMode ??= defaultFilterMode;

			uint wrapModeInt = wrapMode==TextureWrapMode.Repeat ? GLConstants.REPEAT : GLConstants.CLAMP_TO_EDGE;

			(int magFilter, int minFilter) = filterMode switch
			{
				FilterMode.Bilinear => ((int)TextureMagFilter.Linear, (int)TextureMinFilter.Linear),
				FilterMode.Trilinear => ((int)TextureMagFilter.Linear, (int)TextureMinFilter.LinearMipmapLinear),
				_ => ((int)TextureMagFilter.Nearest, (int)(useMipmaps ? TextureMinFilter.NearestMipmapNearest : TextureMinFilter.Nearest))
			};

			GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureMinFilter,minFilter);
			GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureMagFilter,magFilter);
			GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureWrapS,wrapModeInt);
			GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureWrapT,wrapModeInt);

			if(useMipmaps) {
				GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
			}
		}
	}
}