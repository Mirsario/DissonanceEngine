using System;
using System.IO;
using System.Runtime.InteropServices;
using Dissonance.Framework.DevIL;
using Dissonance.Framework.OpenGL;
using GameEngine.Graphics;

namespace GameEngine
{
	public class PngManager : AssetManager<Texture>
	{
		public override string[] Extensions => new [] { ".png" };
		
		public override Texture Import(Stream stream,string fileName)
		{
			int length = (int)stream.Length;

			unsafe {
				IntPtr ptr = Marshal.AllocHGlobal(length);

				var unmanagedStream = new UnmanagedMemoryStream((byte*)ptr,length,length,FileAccess.Write);
				
				stream.CopyTo(unmanagedStream,length);

				if(!IL.Load(LoadImageTypeLumps.Png,ptr,length)) {
					throw new FileLoadException($"Unable to load image '{fileName}'.");
				}
			}

			IL.ConvertImage(ImageDataFormat.Rgba,ImageDataType.UnsignedByte);

			var texture = new Texture(IL.GetInteger(ImageInt.ImageWidth),IL.GetInteger(ImageInt.ImageHeight));

			texture.SetPixels(IL.GetData());

			return texture;
		}
		public override void Export(Texture asset,Stream stream)
		{
			throw new NotImplementedException();
		}
	}
}