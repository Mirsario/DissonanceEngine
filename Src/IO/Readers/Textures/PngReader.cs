using System;
using System.IO;
using System.Runtime.InteropServices;
using Dissonance.Framework.Imaging;
using Dissonance.Engine.Graphics;

namespace Dissonance.Engine.IO
{
	public class PngReader : IAssetReader<Texture>
	{
		public string[] Extensions { get; } = { ".png" };

		public Texture ReadFromStream(Stream stream, string assetPath)
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

			var texture = new Texture(IL.GetInteger(ImageInt.ImageWidth), IL.GetInteger(ImageInt.ImageHeight));

			texture.SetPixels(IL.GetData());

			return texture;
		}
	}
}
