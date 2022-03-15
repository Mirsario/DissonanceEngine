using System;
using System.Collections.Generic;
using Silk.NET.OpenGL;

namespace Dissonance.Engine.Graphics
{
	partial class Rendering
	{
		//TODO: Move this
		public static Dictionary<TextureFormat, (PixelFormat formatGeneral, InternalFormat formatInternal, PixelType pixelType, Type dataType)> textureFormatInfo = new() {
			// A

			{ TextureFormat.A8,		(PixelFormat.Alpha,	InternalFormat.R8,	PixelType.UnsignedByte,	typeof(byte)) },	// 8
			{ TextureFormat.A16,	(PixelFormat.Alpha,	InternalFormat.R16,	PixelType.UnsignedByte,	typeof(byte)) },	// 16
			
			// R

			{ TextureFormat.R8,		(PixelFormat.Red,			InternalFormat.R8,		PixelType.UnsignedByte,		typeof(byte)) },	// 8
			{ TextureFormat.R8i,	(PixelFormat.RedInteger,	InternalFormat.R8i,		PixelType.Byte,				typeof(sbyte)) },
			{ TextureFormat.R8ui,	(PixelFormat.RedInteger,	InternalFormat.R8ui,	PixelType.UnsignedByte,		typeof(byte)) },
			{ TextureFormat.R16,	(PixelFormat.Red,			InternalFormat.R16,		PixelType.UnsignedShort,	typeof(ushort)) },	// 16
			//{ TextureFormat.R16f,	(PixelFormat.Red,			InternalFormat.R16f,	PixelType.HalfFloat,		typeof(Half)) },
			{ TextureFormat.R16i,	(PixelFormat.RedInteger,	InternalFormat.R16i,	PixelType.Short,			typeof(short)) },
			{ TextureFormat.R16ui,	(PixelFormat.RedInteger,	InternalFormat.R16ui,	PixelType.UnsignedShort,	typeof(ushort)) },
			{ TextureFormat.R32f,	(PixelFormat.Red,			InternalFormat.R32f,	PixelType.Float,			typeof(float)) },	// 32
			{ TextureFormat.R32i,	(PixelFormat.RedInteger,	InternalFormat.R32i,	PixelType.Int,				typeof(int)) },
			{ TextureFormat.R32ui,	(PixelFormat.RedInteger,	InternalFormat.R32ui,	PixelType.UnsignedInt,		typeof(uint)) },
			
			// RG

			{ TextureFormat.RG8,		(PixelFormat.RG,		InternalFormat.RG8,		PixelType.UnsignedByte,		typeof(byte)) },	// 8
			{ TextureFormat.RG8i,		(PixelFormat.RGInteger,	InternalFormat.RG8i,	PixelType.Byte,				typeof(sbyte)) },
			{ TextureFormat.RG8ui,		(PixelFormat.RGInteger,	InternalFormat.RG8ui,	PixelType.UnsignedByte,		typeof(byte)) },
			{ TextureFormat.RG16,		(PixelFormat.RG,		InternalFormat.RG16,	PixelType.UnsignedShort,	typeof(ushort)) },	// 16
			//{ TextureFormat.RG16f,	(PixelFormat.Rg,		InternalFormat.Rg16f,	PixelType.HalfFloat,		typeof(Half)) },
			{ TextureFormat.RG16i,		(PixelFormat.RGInteger,	InternalFormat.RG16i,	PixelType.Short,			typeof(short)) },
			{ TextureFormat.RG16ui,		(PixelFormat.RGInteger,	InternalFormat.RG16ui,	PixelType.UnsignedShort,	typeof(ushort)) },
			{ TextureFormat.RG32f,		(PixelFormat.RG,		InternalFormat.RG32f,	PixelType.Float,			typeof(float)) },	// 32
			{ TextureFormat.RG32i,		(PixelFormat.RGInteger,	InternalFormat.RG32i,	PixelType.Int,				typeof(int)) },
			{ TextureFormat.RG32ui,		(PixelFormat.RGInteger,	InternalFormat.RG32ui,	PixelType.UnsignedInt,		typeof(uint)) },
			
			// RGB

			{ TextureFormat.RGB8,		(PixelFormat.Rgb,			InternalFormat.Rgb8,		PixelType.UnsignedByte,		typeof(byte)) },	// 8
			{ TextureFormat.RGB8i,		(PixelFormat.RgbInteger,	InternalFormat.Rgb8i,		PixelType.Byte,				typeof(sbyte)) },
			{ TextureFormat.RGB8ui,		(PixelFormat.RgbInteger,	InternalFormat.Rgb8ui,		PixelType.UnsignedByte,		typeof(byte)) },
			{ TextureFormat.RGB16,		(PixelFormat.Rgb,			InternalFormat.Rgb16,		PixelType.UnsignedShort,	typeof(ushort)) },	// 16
			//{ TextureFormat.RGB16f,	(PixelFormat.Rgb,			InternalFormat.Rgb16f,		PixelType.HalfFloat,		typeof(Half)) },
			{ TextureFormat.RGB16i,		(PixelFormat.RgbInteger,	InternalFormat.Rgb16,		PixelType.Short,			typeof(short)) },
			{ TextureFormat.RGB16ui,	(PixelFormat.RgbInteger,	InternalFormat.Rgb16ui,		PixelType.UnsignedShort,	typeof(ushort)) },
			{ TextureFormat.RGB32f,		(PixelFormat.Rgb,			InternalFormat.Rgb32f,		PixelType.Float,			typeof(float)) },	// 32
			{ TextureFormat.RGB32i,		(PixelFormat.RgbInteger,	InternalFormat.Rgb32i,		PixelType.Int,				typeof(int)) },
			{ TextureFormat.RGB32ui,	(PixelFormat.RgbInteger,	InternalFormat.Rgb32ui,		PixelType.UnsignedInt,		typeof(uint)) },

			// RGBA

			{ TextureFormat.RGBA8,		(PixelFormat.Rgba,			InternalFormat.Rgba8,		PixelType.UnsignedByte,		typeof(byte)) },	// 8
			{ TextureFormat.RGBA8i,		(PixelFormat.RgbaInteger,	InternalFormat.Rgba8i,		PixelType.Byte,				typeof(sbyte)) },
			{ TextureFormat.RGBA8ui,	(PixelFormat.RgbaInteger,	InternalFormat.Rgba8ui,		PixelType.UnsignedByte,		typeof(byte)) },
			{ TextureFormat.RGBA16,		(PixelFormat.Rgba,			InternalFormat.Rgba16,		PixelType.UnsignedShort,	typeof(ushort)) },	// 16
			//{ TextureFormat.RGBA16f,	(PixelFormat.Rgba,			InternalFormat.Rgba16f,		PixelType.HalfFloat,		typeof(Half)) },
			{ TextureFormat.RGBA16i,	(PixelFormat.RgbaInteger,	InternalFormat.Rgba16,		PixelType.Short,			typeof(short)) },
			{ TextureFormat.RGBA16ui,	(PixelFormat.RgbaInteger,	InternalFormat.Rgba16ui,	PixelType.UnsignedShort,	typeof(ushort)) },
			{ TextureFormat.RGBA32f,	(PixelFormat.Rgba,			InternalFormat.Rgba32f,		PixelType.Float,			typeof(float)) },	// 32
			{ TextureFormat.RGBA32i,	(PixelFormat.RgbaInteger,	InternalFormat.Rgba32i,		PixelType.Int,				typeof(int)) },
			{ TextureFormat.RGBA32ui,	(PixelFormat.RgbaInteger,	InternalFormat.Rgba32ui,	PixelType.UnsignedInt,		typeof(uint)) },

			// DepthComponent

			{ TextureFormat.Depth16,	(PixelFormat.DepthComponent,	InternalFormat.DepthComponent16,	PixelType.UnsignedShort,	typeof(ushort)) },	// 16
			{ TextureFormat.Depth32,	(PixelFormat.DepthComponent,	InternalFormat.DepthComponent32,	PixelType.UnsignedInt,		typeof(uint)) },
			{ TextureFormat.Depth32f,	(PixelFormat.DepthComponent,	InternalFormat.DepthComponent32f,	PixelType.Float,			typeof(float)) },	// 32

			// DepthStencil

			{ TextureFormat.Depth24Stencil8,	(PixelFormat.DepthStencil,	InternalFormat.Depth24Stencil8,		(PixelType)GLEnum.UnsignedInt248,			typeof(int)) },		// 16+8
			{ TextureFormat.Depth32fStencil8,	(PixelFormat.DepthStencil,	InternalFormat.Depth32fStencil8,	(PixelType)GLEnum.Float32UnsignedInt248Rev,	typeof(float)) },	// 24+8
		};
	}
}
