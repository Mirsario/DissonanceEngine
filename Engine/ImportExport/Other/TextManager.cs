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
	public class TextManager : AssetManager<string>
	{
		public override string[] Extensions => new [] { ".txt" };
		
		public override string Import(Stream stream,string fileName)
		{
			using(var reader = new StreamReader(stream)) {
				return reader.ReadToEnd();
			}
		}
        public override void Export(string text,Stream stream)
        {
			using(var writer = new StreamWriter(stream)) {
				writer.Write(text);
			}
        }
    }
}