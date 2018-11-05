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
	public class BytesManager : AssetManager<byte[]>
	{
		public override string[] Extensions => new [] { ".bytes" };
		
		public override byte[] Import(Stream stream,string fileName)
		{
			var bytes = new byte[stream.Length];
			stream.Read(bytes,0,bytes.Length);
			return bytes;
		}
        public override void Export(byte[] bytes,Stream stream)
        {
			stream.Write(bytes,0,bytes.Length);
        }
    }
}