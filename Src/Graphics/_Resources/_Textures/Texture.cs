using System;
using Silk.NET.OpenGL;
using static Dissonance.Engine.Graphics.OpenGLApi;

namespace Dissonance.Engine.Graphics
{
	public class Texture : IDisposable
	{
		protected const PixelType DefaultPixelType = PixelType.UnsignedByte;
		protected const PixelFormat DefaultPixelFormat = PixelFormat.Rgba;
		protected const InternalFormat DefaultPixelInternalFormat = InternalFormat.Rgba;

		public static FilterMode DefaultFilterMode { get; set; } = FilterMode.Trilinear;
		public static TextureWrapMode DefaultWrapMode { get; set; } = TextureWrapMode.Repeat;

		public string Name { get; set; } = string.Empty;
		public uint Id { get; protected set; }
		public int Width { get; protected set; }
		public int Height { get; protected set; }
		public PixelType PixelType { get; protected set; }
		public PixelFormat PixelFormat { get; protected set; }
		public InternalFormat PixelInternalFormat { get; protected set; }

		protected FilterMode FilterMode { get; set; }
		protected TextureWrapMode WrapMode { get; set; }
		protected bool UseMipmaps { get; set; }

		public Vector2Int Size => new(Width, Height);

		protected Texture() { }

		public Texture(string name, int width, int height, FilterMode? filterMode = null, TextureWrapMode? wrapMode = null, bool useMipmaps = true, TextureFormat format = TextureFormat.RGBA8)
			: this(width, height, filterMode, wrapMode, useMipmaps, format)
		{
			Name = name;
		}

		public Texture(int width, int height, FilterMode? filterMode = null, TextureWrapMode? wrapMode = null, bool useMipmaps = true, TextureFormat format = TextureFormat.RGBA8)
		{
			Id = OpenGL.GenTexture();
			Width = width;
			Height = height;

			FilterMode = filterMode ?? DefaultFilterMode;
			WrapMode = wrapMode ?? DefaultWrapMode;
			UseMipmaps = useMipmaps;

			(PixelFormat, PixelInternalFormat, PixelType, _) = Rendering.textureFormatInfo[format];

			SetupTexture();
		}

		internal Texture(uint id, int width, int height)
		{
			Id = id;
			Width = width;
			Height = height;
		}

		protected virtual void SetupTexture()
		{
			var pixels = new Pixel[Width * Height];

			Array.Fill(pixels, new Pixel(255, 255, 255, 255));

			SetPixels(pixels);
		}

		public void Dispose()
		{
			OpenGL.DeleteTexture(Id);
			GC.SuppressFinalize(this);
		}

		public unsafe T[,] GetPixels<T>() where T : unmanaged
		{
			var pixels = new T[Width, Height];

			OpenGL.ActiveTexture(TextureUnit.Texture0);
			OpenGL.BindTexture(TextureTarget.Texture2D, Id);

			//TODO: Ensure that this works.
			fixed (T* ptr = &(pixels != null && pixels.Length != 0 ? ref pixels[0, 0] : ref *(T*)null)) {
				OpenGL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat, PixelType, ptr);
			}

			OpenGL.BindTexture(TextureTarget.Texture2D, 0);

			return pixels;
		}

		public unsafe void SetPixels(IntPtr pixels, PixelType pixelType = DefaultPixelType, PixelFormat pixelFormat = DefaultPixelFormat, InternalFormat pixelInternalFormat = DefaultPixelInternalFormat)
		{
			OpenGL.ActiveTexture(TextureUnit.Texture0);
			OpenGL.BindTexture(TextureTarget.Texture2D, Id);

			PixelType = pixelType;
			PixelFormat = pixelFormat;
			PixelInternalFormat = pixelInternalFormat;

			OpenGL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat, (uint)Width, (uint)Height, 0, PixelFormat, PixelType, (void*)pixels);
			
			SetupFiltering(FilterMode, WrapMode, UseMipmaps);
		}

		public unsafe void SetPixels<T>(T[] pixels, PixelType pixelType = DefaultPixelType, PixelFormat pixelFormat = DefaultPixelFormat, InternalFormat pixelInternalFormat = DefaultPixelInternalFormat) where T : unmanaged
		{
			fixed (T* ptr = pixels) {
				SetPixels((IntPtr)ptr, pixelType, pixelFormat, pixelInternalFormat);
			}
		}

		public unsafe void SetPixels<T>(T[,] pixels, PixelType pixelType = DefaultPixelType, PixelFormat pixelFormat = DefaultPixelFormat, InternalFormat pixelInternalFormat = DefaultPixelInternalFormat) where T : unmanaged
		{
			fixed (T* ptr = pixels) {
				SetPixels((IntPtr)ptr, pixelType, pixelFormat, pixelInternalFormat);
			}
		}

		public unsafe void SetPixels<T>(T[,,] pixels, PixelType pixelType = DefaultPixelType, PixelFormat pixelFormat = DefaultPixelFormat, InternalFormat pixelInternalFormat = DefaultPixelInternalFormat) where T : unmanaged
		{
			fixed (T* ptr = pixels) {
				SetPixels((IntPtr)ptr, pixelType, pixelFormat, pixelInternalFormat);
			}
		}

		internal static void SetupFiltering(FilterMode? filterMode = null, TextureWrapMode? wrapMode = null, bool useMipmaps = true)
		{
			wrapMode ??= DefaultWrapMode;
			filterMode ??= DefaultFilterMode;

			uint wrapModeInt = (uint)wrapMode.Value;

			(int magFilter, int minFilter) = filterMode switch {
				FilterMode.Bilinear => ((int)TextureMagFilter.Linear, (int)(useMipmaps ? TextureMinFilter.LinearMipmapNearest : TextureMinFilter.Linear)),
				FilterMode.Trilinear => ((int)TextureMagFilter.Linear, (int)(useMipmaps ? TextureMinFilter.LinearMipmapLinear : TextureMinFilter.Linear)),
				_ => ((int)TextureMagFilter.Nearest, (int)(useMipmaps ? TextureMinFilter.NearestMipmapNearest : TextureMinFilter.Nearest))
			};

			OpenGL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, minFilter);
			OpenGL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, magFilter);
			OpenGL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, wrapModeInt);
			OpenGL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, wrapModeInt);

			if (useMipmaps) {
				OpenGL.GenerateMipmap(TextureTarget.Texture2D);
			}
		}
	}
}
