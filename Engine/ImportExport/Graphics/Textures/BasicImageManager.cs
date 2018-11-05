using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using ImagingPixelFormat = System.Drawing.Imaging.PixelFormat;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace GameEngine
{
	public class BasicImageManager : AssetManager<Texture>
	{
		public override string[] Extensions => new [] { ".png",".jpg",".jpeg",".gif",".bmp" };
		
		public override Texture Import(Stream stream,string fileName)
		{
			var bitmap = new Bitmap(stream);
			var texture = Texture.FromBitmap(bitmap);
			bitmap.Dispose();
			return texture;
		}
        public override void Export(Texture asset,Stream stream)
        {
			asset.GetBitmap().Save(stream,ImageFormat.Png);
        }
    }
}