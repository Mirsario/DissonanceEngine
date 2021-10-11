using System;
using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics
{
	public class RenderTexture : Texture
	{
		public readonly Func<Vector2Int> TargetSize;
		public readonly TextureFormat TextureFormat;

		public RenderTexture(string name, Func<Vector2Int> targetSize, FilterMode? filterMode = null, TextureWrapMode? wrapMode = null, bool useMipmaps = true, TextureFormat textureFormat = TextureFormat.RGBA8)
			: this(name, targetSize().X, targetSize().Y, filterMode, wrapMode, useMipmaps, textureFormat)
		{
			TargetSize = targetSize;
		}

		public RenderTexture(string name, int width, int height, FilterMode? filterMode = null, TextureWrapMode? wrapMode = null, bool useMipmaps = true, TextureFormat textureFormat = TextureFormat.RGBA8)
			: base(name, width, height, filterMode, wrapMode, useMipmaps, textureFormat) { }

		protected override void SetupTexture()
		{
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, Id);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat, Width, Height, 0, PixelFormat, PixelType.UnsignedByte, IntPtr.Zero);

			SetupFiltering(filterMode, wrapMode, useMipmaps);
		}

		public void GenerateMipmaps()
		{
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, Id);
			GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
		}

		public void Resize(int width, int height)
		{
			Width = width;
			Height = height;

			SetupTexture();
		}

		public bool UpdateSize()
		{
			if (TargetSize == null) {
				return false;
			}

			var vec = TargetSize();

			if (vec.X <= 0) {
				throw new InvalidOperationException("Texture's width can't equal or be less than zero.");
			}

			if (vec.Y <= 0) {
				throw new InvalidOperationException("Texture's height can't equal or be less than zero.");
			}

			if (vec.X == Width && vec.Y == Height) {
				return false;
			}

			Resize(vec.X, vec.Y);

			return true;
		}
	}
}
