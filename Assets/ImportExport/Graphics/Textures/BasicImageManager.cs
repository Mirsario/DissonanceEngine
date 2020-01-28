using System;
using System.IO;
using GameEngine.Graphics;

namespace GameEngine
{
	public class BasicImageManager : AssetManager<Texture>
	{
		public override string[] Extensions => new [] { ".png",".jpg",".jpeg",".gif",".bmp" };
		
		public override Texture Import(Stream stream,string fileName)
		{
			//var texture = new Texture(bitmap.Width,bitmap.Height);
			//texture.SetPixels(

			//return texture;

			throw new NotImplementedException();
		}
		public override void Export(Texture asset,Stream stream)
		{
			//asset.GetBitmap().Save(stream,ImageFormat.Png);

			throw new NotImplementedException();
		}
	}
}