using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.IO;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using ImagingPixelformat = System.Drawing.Imaging.PixelFormat;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace GameEngine
{
	//A "rewrite" of http://github.com/andburn/dds-reader
	//
	public class DDSManager : AssetManager<Texture>
	{
		public override string[] Extensions => new [] {".dds"};

		public static bool preserveAlpha = true;
		private static readonly byte[] signature = { (byte)'D',(byte)'D',(byte)'S',(byte)' '};

		public override Texture Import(Stream stream,string fileName)
		{
			var reader = new BinaryReader(stream);

			var header = ReadHeader(reader);
			uint blockSize = 0;
			var pixelFormat = GetFormat(header,ref blockSize);
			if(pixelFormat==DDSPixelFormat.UNKNOWN){
				throw new FormatException("Unknown pixel format");
			}
			var data = ReadData(reader,header);
			var rawData = DDSDecompressor.Expand(header,data,pixelFormat);
			var bitmap = CreateBitmap((int)header.width,(int)header.height,rawData);

			reader.Dispose();
			stream.Dispose();

			var texture = Texture.FromBitmap(bitmap);
			bitmap.Dispose();
			return texture;
		}

		private DDSHeader ReadHeader(BinaryReader reader)
		{
			var header = new DDSHeader();
			var sign = reader.ReadBytes(4);
			for(int i=0;i<4;i++){
				if(sign[i]!=signature[i]){
					throw new Exception("File's signature is wrong");
				}
			}
			header.size = reader.ReadUInt32();
			if(header.size!=124){
				throw new Exception("File's header size is wrong");
			}

			//convert the data
			header.flags = reader.ReadUInt32();
			header.height = reader.ReadUInt32();
			header.width = reader.ReadUInt32();
			header.sizeorpitch = reader.ReadUInt32();
			header.depth = reader.ReadUInt32();
			if(header.depth==0){
				header.depth = 1;
			}
			header.mipmapcount = reader.ReadUInt32();
			header.alphabitdepth = reader.ReadUInt32();

			header.reserved = new uint[10];
			for(int i=0;i<10;i++){
				header.reserved[i] = reader.ReadUInt32();
			}

			//pixelFormat
			header.pixelformat.size = reader.ReadUInt32();
			header.pixelformat.flags = reader.ReadUInt32();
			header.pixelformat.fourcc = reader.ReadUInt32();
			header.pixelformat.rgbbitcount = reader.ReadUInt32();
			header.pixelformat.rbitmask = reader.ReadUInt32();
			header.pixelformat.gbitmask = reader.ReadUInt32();
			header.pixelformat.bbitmask = reader.ReadUInt32();
			header.pixelformat.alphabitmask = reader.ReadUInt32();

			//caps
			header.ddscaps.caps1 = reader.ReadUInt32();
			header.ddscaps.caps2 = reader.ReadUInt32();
			header.ddscaps.caps3 = reader.ReadUInt32();
			header.ddscaps.caps4 = reader.ReadUInt32();
			header.texturestage = reader.ReadUInt32();

			return header;
		}
		private byte[] ReadData(BinaryReader reader,DDSHeader header)
		{
			byte[] compdata = null;
			uint compSize = 0;

			if((header.flags & DDSHelper.DDSD_LINEARSIZE)>1){
				compdata = reader.ReadBytes((int)header.sizeorpitch);
				compSize = (uint)compdata.Length;
			}else{
				uint bps = header.width*header.pixelformat.rgbbitcount/8;
				compSize = bps*header.height*header.depth;
				compdata = new byte[compSize];

				var mem = new MemoryStream((int)compSize);

				byte[] temp;
				for(int z=0;z<header.depth;z++){
					for(int y=0;y<header.height;y++){
						temp = reader.ReadBytes((int)bps);
						mem.Write(temp,0,temp.Length);
					}
				}
				mem.Seek(0,SeekOrigin.Begin);

				mem.Read(compdata,0,compdata.Length);
				mem.Close();
			}
			return compdata;
		}
		private Bitmap CreateBitmap(int width,int height,byte[] rawData)
		{
			var pxFormat = preserveAlpha ? ImagingPixelformat.Format32bppArgb : ImagingPixelformat.Format32bppRgb;

			var bitmap = new Bitmap(width,height,pxFormat);

			var data = bitmap.LockBits(new Rectangle(0,0,bitmap.Width,bitmap.Height),ImageLockMode.WriteOnly,pxFormat);
			var scan = data.Scan0;
			int size = bitmap.Width*bitmap.Height*4;

			unsafe {
				var p = (byte*)scan;
				for(int i=0;i<size;i += 4){
					//iterate through bytes.
					//Bitmap stores it's data in RGBA order.
					//DDS stores it's data in BGRA order.
					p[i] = rawData[i+2];//B
					p[i+1] = rawData[i+1];//G
					p[i+2] = rawData[i];//R
					p[i+3] = rawData[i+3];//A
				}
			}

			bitmap.UnlockBits(data);
			return bitmap;
		}
		private DDSPixelFormat GetFormat(DDSHeader header,ref uint blockSize)
		{
			var format = DDSPixelFormat.UNKNOWN;
			if((header.pixelformat.flags & DDSHelper.DDPF_FOURCC)==DDSHelper.DDPF_FOURCC){
				blockSize = (header.width+3)/4*((header.height+3)/4)*header.depth;
				switch(header.pixelformat.fourcc){
					case DDSHelper.FOURCC_DXT1:
						format = DDSPixelFormat.DXT1;
						blockSize *= 8;
						break;
					case DDSHelper.FOURCC_DXT2:
						format = DDSPixelFormat.DXT2;
						blockSize *= 16;
						break;
					case DDSHelper.FOURCC_DXT3:
						format = DDSPixelFormat.DXT3;
						blockSize *= 16;
						break;
					case DDSHelper.FOURCC_DXT4:
						format = DDSPixelFormat.DXT4;
						blockSize *= 16;
						break;
					case DDSHelper.FOURCC_DXT5:
						format = DDSPixelFormat.DXT5;
						blockSize *= 16;
						break;
					case DDSHelper.FOURCC_ATI1:
						format = DDSPixelFormat.ATI1N;
						blockSize *= 8;
						break;
					case DDSHelper.FOURCC_ATI2:
						format = DDSPixelFormat.THREEDC;
						blockSize *= 16;
						break;
					case DDSHelper.FOURCC_RXGB:
						format = DDSPixelFormat.RXGB;
						blockSize *= 16;
						break;
					case DDSHelper.FOURCC_DOLLARNULL:
						format = DDSPixelFormat.A16B16G16R16;
						blockSize = header.width*header.height*header.depth*8;
						break;
					case DDSHelper.FOURCC_oNULL:
						format = DDSPixelFormat.R16F;
						blockSize = header.width*header.height*header.depth*2;
						break;
					case DDSHelper.FOURCC_pNULL:
						format = DDSPixelFormat.G16R16F;
						blockSize = header.width*header.height*header.depth*4;
						break;
					case DDSHelper.FOURCC_qNULL:
						format = DDSPixelFormat.A16B16G16R16F;
						blockSize = header.width*header.height*header.depth*8;
						break;
					case DDSHelper.FOURCC_rNULL:
						format = DDSPixelFormat.R32F;
						blockSize = header.width*header.height*header.depth*4;
						break;
					case DDSHelper.FOURCC_sNULL:
						format = DDSPixelFormat.G32R32F;
						blockSize = header.width*header.height*header.depth*8;
						break;
					case DDSHelper.FOURCC_tNULL:
						format = DDSPixelFormat.A32B32G32R32F;
						blockSize = header.width*header.height*header.depth*16;
						break;
					default:
						format = DDSPixelFormat.UNKNOWN;
						blockSize *= 16;
						break;
				}
			}else{
				//uncompressed image
				if((header.pixelformat.flags & DDSHelper.DDPF_LUMINANCE)==DDSHelper.DDPF_LUMINANCE){
					if((header.pixelformat.flags & DDSHelper.DDPF_ALPHAPIXELS)==DDSHelper.DDPF_ALPHAPIXELS){
						format = DDSPixelFormat.LUMINANCE_ALPHA;
					}else{
						format = DDSPixelFormat.LUMINANCE;
					}
				}else{
					if((header.pixelformat.flags & DDSHelper.DDPF_ALPHAPIXELS)==DDSHelper.DDPF_ALPHAPIXELS){
						format = DDSPixelFormat.RGBA;
					}else{
						format = DDSPixelFormat.RGB;
					}
				}
				blockSize = header.width*header.height*header.depth*(header.pixelformat.rgbbitcount>>3);
			}
			return format;
		}

		//Enums
		private struct Color8888
		{
			public byte red;
			public byte green;
			public byte blue;
			public byte alpha;
		}
		private struct Color565
		{
			public ushort blue;		//: 5;
			public ushort green;	//: 6;
			public ushort red;		//: 5;
		}
		private static class DDSHelper
		{
			#region Constants
			//DDSHeader flags
			public const int DDSD_CAPS = 0x00000001;

			public const int DDSD_HEIGHT = 0x00000002;
			public const int DDSD_WIDTH = 0x00000004;
			public const int DDSD_PITCH = 0x00000008;
			public const int DDSD_PIXELformat = 0x00001000;
			public const int DDSD_MIPMAPCOUNT = 0x00020000;
			public const int DDSD_LINEARSIZE = 0x00080000;
			public const int DDSD_DEPTH = 0x00800000;

			//PixelFormat values
			public const int DDPF_ALPHAPIXELS = 0x00000001;

			public const int DDPF_FOURCC = 0x00000004;
			public const int DDPF_RGB = 0x00000040;
			public const int DDPF_LUMINANCE = 0x00020000;

			//DDSCaps
			public const int DDSCAPS_COMPLEX = 0x00000008;

			public const int DDSCAPS_TEXTURE = 0x00001000;
			public const int DDSCAPS_MIPMAP = 0x00400000;
			public const int DDSCAPS2_CUBEMAP = 0x00000200;
			public const int DDSCAPS2_CUBEMAP_POSITIVEX = 0x00000400;
			public const int DDSCAPS2_CUBEMAP_NEGATIVEX = 0x00000800;
			public const int DDSCAPS2_CUBEMAP_POSITIVEY = 0x00001000;
			public const int DDSCAPS2_CUBEMAP_NEGATIVEY = 0x00002000;
			public const int DDSCAPS2_CUBEMAP_POSITIVEZ = 0x00004000;
			public const int DDSCAPS2_CUBEMAP_NEGATIVEZ = 0x00008000;
			public const int DDSCAPS2_VOLUME = 0x00200000;

			//FOURCC
			public const uint FOURCC_DXT1 = 0x31545844;

			public const uint FOURCC_DXT2 = 0x32545844;
			public const uint FOURCC_DXT3 = 0x33545844;
			public const uint FOURCC_DXT4 = 0x34545844;
			public const uint FOURCC_DXT5 = 0x35545844;
			public const uint FOURCC_ATI1 = 0x31495441;
			public const uint FOURCC_ATI2 = 0x32495441;
			public const uint FOURCC_RXGB = 0x42475852;
			public const uint FOURCC_DOLLARNULL = 0x24;
			public const uint FOURCC_oNULL = 0x6f;
			public const uint FOURCC_pNULL = 0x70;
			public const uint FOURCC_qNULL = 0x71;
			public const uint FOURCC_rNULL = 0x72;
			public const uint FOURCC_sNULL = 0x73;
			public const uint FOURCC_tNULL = 0x74;
			#endregion Constants
			#region Functions
			//iCompFormatToBpp
			public static uint PixelFormatToBpp(DDSPixelFormat pf,uint rgbbitcount)
			{
				switch(pf){
					case DDSPixelFormat.LUMINANCE:
					case DDSPixelFormat.LUMINANCE_ALPHA:
					case DDSPixelFormat.RGBA:
					case DDSPixelFormat.RGB:
						return rgbbitcount/8;
					case DDSPixelFormat.THREEDC:
					case DDSPixelFormat.RXGB:
						return 3;
					case DDSPixelFormat.ATI1N:
						return 1;
					case DDSPixelFormat.R16F:
						return 2;
					case DDSPixelFormat.A16B16G16R16:
					case DDSPixelFormat.A16B16G16R16F:
					case DDSPixelFormat.G32R32F:
						return 8;
					case DDSPixelFormat.A32B32G32R32F:
						return 16;
					default:
						return 4;
				}
			}
			//iCompFormatToBpc
			public static uint PixelFormatToBpc(DDSPixelFormat pf)
			{
				switch(pf){
					case DDSPixelFormat.R16F:
					case DDSPixelFormat.G16R16F:
					case DDSPixelFormat.A16B16G16R16F:
						return 4;
					case DDSPixelFormat.R32F:
					case DDSPixelFormat.G32R32F:
					case DDSPixelFormat.A32B32G32R32F:
						return 4;
					case DDSPixelFormat.A16B16G16R16:
						return 2;
					default:
						return 1;
				}
			}
			public static bool Check16BitComponents(DDSHeader header)
			{
				if(header.pixelformat.rgbbitcount!=32)
					return false;
				//a2b10g10r10 format
				if(header.pixelformat.rbitmask==0x3FF00000 && header.pixelformat.gbitmask==0x000FFC00 && header.pixelformat.bbitmask==0x000003FF && header.pixelformat.alphabitmask==0xC0000000)
					return true;
				//a2r10g10b10 format
				if(header.pixelformat.rbitmask==0x000003FF && header.pixelformat.gbitmask==0x000FFC00 && header.pixelformat.bbitmask==0x3FF00000 && header.pixelformat.alphabitmask==0xC0000000)
					return true;

				return false;
			}
			public static void CorrectPremult(uint pixnum,ref byte[] buffer)
			{
				for(uint i=	0;i<pixnum;i++)
				{
					byte alpha = buffer[i+3];
					if(alpha==0)continue;
					int red = (buffer[i] << 8)/alpha;
					int green = (buffer[i+1] << 8)/alpha;
					int blue = (buffer[i+2] << 8)/alpha;

					buffer[i] = (byte)red;
					buffer[i+1] = (byte)green;
					buffer[i+2] = (byte)blue;
				}
			}
			public static void ComputeMaskParams(uint mask,ref int shift1,ref int mul,ref int shift2)
			{
				shift1 = 0;mul = 1;shift2 = 0;
				if(mask==0 || mask==uint.MaxValue)
					return;
				while((mask & 1)==0)
				{
					mask >>=1;
					shift1++;
				}
				uint bc = 0;
				while((mask & 1 <<(int)bc)!=0)bc++;
				while(mask*mul<255)
					mul = (mul <<(int)bc)+1;
				mask *= (uint)mul;

				while((mask & ~0xff)!=0)
				{
					mask >>=1;
					shift2++;
				}
			}
			public static unsafe void DxtcReadColors(byte*data,ref Color8888[] op)
			{
				byte r0,g0,b0,r1,g1,b1;

				b0 = (byte)(data[0] & 0x1F);
				g0 = (byte)((data[0] & 0xE0)>> 5|(data[1] & 0x7)<< 3);
				r0 = (byte)((data[1] & 0xF8)>> 3);

				b1 = (byte)(data[2] & 0x1F);
				g1 = (byte)((data[2] & 0xE0)>> 5|(data[3] & 0x7)<< 3);
				r1 = (byte)((data[3] & 0xF8)>> 3);

				op[0].red = (byte)(r0 << 3 | r0 >> 2);
				op[0].green = (byte)(g0 << 2 | g0 >> 3);
				op[0].blue = (byte)(b0 << 3 | b0 >> 2);

				op[1].red = (byte)(r1 << 3 | r1 >> 2);
				op[1].green = (byte)(g1 << 2 | g1 >> 3);
				op[1].blue = (byte)(b1 << 3 | b1 >> 2);
			}
			public static void DxtcReadColor(ushort data,ref Color8888 op)
			{
				byte r,g,b;

				b = (byte)(data & 0x1f);
				g = (byte)((data & 0x7E0)>> 5);
				r = (byte)((data & 0xF800)>> 11);

				op.red = (byte)(r << 3 | r >> 2);
				op.green = (byte)(g << 2 | g >> 3);
				op.blue = (byte)(b << 3 | r >> 2);
			}
			public static unsafe void DxtcReadColors(byte*data,ref Color565 color_0,ref Color565 color_1)
			{
				color_0.blue = (byte)(data[0] & 0x1F);
				color_0.green = (byte)((data[0] & 0xE0)>> 5|(data[1] & 0x7)<< 3);
				color_0.red = (byte)((data[1] & 0xF8)>> 3);

				color_0.blue = (byte)(data[2] & 0x1F);
				color_0.green = (byte)((data[2] & 0xE0)>> 5|(data[3] & 0x7)<< 3);
				color_0.red = (byte)((data[3] & 0xF8)>> 3);
			}
			public static void GetBitsFromMask(uint mask,ref uint shiftLeft,ref uint shiftRight)
			{
				uint temp,i;

				if(mask==0)
				{
					shiftLeft = shiftRight = 0;
					return;
				}

				temp = mask;
				for(i=	0;i<32;i++,temp >>=1)
				{
					if((temp & 1)!=0)
						break;
				}
				shiftRight = i;

				//Temp is preserved,so use it again:
				for(i=	0;i<8;i++,temp >>=1)
				{
					if((temp & 1)==0)
						break;
				}
				shiftLeft = 8-i;
			}
			//This function simply counts how many contiguous bits are in the mask.
			public static uint CountBitsFromMask(uint mask)
			{
				uint i,testBit = 0x01,count = 0;
				bool foundBit = false;

				for(i=	0;i<32;i++,testBit <<=1)
				{
					if((mask & testBit)!=0)
					{
						if(!foundBit)
							foundBit = true;
						count++;
					}
					else if(foundBit)
						return count;
				}

				return count;
			}
			public static uint HalfToFloat(ushort y)
			{
				int s = y >> 15& 0x00000001;
				int e = y >> 10& 0x0000001f;
				int m = y & 0x000003ff;

				if(e==0)
				{
					if(m==0)
					{
						//
						//Plus or minus zero
						//
						return(uint)(s << 31);
					}
					//
					//Denormalized number--renormalize it
					//
					while((m & 0x00000400)==0)
					{
						m <<=1;
						e -= 1;
					}

					e += 1;
					m &= ~0x00000400;
				}
				else if(e==31)
				{
					if(m==0)
					{
						//
						//Positive or negative infinity
						//
						return(uint)(s << 31| 0x7f800000);
					}
					//
					//Nan--preserve sign and significand bits
					//
					return(uint)(s << 31| 0x7f800000 | m << 13);
				}

				//
				//Normalized number
				//
				e = e+(127-15);
				m = m << 13;

				//
				//Assemble s,e and m.
				//
				return(uint)(s << 31|e << 23| m);
			}
			public static unsafe void ConvFloat16ToFloat32(uint*dest,ushort*src,uint size)
			{
				uint i;
				for(i=	0;i<size;++i,++dest,++src)
				{
					//float: 1 sign bit,8 exponent bits,23 mantissa bits
					//half: 1 sign bit,5 exponent bits,10 mantissa bits
					*dest = HalfToFloat(*src);
				}
			}
			public static unsafe void ConvG16R16ToFloat32(uint*dest,ushort*src,uint size)
			{
				uint i;
				for(i=	0;i<size;i += 3)
				{
					//float: 1 sign bit,8 exponent bits,23 mantissa bits
					//half: 1 sign bit,5 exponent bits,10 mantissa bits
					*dest++=	HalfToFloat(*src++);
					*dest++=	HalfToFloat(*src++);
					*(float*)dest++=	1.0f;
				}
			}
			public static unsafe void ConvR16ToFloat32(uint*dest,ushort*src,uint size)
			{
				uint i;
				for(i=	0;i<size;i += 3)
				{
					//float: 1 sign bit,8 exponent bits,23 mantissa bits
					//half: 1 sign bit,5 exponent bits,10 mantissa bits
					*dest++=	HalfToFloat(*src++);
					*(float*)dest++=	1.0f;
					*(float*)dest++=	1.0f;
				}
			}
			#endregion
		}
		public enum DDSPixelFormat
		{
			RGBA,
			RGB,
			DXT1,
			DXT2,
			DXT3,
			DXT4,
			DXT5,
			THREEDC,
			ATI1N,
			LUMINANCE,
			LUMINANCE_ALPHA,
			RXGB,
			A16B16G16R16,
			R16F,
			G16R16F,
			A16B16G16R16F,
			R32F,
			G32R32F,
			A32B32G32R32F,
			UNKNOWN
		}
		//Structs
		[StructLayout(LayoutKind.Sequential,CharSet = CharSet.Auto)]
		private struct DDSHeader
		{
			//todo: rename variables
			public uint size;	//equals size of struct(which is part of the data file!)
			public uint flags;
			public uint height;
			public uint width;
			public uint sizeorpitch;
			public uint depth;
			public uint mipmapcount;
			public uint alphabitdepth;

			//[MarshalAs(UnmanagedType.U4,SizeConst = 11)]
			public uint[] reserved;//[11];

			[StructLayout(LayoutKind.Sequential,CharSet = CharSet.Auto)]
			public struct PixelFormatStruct
			{
				public uint size;//equals size of struct(which is part of the data file!)
				public uint flags;
				public uint fourcc;
				public uint rgbbitcount;
				public uint rbitmask;
				public uint gbitmask;
				public uint bbitmask;
				public uint alphabitmask;
			}

			public PixelFormatStruct pixelformat;

			[StructLayout(LayoutKind.Sequential,CharSet = CharSet.Auto)]
			public struct DDSCapsStruct
			{
				public uint caps1;
				public uint caps2;
				public uint caps3;
				public uint caps4;
			}

			public DDSCapsStruct ddscaps;
			public uint texturestage;
		}
		private class DDSDecompressor
		{
			public static byte[] Expand(DDSHeader header,byte[] data,DDSPixelFormat pixelFormat)
			{
				//System.Diagnostics.Debug.WriteLine(pixelFormat);
				byte[] rawData = null;
				switch(pixelFormat){
					case DDSPixelFormat.RGBA:
						rawData = DecompressRGBA(header,data,pixelFormat);
						break;
					case DDSPixelFormat.RGB:
						rawData = DecompressRGB(header,data,pixelFormat);
						break;
					case DDSPixelFormat.LUMINANCE:
					case DDSPixelFormat.LUMINANCE_ALPHA:
						rawData = DecompressLum(header,data,pixelFormat);
						break;
					case DDSPixelFormat.DXT1:
						rawData = DecompressDXT1(header,data,pixelFormat);
						break;
					case DDSPixelFormat.DXT2:
						rawData = DecompressDXT2(header,data,pixelFormat);
						break;
					case DDSPixelFormat.DXT3:
						rawData = DecompressDXT3(header,data,pixelFormat);
						break;
					case DDSPixelFormat.DXT4:
						rawData = DecompressDXT4(header,data,pixelFormat);
						break;
					case DDSPixelFormat.DXT5:
						rawData = DecompressDXT5(header,data,pixelFormat);
						break;
					case DDSPixelFormat.THREEDC:
						rawData = Decompress3Dc(header,data,pixelFormat);
						break;
					case DDSPixelFormat.ATI1N:
						rawData = DecompressAti1n(header,data,pixelFormat);
						break;
					case DDSPixelFormat.RXGB:
						rawData = DecompressRXGB(header,data,pixelFormat);
						break;
					case DDSPixelFormat.R16F:
					case DDSPixelFormat.G16R16F:
					case DDSPixelFormat.A16B16G16R16F:
					case DDSPixelFormat.R32F:
					case DDSPixelFormat.G32R32F:
					case DDSPixelFormat.A32B32G32R32F:
						rawData = DecompressFloat(header,data,pixelFormat);
						break;
					default:
						throw new FormatException();
				}
				return rawData;
			}
			private static unsafe byte[] DecompressDXT1(DDSHeader header,byte[] data,DDSPixelFormat pixelFormat)
			{
				int bpp = (int)DDSHelper.PixelFormatToBpp(pixelFormat,header.pixelformat.rgbbitcount);
				int bps = (int)(header.width*bpp*DDSHelper.PixelFormatToBpc(pixelFormat));
				int sizeofplane = (int)(bps*header.height);
				int width = (int)header.width;
				int height = (int)header.height;
				int depth = (int)header.depth;
				var rawData = new byte[depth*sizeofplane+height*bps+width*bpp];
				var colors = new Color8888[4];
				colors[0].alpha = 0xFF;
				colors[1].alpha = 0xFF;
				colors[2].alpha = 0xFF;
				fixed(byte*bytePtr = data){
					var temp = bytePtr;
					for(int z=0;z<depth;z++){
						for(int y=0;y<height;y += 4){
							for(int x=0;x<width;x += 4){
								ushort colour0=	*(ushort*)temp;
								ushort colour1=	*(ushort*)(temp+2);
								DDSHelper.DxtcReadColor(colour0,ref colors[0]);
								DDSHelper.DxtcReadColor(colour1,ref colors[1]);
								uint bitmask = ((uint*)temp)[1];
								temp += 8;
								if(colour0>colour1){
									colors[2].blue = (byte)((2*colors[0].blue+colors[1].blue+1)/3);
									colors[2].green = (byte)((2*colors[0].green+colors[1].green+1)/3);
									colors[2].red = (byte)((2*colors[0].red+colors[1].red+1)/3);
									colors[3].blue = (byte)((colors[0].blue+2*colors[1].blue+1)/3);
									colors[3].green = (byte)((colors[0].green+2*colors[1].green+1)/3);
									colors[3].red = (byte)((colors[0].red+2*colors[1].red+1)/3);
									colors[3].alpha = 0xFF;
								}else{
									colors[2].blue = (byte)((colors[0].blue+colors[1].blue)/2);
									colors[2].green = (byte)((colors[0].green+colors[1].green)/2);
									colors[2].red = (byte)((colors[0].red+colors[1].red)/2);
									colors[3].blue = (byte)((colors[0].blue+2*colors[1].blue+1)/3);
									colors[3].green = (byte)((colors[0].green+2*colors[1].green+1)/3);
									colors[3].red = (byte)((colors[0].red+2*colors[1].red+1)/3);
									colors[3].alpha = 0x00;
								}
								for(int j=0,k=0;j<4;j++){
									for(int i=0;i<4;i++,k++){
										int select = (int)((bitmask & 0x03 << k*2)>> k*2);
										var col = colors[select];
										if(x+i<width&&y+j<height){
											uint offset = (uint)(z*sizeofplane+(y+j)*bps+(x+i)*bpp);
											rawData[offset+0] = col.red;
											rawData[offset+1] = col.green;
											rawData[offset+2] = col.blue;
											rawData[offset+3] = col.alpha;
										}
									}
								}
							}
						}
					}
				}
				return rawData;
			}
			private static byte[] DecompressDXT2(DDSHeader header,byte[] data,DDSPixelFormat pixelFormat)
			{
				int width = (int)header.width;
				int height = (int)header.height;
				int depth = (int)header.depth;
				var rawData = DecompressDXT3(header,data,pixelFormat);
				DDSHelper.CorrectPremult((uint)(width*height*depth),ref rawData);
				return rawData;
			}
			private static unsafe byte[] DecompressDXT3(DDSHeader header,byte[] data,DDSPixelFormat pixelFormat)
			{
				int bpp = (int)DDSHelper.PixelFormatToBpp(pixelFormat,header.pixelformat.rgbbitcount);
				int bps = (int)(header.width*bpp*DDSHelper.PixelFormatToBpc(pixelFormat));
				int sizeofplane = (int)(bps*header.height);
				int width = (int)header.width;
				int height = (int)header.height;
				int depth = (int)header.depth;
				var rawData = new byte[depth*sizeofplane+height*bps+width*bpp];
				var colors = new Color8888[4];
				fixed(byte*bytePtr = data){
					var temp = bytePtr;
					for(int z=0;z<depth;z++){
						for(int y=0;y<height;y += 4){
							for(int x=0;x<width;x += 4){
								var alpha = temp;
								temp += 8;
								DDSHelper.DxtcReadColors(temp,ref colors);
								temp += 4;
								uint bitmask = ((uint*)temp)[1];
								temp += 4;
								colors[2].blue = (byte)((2*colors[0].blue+colors[1].blue+1)/3);
								colors[2].green = (byte)((2*colors[0].green+colors[1].green+1)/3);
								colors[2].red = (byte)((2*colors[0].red+colors[1].red+1)/3);
								colors[3].blue = (byte)((colors[0].blue+2*colors[1].blue+1)/3);
								colors[3].green = (byte)((colors[0].green+2*colors[1].green+1)/3);
								colors[3].red = (byte)((colors[0].red+2*colors[1].red+1)/3);
								for(int j=0,k=0;j<4;j++){
									for(int i=	0;i<4;k++,i++){
										int select = (int)((bitmask & 0x03 << k*2)>> k*2);
										if(x+i<width&&y+j<height)
										{
											uint offset = (uint)(z*sizeofplane+(y+j)*bps+(x+i)*bpp);
											rawData[offset+0] = colors[select].red;
											rawData[offset+1] = colors[select].green;
											rawData[offset+2] = colors[select].blue;
										}
									}
								}
								for(int j=0;j<4;j++)
								{
									ushort word = (ushort)(alpha[2*j] | alpha[2*j+1] << 8);
									for(int i=0;i<4;i++){
										if(x+i<width&&y+j<height){
											uint offset = (uint)(z*sizeofplane+(y+j)*bps+(x+i)*bpp+3);
											rawData[offset] = (byte)(word & 0x0F);
											rawData[offset] = (byte)(rawData[offset] | rawData[offset] << 4);
										}
										word>>=	4;
									}
								}
							}
						}
					}
				}
				return rawData;
			}
			private static byte[] DecompressDXT4(DDSHeader header,byte[] data,DDSPixelFormat pixelFormat)
			{
				int width = (int)header.width;
				int height = (int)header.height;
				int depth = (int)header.depth;
				var rawData = DecompressDXT5(header,data,pixelFormat);
				DDSHelper.CorrectPremult((uint)(width*height*depth),ref rawData);
				return rawData;
			}
			private static unsafe byte[] DecompressDXT5(DDSHeader header,byte[] data,DDSPixelFormat pixelFormat)
			{
				int bpp = (int)DDSHelper.PixelFormatToBpp(pixelFormat,header.pixelformat.rgbbitcount);
				int bps = (int)(header.width*bpp*DDSHelper.PixelFormatToBpc(pixelFormat));
				int sizeofplane = (int)(bps*header.height);
				int width = (int)header.width;
				int height = (int)header.height;
				int depth = (int)header.depth;
				var rawData = new byte[depth*sizeofplane+height*bps+width*bpp];
				var colors = new Color8888[4];
				var alphas = new ushort[8];
				fixed(byte*bytePtr = data){
					var temp = bytePtr;
					for(int z=0;z<depth;z++){
						for(int y=0;y<height;y += 4){
							for(int x=0;x<width;x += 4){
								if(y>=height || x>=width){
									break;
								}
								alphas[0] = temp[0];
								alphas[1] = temp[1];
								var alphamask = temp+2;
								temp += 8;
								DDSHelper.DxtcReadColors(temp,ref colors);
								uint bitmask = ((uint*)temp)[1];
								temp += 8;
								colors[2].blue = (byte)((2*colors[0].blue+colors[1].blue+1)/3);
								colors[2].green = (byte)((2*colors[0].green+colors[1].green+1)/3);
								colors[2].red = (byte)((2*colors[0].red+colors[1].red+1)/3);
								colors[3].blue = (byte)((colors[0].blue+2*colors[1].blue+1)/3);
								colors[3].green = (byte)((colors[0].green+2*colors[1].green+1)/3);
								colors[3].red = (byte)((colors[0].red+2*colors[1].red+1)/3);
								int k = 0;
								for(int j=0;j<4;j++){
									for(int i=0;i<4;k++,i++){
										int select = (int)((bitmask & 0x03 << k*2)>> k*2);
										var col = colors[select];
										if(x+i<width&&y+j<height)
										{
											uint offset = (uint)(z*sizeofplane+(y+j)*bps+(x+i)*bpp);
											rawData[offset] = col.red;
											rawData[offset+1] = col.green;
											rawData[offset+2] = col.blue;
										}
									}
								}
								if(alphas[0]>alphas[1]){
									alphas[2] = (ushort)((6*alphas[0]+1*alphas[1]+3)/7);
									alphas[3] = (ushort)((5*alphas[0]+2*alphas[1]+3)/7);
									alphas[4] = (ushort)((4*alphas[0]+3*alphas[1]+3)/7);
									alphas[5] = (ushort)((3*alphas[0]+4*alphas[1]+3)/7);
									alphas[6] = (ushort)((2*alphas[0]+5*alphas[1]+3)/7);
									alphas[7] = (ushort)((1*alphas[0]+6*alphas[1]+3)/7);
								}else{
									alphas[2] = (ushort)((4*alphas[0]+1*alphas[1]+2)/5);
									alphas[3] = (ushort)((3*alphas[0]+2*alphas[1]+2)/5);
									alphas[4] = (ushort)((2*alphas[0]+3*alphas[1]+2)/5);
									alphas[5] = (ushort)((1*alphas[0]+4*alphas[1]+2)/5);
									alphas[6] = 0x00;
									alphas[7] = 0xFF;
								}
								uint bits = (uint)(alphamask[0]|alphamask[1] << 8|alphamask[2] << 16);
								for(int j=0;j<2;j++){
									for(int i=0;i<4;i++){
										if(x+i<width&&y+j<height){
											uint offset = (uint)(z*sizeofplane+(y+j)*bps+(x+i)*bpp+3);
											rawData[offset] = (byte)alphas[bits & 0x07];
										}
										bits>>=	3;
									}
								}
								bits = (uint)(alphamask[3]|alphamask[4] << 8|alphamask[5] << 16);
								for(int j=2;j<4;j++){
									for(int i=0;i<4;i++){
										if(x+i<width&&y+j<height){
											uint offset = (uint)(z*sizeofplane+(y+j)*bps+(x+i)*bpp+3);
											rawData[offset] = (byte)alphas[bits & 0x07];
										}
										bits>>=	3;
									}
								}
							}
						}
					}
				}
				return rawData;
			}
			//not cleaned up past this ^
			private static unsafe byte[] DecompressRGB(DDSHeader header,byte[] data,DDSPixelFormat pixelFormat)
			{
				int bpp = (int)DDSHelper.PixelFormatToBpp(pixelFormat,header.pixelformat.rgbbitcount);
				int bps = (int)(header.width*bpp*DDSHelper.PixelFormatToBpc(pixelFormat));
				int sizeofplane = (int)(bps*header.height);
				int width = (int)header.width;
				int height = (int)header.height;
				int depth = (int)header.depth;
				var rawData = new byte[depth*sizeofplane+height*bps+width*bpp];
				uint valMask = (uint)(header.pixelformat.rgbbitcount==32? ~0 :(1 <<(int)header.pixelformat.rgbbitcount)-1);
				uint pixSize = (uint)(((int)header.pixelformat.rgbbitcount+7)/8);
				int rShift1 = 0;int rMul = 0;int rShift2 = 0;
				DDSHelper.ComputeMaskParams(header.pixelformat.rbitmask,ref rShift1,ref rMul,ref rShift2);
				int gShift1 = 0;int gMul = 0;int gShift2 = 0;
				DDSHelper.ComputeMaskParams(header.pixelformat.gbitmask,ref gShift1,ref gMul,ref gShift2);
				int bShift1 = 0;int bMul = 0;int bShift2 = 0;
				DDSHelper.ComputeMaskParams(header.pixelformat.bbitmask,ref bShift1,ref bMul,ref bShift2);
				int offset = 0;
				int pixnum = width*height*depth;
				fixed(byte*bytePtr = data)
				{
					var temp = bytePtr;
					while(pixnum-->0)
					{
						uint px=	*(uint*)temp& valMask;
						temp += pixSize;
						uint pxc = px & header.pixelformat.rbitmask;
						rawData[offset+0] = (byte)((pxc >> rShift1)*rMul>> rShift2);
						pxc = px & header.pixelformat.gbitmask;
						rawData[offset+1] = (byte)((pxc >> gShift1)*gMul>> gShift2);
						pxc = px & header.pixelformat.bbitmask;
						rawData[offset+2] = (byte)((pxc >> bShift1)*bMul>> bShift2);
						rawData[offset+3] = 0xff;
						offset += 4;
					}
				}
				return rawData;
			}
			private static unsafe byte[] DecompressRGBA(DDSHeader header,byte[] data,DDSPixelFormat pixelFormat)
			{
				int bpp = (int)DDSHelper.PixelFormatToBpp(pixelFormat,header.pixelformat.rgbbitcount);
				int bps = (int)(header.width*bpp*DDSHelper.PixelFormatToBpc(pixelFormat));
				int sizeofplane = (int)(bps*header.height);
				int width = (int)header.width;
				int height = (int)header.height;
				int depth = (int)header.depth;
				var rawData = new byte[depth*sizeofplane+height*bps+width*bpp];
				uint valMask = (uint)(header.pixelformat.rgbbitcount==32? ~0 :(1 <<(int)header.pixelformat.rgbbitcount)-1);
				uint pixSize = (header.pixelformat.rgbbitcount+7)/8;
				int rShift1 = 0;int rMul = 0;int rShift2 = 0;
				DDSHelper.ComputeMaskParams(header.pixelformat.rbitmask,ref rShift1,ref rMul,ref rShift2);
				int gShift1 = 0;int gMul = 0;int gShift2 = 0;
				DDSHelper.ComputeMaskParams(header.pixelformat.gbitmask,ref gShift1,ref gMul,ref gShift2);
				int bShift1 = 0;int bMul = 0;int bShift2 = 0;
				DDSHelper.ComputeMaskParams(header.pixelformat.bbitmask,ref bShift1,ref bMul,ref bShift2);
				int aShift1 = 0;int aMul = 0;int aShift2 = 0;
				DDSHelper.ComputeMaskParams(header.pixelformat.alphabitmask,ref aShift1,ref aMul,ref aShift2);
				int offset = 0;
				int pixnum = width*height*depth;
				fixed(byte*bytePtr = data)
				{
					var temp = bytePtr;
					while(pixnum-->0)
					{
						uint px=	*(uint*)temp& valMask;
						temp += pixSize;
						uint pxc = px & header.pixelformat.rbitmask;
						rawData[offset+0] = (byte)((pxc >> rShift1)*rMul>> rShift2);
						pxc = px & header.pixelformat.gbitmask;
						rawData[offset+1] = (byte)((pxc >> gShift1)*gMul>> gShift2);
						pxc = px & header.pixelformat.bbitmask;
						rawData[offset+2] = (byte)((pxc >> bShift1)*bMul>> bShift2);
						pxc = px & header.pixelformat.alphabitmask;
						rawData[offset+3] = (byte)((pxc >> aShift1)*aMul>> aShift2);
						offset += 4;
					}
				}
				return rawData;
			}
			private static unsafe byte[] Decompress3Dc(DDSHeader header,byte[] data,DDSPixelFormat pixelFormat)
			{
				int bpp = (int)DDSHelper.PixelFormatToBpp(pixelFormat,header.pixelformat.rgbbitcount);
				int bps = (int)(header.width*bpp*DDSHelper.PixelFormatToBpc(pixelFormat));
				int sizeofplane = (int)(bps*header.height);
				int width = (int)header.width;
				int height = (int)header.height;
				int depth = (int)header.depth;
				var rawData = new byte[depth*sizeofplane+height*bps+width*bpp];
				var yColours = new byte[8];
				var xColours = new byte[8];
				int offset = 0;
				fixed(byte*bytePtr = data)
				{
					var temp = bytePtr;
					for(int z=	0;z<depth;z++)
					{
						for(int y=	0;y<height;y += 4)
						{
							for(int x=	0;x<width;x += 4)
							{
								var temp2 = temp+8;
								int t1 = yColours[0] = temp[0];
								int t2 = yColours[1] = temp[1];
								temp += 2;
								if(t1>t2)
									for(int i=	2;i<8;++i)
										yColours[i] = (byte)(t1+(t2-t1)*(i-1)/7);else
								{
									for(int i=	2;i<6;++i)
										yColours[i] = (byte)(t1+(t2-t1)*(i-1)/5);
									yColours[6] = 0;
									yColours[7] = 255;
								}
								t1 = xColours[0] = temp2[0];
								t2 = xColours[1] = temp2[1];
								temp2 += 2;
								if(t1>t2)
									for(int i=	2;i<8;++i)
										xColours[i] = (byte)(t1+(t2-t1)*(i-1)/7);else
								{
									for(int i=	2;i<6;++i)
										xColours[i] = (byte)(t1+(t2-t1)*(i-1)/5);
									xColours[6] = 0;
									xColours[7] = 255;
								}
								int currentOffset = offset;
								for(int k=	0;k<4;k += 2)
								{
									uint bitmask = (uint)temp[0]<< 0|(uint)temp[1]<< 8|(uint)temp[2]<< 16;
									uint bitmask2 = (uint)temp2[0]<< 0|(uint)temp2[1]<< 8|(uint)temp2[2]<< 16;
									for(int j=	0;j<2;j++)
									{
										if(y+k+j<height)
										{
											for(int i=	0;i<4;i++)
											{
												if(x+i<width)
												{
													int t;
													byte tx,ty;
													t1 = currentOffset+(x+i)*3;
													rawData[t1+1] = ty = yColours[bitmask & 0x07];
													rawData[t1+0] = tx = xColours[bitmask2 & 0x07];
													t = 127*128-(tx-127)*(tx-128)-(ty-127)*(ty-128);
													if(t>0)
														rawData[t1+2] = (byte)(Math.Sqrt(t)+128);else
														rawData[t1+2] = 0x7F;
												}
												bitmask >>=3;
												bitmask2 >>=3;
											}
											currentOffset += bps;
										}
									}
									temp += 3;
									temp2 += 3;
								}
								temp += 8;
							}
							offset += bps*4;
						}
					}
				}
				return rawData;
			}
			private static unsafe byte[] DecompressAti1n(DDSHeader header,byte[] data,DDSPixelFormat pixelFormat)
			{
				int bpp = (int)DDSHelper.PixelFormatToBpp(pixelFormat,header.pixelformat.rgbbitcount);
				int bps = (int)(header.width*bpp*DDSHelper.PixelFormatToBpc(pixelFormat));
				int sizeofplane = (int)(bps*header.height);
				int width = (int)header.width;
				int height = (int)header.height;
				int depth = (int)header.depth;
				var rawData = new byte[depth*sizeofplane+height*bps+width*bpp];
				var colors = new byte[8];
				uint offset = 0;
				fixed(byte*bytePtr = data)
				{
					var temp = bytePtr;
					for(int z=	0;z<depth;z++)
					{
						for(int y=	0;y<height;y += 4)
						{
							for(int x=	0;x<width;x += 4)
							{
								int t1 = colors[0] = temp[0];
								int t2 = colors[1] = temp[1];
								temp += 2;
								if(t1>t2)
									for(int i=	2;i<8;++i)
										colors[i] = (byte)(t1+(t2-t1)*(i-1)/7);else
								{
									for(int i=	2;i<6;++i)
										colors[i] = (byte)(t1+(t2-t1)*(i-1)/5);
									colors[6] = 0;
									colors[7] = 255;
								}
								uint currOffset = offset;
								for(int k=	0;k<4;k += 2)
								{
									uint bitmask = (uint)temp[0]<< 0|(uint)temp[1]<< 8|(uint)temp[2]<< 16;
									for(int j=	0;j<2;j++)
									{
										if(y+k+j<height)
										{
											for(int i=	0;i<4;i++)
											{
												if(x+i<width)
												{
													t1 = (int)(currOffset+(x+i));
													rawData[t1] = colors[bitmask & 0x07];
												}
												bitmask >>=3;
											}
											currOffset += (uint)bps;
										}
									}
									temp += 3;
								}
							}
							offset += (uint)(bps*4);
						}
					}
				}
				return rawData;
			}
			private static unsafe byte[] DecompressLum(DDSHeader header,byte[] data,DDSPixelFormat pixelFormat)
			{
				int bpp = (int)DDSHelper.PixelFormatToBpp(pixelFormat,header.pixelformat.rgbbitcount);
				int bps = (int)(header.width*bpp*DDSHelper.PixelFormatToBpc(pixelFormat));
				int planeSize = (int)(bps*header.height);
				int width = (int)header.width;
				int height = (int)header.height;
				int depth = (int)header.depth;
				var rawData = new byte[depth*planeSize+height*bps+width*bpp];
				int lShift1 = 0;
				int lMul = 0;
				int lShift2 = 0;
				DDSHelper.ComputeMaskParams(header.pixelformat.rbitmask,ref lShift1,ref lMul,ref lShift2);
				int offset = 0;
				int pixnum = width*height*depth;
				fixed(byte*bytePtr = data)
				{
					var temp = bytePtr;
					while(pixnum-->0)
					{
						byte px=	*temp++;
						rawData[offset+0] = (byte)((px >> lShift1)*lMul>> lShift2);
						rawData[offset+1] = (byte)((px >> lShift1)*lMul>> lShift2);
						rawData[offset+2] = (byte)((px >> lShift1)*lMul>> lShift2);
						rawData[offset+3] = (byte)((px >> lShift1)*lMul>> lShift2);
						offset += 4;
					}
				}
				return rawData;
			}
			private static unsafe byte[] DecompressRXGB(DDSHeader header,byte[] data,DDSPixelFormat pixelFormat)
			{
				int bpp = (int)DDSHelper.PixelFormatToBpp(pixelFormat,header.pixelformat.rgbbitcount);
				int bps = (int)(header.width*bpp*DDSHelper.PixelFormatToBpc(pixelFormat));
				int sizeofplane = (int)(bps*header.height);
				int width = (int)header.width;
				int height = (int)header.height;
				int depth = (int)header.depth;
				var rawData = new byte[depth*sizeofplane+height*bps+width*bpp];
				var color_0 = new Color565();
				var color_1 = new Color565();
				var colors = new Color8888[4];
				var alphas = new byte[8];
				fixed(byte*bytePtr = data)
				{
					var temp = bytePtr;
					for(int z=	0;z<depth;z++)
					{
						for(int y=	0;y<height;y += 4)
						{
							for(int x=	0;x<width;x += 4)
							{
								if(y>=height || x>=width)
									break;
								alphas[0] = temp[0];
								alphas[1] = temp[1];
								var alphamask = temp+2;
								temp += 8;
								DDSHelper.DxtcReadColors(temp,ref color_0,ref color_1);
								temp += 4;
								uint bitmask = ((uint*)temp)[1];
								temp += 4;
								colors[0].red = (byte)(color_0.red << 3);
								colors[0].green = (byte)(color_0.green << 2);
								colors[0].blue = (byte)(color_0.blue << 3);
								colors[0].alpha = 0xFF;
								colors[1].red = (byte)(color_1.red << 3);
								colors[1].green = (byte)(color_1.green << 2);
								colors[1].blue = (byte)(color_1.blue << 3);
								colors[1].alpha = 0xFF;
								colors[2].blue = (byte)((2*colors[0].blue+colors[1].blue+1)/3);
								colors[2].green = (byte)((2*colors[0].green+colors[1].green+1)/3);
								colors[2].red = (byte)((2*colors[0].red+colors[1].red+1)/3);
								colors[2].alpha = 0xFF;
								colors[3].blue = (byte)((colors[0].blue+2*colors[1].blue+1)/3);
								colors[3].green = (byte)((colors[0].green+2*colors[1].green+1)/3);
								colors[3].red = (byte)((colors[0].red+2*colors[1].red+1)/3);
								colors[3].alpha = 0xFF;
								int k = 0;
								for(int j=	0;j<4;j++)
								{
									for(int i=	0;i<4;i++,k++)
									{
										int select = (int)((bitmask & 0x03 << k*2)>> k*2);
										var col = colors[select];
										if(x+i<width&&y+j<height)
										{
											uint offset = (uint)(z*sizeofplane+(y+j)*bps+(x+i)*bpp);
											rawData[offset+0] = col.red;
											rawData[offset+1] = col.green;
											rawData[offset+2] = col.blue;
										}
									}
								}
								if(alphas[0]>alphas[1])
								{
									alphas[2] = (byte)((6*alphas[0]+1*alphas[1]+3)/7);	
									alphas[3] = (byte)((5*alphas[0]+2*alphas[1]+3)/7);	
									alphas[4] = (byte)((4*alphas[0]+3*alphas[1]+3)/7);	
									alphas[5] = (byte)((3*alphas[0]+4*alphas[1]+3)/7);	
									alphas[6] = (byte)((2*alphas[0]+5*alphas[1]+3)/7);	
									alphas[7] = (byte)((1*alphas[0]+6*alphas[1]+3)/7);	
								}else
								{
									alphas[2] = (byte)((4*alphas[0]+1*alphas[1]+2)/5);	
									alphas[3] = (byte)((3*alphas[0]+2*alphas[1]+2)/5);	
									alphas[4] = (byte)((2*alphas[0]+3*alphas[1]+2)/5);	
									alphas[5] = (byte)((1*alphas[0]+4*alphas[1]+2)/5);	
									alphas[6] = 0x00;									
									alphas[7] = 0xFF;									
								}
								uint bits=	*(uint*)alphamask;
								for(int j=	0;j<2;j++)
								{
									for(int i=	0;i<4;i++)
									{
										if(x+i<width&&y+j<height)
										{
											uint offset = (uint)(z*sizeofplane+(y+j)*bps+(x+i)*bpp+3);
											rawData[offset] = alphas[bits & 0x07];
										}
										bits >>=3;
									}
								}
								bits=	*(uint*)&alphamask[3];
								for(int j=	2;j<4;j++)
								{
									for(int i=	0;i<4;i++)
									{
										if(x+i<width&&y+j<height)
										{
											uint offset = (uint)(z*sizeofplane+(y+j)*bps+(x+i)*bpp+3);
											rawData[offset] = alphas[bits & 0x07];
										}
										bits >>=3;
									}
								}
							}
						}
					}
				}
				return rawData;
			}
			private static unsafe byte[] DecompressFloat(DDSHeader header,byte[] data,DDSPixelFormat pixelFormat)
			{
				int bpp = (int)DDSHelper.PixelFormatToBpp(pixelFormat,header.pixelformat.rgbbitcount);
				int bps = (int)(header.width*bpp*DDSHelper.PixelFormatToBpc(pixelFormat));
				int planeSize = (int)(bps*header.height);
				int width = (int)header.width;
				int height = (int)header.height;
				int depth = (int)header.depth;
				var rawData = new byte[depth*planeSize+height*bps+width*bpp];
				int size = 0;
				fixed(byte*bytePtr = data)
				{
					var temp = bytePtr;
					fixed(byte*destPtr = rawData)
					{
						var destData = destPtr;
						switch(pixelFormat)
						{
							case DDSPixelFormat.R32F: 
								size = width*height*depth*3;
								for(int i=0,j=0;i<size;i += 3,j++)
								{
									((float*)destData)[i] = ((float*)temp)[j];
									((float*)destData)[i+1] = 1.0f;
									((float*)destData)[i+2] = 1.0f;
								}
								break;
							case DDSPixelFormat.A32B32G32R32F: 
								Array.Copy(data,rawData,data.Length);
								break;
							case DDSPixelFormat.G32R32F: 
								size = width*height*depth*3;
								for(int i=0,j=0;i<size;i += 3,j += 2)
								{
									((float*)destData)[i] = ((float*)temp)[j];
									((float*)destData)[i+1] = ((float*)temp)[j+1];
									((float*)destData)[i+2] = 1.0f;
								}
								break;
							case DDSPixelFormat.R16F: 
								size = width*height*depth*bpp;
								DDSHelper.ConvR16ToFloat32((uint*)destData,(ushort*)temp,(uint)size);
								break;
							case DDSPixelFormat.A16B16G16R16F: 
								size = width*height*depth*bpp;
								DDSHelper.ConvFloat16ToFloat32((uint*)destData,(ushort*)temp,(uint)size);
								break;
							case DDSPixelFormat.G16R16F: 
								size = width*height*depth*bpp;
								DDSHelper.ConvG16R16ToFloat32((uint*)destData,(ushort*)temp,(uint)size);
								break;
							default:
								break;
						}
					}
				}
				return rawData;
			}
		}
	}
}