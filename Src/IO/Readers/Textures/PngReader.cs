using System;
using System.IO;
using System.Runtime.InteropServices;
using Dissonance.Framework.Imaging;
using Dissonance.Engine.Graphics;
using System.Threading.Tasks;

namespace Dissonance.Engine.IO
{
	public class PngReader : IAssetReader<Texture>
	{
		public string[] Extensions { get; } = { ".png" };

		public async ValueTask<Texture> ReadFromStream(Stream stream, string assetPath, MainThreadCreationContext switchToMainThread)
		{
			int length = (int)stream.Length;

			IL.Init();

			unsafe {
				IntPtr ptr = Marshal.AllocHGlobal(length);

				var unmanagedStream = new UnmanagedMemoryStream((byte*)ptr, length, length, FileAccess.Write);

				stream.CopyTo(unmanagedStream, length);

				if (!IL.Load(LoadImageTypeLumps.Png, ptr, length)) {
					throw new FileLoadException($"Unable to load image.");
				}
			}

			IL.ConvertImage(ImageDataFormat.Rgba, ImageDataType.UnsignedByte);

			int width = IL.GetInteger(ImageInt.ImageWidth);
			int height = IL.GetInteger(ImageInt.ImageHeight);

			await switchToMainThread;

			var texture = new Texture(width, height);

			texture.SetPixels(IL.GetData());

			return texture;
		}
	}
}
