using System;
using System.IO;
using System.Threading.Tasks;
using Dissonance.Engine.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace Dissonance.Engine.IO
{
	public class PngReader : IAssetReader<Texture>, IAssetReader<Pixel[,]>
	{
		public string[] Extensions { get; } = { ".png" };

		async ValueTask<Texture> IAssetReader<Texture>.ReadAsset(AssetFileEntry assetFile, MainThreadCreationContext switchToMainThread)
		{
			var (width, height, pixels) = ReadPixels<Rgba32>(assetFile);

			await switchToMainThread; // Switches context to the main thread for texture uploading.

			var texture = new Texture(width, height);

			texture.SetPixels(pixels);

			return texture;
		}

		async ValueTask<Pixel[,]> IAssetReader<Pixel[,]>.ReadAsset(AssetFileEntry assetFile, MainThreadCreationContext switchToMainThread)
		{
			var (width, height, pixels) = ReadPixels<Rgba32>(assetFile);
			var result = new Pixel[width, height];
			int i = 0;

			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					var pixel = pixels[i++];

					result[x, y] = new Pixel(pixel.R, pixel.G, pixel.B, pixel.A);
				}
			}

			return result;
		}

		private static (int width, int height, T[]) ReadPixels<T>(AssetFileEntry assetFile) where T : unmanaged, IPixel<T>
		{
			using var stream = assetFile.OpenStream();

			return LoadImageData<T>(stream);
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
