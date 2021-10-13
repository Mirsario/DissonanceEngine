using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
			var (width, height, data) = LoadImageData(stream);

			await switchToMainThread; // Switches context to the main thread for texture uploading.

			var texture = new Texture(width, height);

			texture.SetPixels(data);

			return texture;
		}

		// Has to be split because async methods can't work with ref structures like Span<T>.
		private static unsafe (int width, int height, IntPtr data) LoadImageData(Stream stream)
		{
			var decoder = new PngDecoder();
			var image = Image.Load<Rgba32>(stream, decoder);

			if (!image.TryGetSinglePixelSpan(out var span)) {
				throw new InvalidOperationException($"Internal error in {nameof(PngReader)}.");
			}

			var data = (IntPtr)Unsafe.AsPointer(ref span.GetPinnableReference());

			return (image.Width, image.Height, data);
		}
	}
}
