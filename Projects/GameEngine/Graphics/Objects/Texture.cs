using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using ImagingPixelFormat = System.Drawing.Imaging.PixelFormat;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace GameEngine.Graphics
{
	public class Texture : Asset<Texture>
	{
		public static FilterMode defaultFilterMode = FilterMode.Trilinear;
		public static TextureWrapMode defaultWrapMode = TextureWrapMode.Repeat;
		
		public string name = "";
		protected FilterMode filterMode;
		protected TextureWrapMode wrapMode;
		protected bool useMipmaps;

		public int Id { get; protected set; }
		public int Width { get; protected set; }
		public int Height { get; protected set; }
		
		public Vector2Int Size => new Vector2Int(Width,Height);
		
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

			this.filterMode = filterMode ?? defaultFilterMode;
			this.wrapMode = wrapMode ?? defaultWrapMode;
			this.useMipmaps = useMipmaps;
			
			var fillColor = new Pixel(255,255,255,255);
			int length = width*height;
			var pixels = new Pixel[length];
			for(int i=0;i<length;i++) {
				pixels[i] = fillColor;
			}

			SetupTexture(pixels);
		}
		internal Texture(int id,int width,int height)
		{
			Id = id;
			Width = width;
			Height = height;
		}

		public override void Dispose()
		{
			GL.DeleteTexture(Id);
		}

		public Pixel[,] GetPixels()
		{
			var pixels1D = new Pixel[Width*Height];
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D,Id);
			GL.GetTexImage(TextureTarget.Texture2D,0,PixelFormat.Rgba,PixelType.UnsignedByte,pixels1D);
			GL.BindTexture(TextureTarget.Texture2D,0);

			//TODO: Can these loops be avoided?
			var pixels = new Pixel[Width,Height];
			int i = 0;
			for(int y = 0;y<Height;y++) {
				for(int x = 0;x<Width;x++) {
					pixels[x,y] = pixels1D[i++];
				}
			}
			return pixels;
		}
		public Bitmap GetBitmap()
		{
		   //TODO: Speed this up
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
			//TODO: Can these loops be avoided?
			var pixels1D = new Pixel[Width*Height];
			int i = 0;
			for(int y = 0;y<Height;y++) {
				for(int x = 0;x<Width;x++) {
					pixels1D[i++] = pixels[x,y];
				}
			}

			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D,Id);
			GL.TexSubImage2D(TextureTarget.Texture2D,0,0,0,Width,Height,PixelFormat.Rgba,PixelType.UnsignedByte,pixels1D);
			GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
		}
		public void Save(string path)
		{
			var bitmap = GetBitmap();
			bitmap.Save(path);
			bitmap.Dispose();
		}

		private void SetupTexture(Pixel[] pixels)
		{
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D,Id);
			GL.TexImage2D(TextureTarget.Texture2D,0,PixelInternalFormat.Rgba,Width,Height,0,PixelFormat.Rgba,PixelType.UnsignedByte,pixels);

			SetupFiltering(filterMode,wrapMode,useMipmaps);
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
			filterMode ??= defaultFilterMode;
			wrapMode ??= defaultWrapMode;

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

			int wrapModeInt = wrapMode==TextureWrapMode.Repeat ? 10497 : 33071;
			GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureWrapS,wrapModeInt);
			GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureWrapT,wrapModeInt);
			GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureMagFilter,magFilter);
			GL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureMinFilter,minFilter);	

			if(useMipmaps) {
				GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
			}
		}
	}
}