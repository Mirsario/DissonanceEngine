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
	public struct Pixel
	{
		public byte r,g,b,a;

		public float R {
			get => r/255f;
			set => r = (byte)(value/255);
		}
		public float G {
			get => g/255f;
			set => g = (byte)(value/255);
		}
		public float B {
			get => b/255f;
			set => b = (byte)(value/255);
		}
		public float A {
			get => a/255f;
			set => a = (byte)(value/255);
		}

		public byte this[int index] {
			get {
				switch(index) {
					case 0: return r;
					case 1: return g;
					case 2: return b;
					case 3: return a;
					default:
						throw new IndexOutOfRangeException("Indices for Pixel run from 0 to 3,inclusive.");
				}
			}
			set {
				switch(index) {
					case 0: r = value; return;
					case 1: g = value; return;
					case 2: b = value; return;
					case 3: a = value; return;
					default:
						throw new IndexOutOfRangeException("Indices for Pixel run from 0 to 3,inclusive.");
				}
			}
		}

		public Pixel(byte r,byte g,byte b,byte a)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}
		public Pixel(float r,float g,float b,float a)
		{
			this.r = (byte)(r/255);
			this.g = (byte)(g/255);
			this.b = (byte)(b/255);
			this.a = (byte)(a/255);
		}

		public static implicit operator Color(Pixel value)
		{
			return Color.FromArgb(value.a,value.r,value.g,value.b);
		}
		public static implicit operator Pixel(Color value)
		{
			return new Pixel(value.R,value.G,value.B,value.A);
		}
		public static implicit operator Vector4(Pixel value)
		{
			return new Vector4(value.R,value.G,value.B,value.A);
		}
		public static implicit operator Pixel(Vector4 value)
		{
			return new Pixel(value.x,value.y,value.z,value.w);
		}
	}
	public enum FilterMode
	{
		Point = 0,
		Bilinear,
		Trilinear
	}
	public enum TextureWrapMode
	{
		Clamp = 0,
		Repeat
	}
	public class Texture : Asset<Texture>
	{
		public static TextureWrapMode defaultWrapMode = TextureWrapMode.Repeat;
		public static FilterMode defaultFilterMode = FilterMode.Trilinear;
		
		public string name = "";
		public int Id		{ protected set; get; }
		public int Width	{ protected set; get; }
		public int Height	{ protected set; get; }
		
		protected Texture() { }
		
		public Texture(string name,int width,int height,FilterMode? filterMode = null,TextureWrapMode? wrapMode = null,bool useMipmaps = true,TextureFormat format = TextureFormat.RGBA8)
		: this(width,height,filterMode,wrapMode,useMipmaps,format)
		{
			this.name = name;
		}
		public Texture(int width,int height,FilterMode? filterMode = null,TextureWrapMode? wrapMode = null,bool useMipmaps = true,TextureFormat format = TextureFormat.RGBA8)
		{
			Id = GL.GenTexture();
			Width = width;
			Height = height;
			
			var fillColor = new Pixel(255,255,255,255);
			int length = width*height;
			var pixels = new Pixel[length];
			for(int i=0;i<length;i++) {
				pixels[i] = fillColor;
			}
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D,Id);
			GL.TexImage2D(TextureTarget.Texture2D,0,PixelInternalFormat.Rgba,width,height,0,PixelFormat.Rgba,PixelType.UnsignedByte,pixels);
			SetupFiltering(filterMode,wrapMode,useMipmaps);
		}
		internal Texture(int id,int width,int height)
		{
			Id = id;
			Width = width;
			Height = height;
		}
		public Pixel[,] GetPixels()
		{
			var pixels = new Pixel[Width,Height];
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D,Id);
			GL.GetTexImage(TextureTarget.Texture2D,0,PixelFormat.Rgba,PixelType.UnsignedByte,pixels);
			GL.BindTexture(TextureTarget.Texture2D,0);
			return pixels;
        }
        public Bitmap GetBitmap()
        {
           //TODO: Speed this up, it's currently extremely dumb.
            var pixels = GetPixels();
			var bitmap = new Bitmap(Width,Height);
			for(int y=0;y<Height;y++) {
				for(int x=0;x<Width;x++) {
					bitmap.SetPixel(x,y,pixels[x,y]);
				}
			}
            return bitmap;
        }
        public void SetPixels(Pixel[,] pixels)
		{
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D,Id);
			GL.TexSubImage2D(TextureTarget.Texture2D,0,0,0,Width,Height,PixelFormat.Rgba,PixelType.UnsignedByte,pixels);
			GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
		}
		public void Save(string path)
		{
			var bitmap = new Bitmap(Width,Height);
			var pixels = GetPixels();
			for(int y=0;y<Height;y++) {
				for(int x=0;x<Width;x++) {
					bitmap.SetPixel(x,y,Color.FromArgb(pixels[x,y].a,pixels[x,y].r,pixels[x,y].g,pixels[x,y].b));
				}	
			}
			bitmap.Save(path);
			bitmap.Dispose();
			Debug.Log("tried to save texture");
		}
		public override void Dispose()
		{
			GL.DeleteTexture(Id);
		}
		
		public static Texture FromBitmap(Bitmap bitmap,FilterMode? filterMode = null,TextureWrapMode? wrapMode = null,bool useMipmaps = true)
		{
			int id = GL.GenTexture();
			int width = bitmap.Width;
			int height = bitmap.Height;
			
			var bitData = bitmap.LockBits(new Rectangle(0,0,width,height),ImageLockMode.ReadOnly,ImagingPixelFormat.Format32bppArgb);
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D,id);
			GL.TexImage2D(TextureTarget.Texture2D,0,PixelInternalFormat.Rgba,width,height,0,PixelFormat.Bgra,PixelType.UnsignedByte,bitData.Scan0);
			SetupFiltering(filterMode,wrapMode,useMipmaps);
			bitmap.UnlockBits(bitData);
			//bitmap.Dispose();

			return new Texture(id,width,height);
		}
		internal static void SetupFiltering(FilterMode? filterMode = null,TextureWrapMode? wrapMode = null,bool useMipmaps = true)
		{
			filterMode = filterMode ?? defaultFilterMode;
			wrapMode = wrapMode ?? defaultWrapMode;

			int magFilter;
			int minFilter;
			switch(filterMode) {
				case FilterMode.Bilinear:
					magFilter = (int)TextureMagFilter.Linear;
					minFilter = (int)TextureMinFilter.Linear;
					break;
				case FilterMode.Trilinear:
					magFilter = (int)TextureMagFilter.Linear;
					minFilter = (int)TextureMinFilter.LinearMipmapLinear;
					break;
				default:
					magFilter = (int)TextureMagFilter.Nearest;
					minFilter = (int)(useMipmaps ? TextureMinFilter.NearestMipmapNearest : TextureMinFilter.Nearest);
					break;
			}
			int wrapModeInt = wrapMode==TextureWrapMode.Repeat ? 10497 : 33071;//33071
			GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureWrapS,wrapModeInt);
			GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureWrapT,wrapModeInt);
			GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureMagFilter,magFilter);	//< ERROR: InvalidEnum
			GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureMinFilter,minFilter);	
			if(useMipmaps) {
				GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
				Graphics.CheckGLErrors();
			}
			Graphics.CheckGLErrors();
		}
	}
}