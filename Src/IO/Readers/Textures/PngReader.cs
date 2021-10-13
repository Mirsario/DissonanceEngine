using System;
using System.IO;
using System.Threading.Tasks;
using Dissonance.Engine.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace Dissonance.Engine.IO
{
	public class PngReader : IAssetReader<Texture>
	{
		public string[] Extensions { get; } = { ".png" };

		public async ValueTask<Texture> ReadFromStream(Stream stream, string assetPath, MainThreadCreationContext switchToMainThread)
		{
			var (width, height, pixels) = LoadImageData<Rgba32>(stream);

			await switchToMainThread; // Switches context to the main thread for texture uploading.

			var texture = new Texture(width, height);

			texture.SetPixels(pixels);

			return texture;
		}

		// Has to be split because async methods can't work with ref structures like Span<T>.
		private static unsafe (int width, int height, TPixel[] pixels) LoadImageData<TPixel>(Stream stream) where TPixel : unmanaged, IPixel<TPixel>
		{
			var decoder = new PngDecoder();
			var image = Image.Load<TPixel>(stream, decoder);

			if (!image.TryGetSinglePixelSpan(out var span)) {
				throw new InvalidOperationException($"Internal error in {nameof(PngReader)}.");
			}

			var data = span.ToArray();

			return (image.Width, image.Height, data);
		}
	}
}
