using System;
using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics
{
	public class RenderTexture : Texture
	{
		public readonly Func<Vector2Int> TargetSize;
		public readonly TextureFormat TextureFormat;

		public RenderTexture(string name, Func<Vector2Int> targetSize, FilterMode? filterMode = null, TextureWrapMode? wrapMode = null, bool useMipmaps = true, TextureFormat textureFormat = TextureFormat.RGBA8)
			: this(name, targetSize().x, targetSize().y, filterMode, wrapMode, useMipmaps, textureFormat)
		{
			TargetSize = targetSize;
		}
		public RenderTexture(string name, int width, int height, FilterMode? filterMode = null, TextureWrapMode? wrapMode = null, bool useMipmaps = true, TextureFormat textureFormat = TextureFormat.RGBA8)
		{
			Id = GL.GenTexture();
			Width = width;
			Height = height;
			TextureFormat = textureFormat;

			this.Name = name;
			this.filterMode = filterMode ?? DefaultFilterMode;
			this.wrapMode = wrapMode ?? DefaultWrapMode;
			this.useMipmaps = useMipmaps;

			//Debug.Log($"Created [{Width},{Height}] RenderTexture");

			SetupTexture();
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
			if(TargetSize == null) {
				return false;
			}

			var vec = TargetSize();

			if(vec.x <= 0) {
				throw new InvalidOperationException("Texture's width can't equal or be less than zero.");
			}

			if(vec.y <= 0) {
				throw new InvalidOperationException("Texture's height can't equal or be less than zero.");
			}

			if(vec.x == Width && vec.y == Height) {
				return false;
			}

			Resize(vec.x, vec.y);

			return true;
		}

		private void SetupTexture()
		{
			var (formatGeneral, formatInternal, _, _) = Rendering.textureFormatInfo[TextureFormat];

			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, Id);
			GL.TexImage2D(TextureTarget.Texture2D, 0, formatInternal, Width, Height, 0, formatGeneral, PixelType.UnsignedByte, IntPtr.Zero);

			SetupFiltering(filterMode, wrapMode, useMipmaps);
		}
	}
}
