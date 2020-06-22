using Dissonance.Engine.Graphics.Enums;
using Dissonance.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Dissonance.Engine.Graphics
{
	partial class Rendering
	{
		//TODO: Move this
		public static Dictionary<TextureFormat,(PixelFormat formatGeneral, PixelInternalFormat formatInternal, PixelType pixelType, Type dataType)> textureFormatInfo = new Dictionary<TextureFormat,(PixelFormat, PixelInternalFormat, PixelType, Type)> {
			#region A

			{ TextureFormat.A8,                 (PixelFormat.Alpha,             PixelInternalFormat.R8,                 PixelType.UnsignedByte,             typeof(byte)) },	//8
			{ TextureFormat.A16,                (PixelFormat.Alpha,             PixelInternalFormat.R16,                PixelType.UnsignedByte,             typeof(byte)) },    //16
			
			#endregion
			
			#region R

			{ TextureFormat.R8,                 (PixelFormat.Red,               PixelInternalFormat.R8,                 PixelType.UnsignedByte,             typeof(byte)) },	//8
			{ TextureFormat.R8i,                (PixelFormat.RedInteger,        PixelInternalFormat.R8i,                PixelType.Byte,                     typeof(sbyte)) },
			{ TextureFormat.R8ui,               (PixelFormat.RedInteger,        PixelInternalFormat.R8ui,               PixelType.UnsignedByte,             typeof(byte)) },
			{ TextureFormat.R16,                (PixelFormat.Red,               PixelInternalFormat.R16,                PixelType.UnsignedShort,            typeof(ushort)) },	//16
			//{ TextureFormat.R16f,				(PixelFormat.Red,				PixelInternalFormat.R16f,				PixelType.HalfFloat,				typeof(Half)) },
			{ TextureFormat.R16i,               (PixelFormat.RedInteger,        PixelInternalFormat.R16i,               PixelType.Short,                    typeof(short)) },
			{ TextureFormat.R16ui,              (PixelFormat.RedInteger,        PixelInternalFormat.R16ui,              PixelType.UnsignedShort,            typeof(ushort)) },
			{ TextureFormat.R32f,               (PixelFormat.Red,               PixelInternalFormat.R32f,               PixelType.Float,                    typeof(float)) },	//32
			{ TextureFormat.R32i,               (PixelFormat.RedInteger,        PixelInternalFormat.R32i,               PixelType.Int,                      typeof(int)) },
			{ TextureFormat.R32ui,              (PixelFormat.RedInteger,        PixelInternalFormat.R32ui,              PixelType.UnsignedInt,              typeof(uint)) },
			
			#endregion
			
			#region RG

			{ TextureFormat.RG8,                (PixelFormat.Rg,                PixelInternalFormat.Rg8,                PixelType.UnsignedByte,             typeof(byte)) },	//8
			{ TextureFormat.RG8i,               (PixelFormat.RgInteger,         PixelInternalFormat.Rg8i,               PixelType.Byte,                     typeof(sbyte)) },
			{ TextureFormat.RG8ui,              (PixelFormat.RgInteger,         PixelInternalFormat.Rg8ui,              PixelType.UnsignedByte,             typeof(byte)) },
			{ TextureFormat.RG16,               (PixelFormat.Rg,                PixelInternalFormat.Rg16,               PixelType.UnsignedShort,            typeof(ushort)) },	//16
			//{ TextureFormat.RG16f,			(PixelFormat.Rg,				PixelInternalFormat.Rg16f,				PixelType.HalfFloat,				typeof(Half)) },
			{ TextureFormat.RG16i,              (PixelFormat.RgInteger,         PixelInternalFormat.Rg16i,              PixelType.Short,                    typeof(short)) },
			{ TextureFormat.RG16ui,             (PixelFormat.RgInteger,         PixelInternalFormat.Rg16ui,             PixelType.UnsignedShort,            typeof(ushort)) },
			{ TextureFormat.RG32f,              (PixelFormat.Rg,                PixelInternalFormat.Rg32f,              PixelType.Float,                    typeof(float)) },	//32
			{ TextureFormat.RG32i,              (PixelFormat.RgInteger,         PixelInternalFormat.Rg32i,              PixelType.Int,                      typeof(int)) },
			{ TextureFormat.RG32ui,             (PixelFormat.RgInteger,         PixelInternalFormat.Rg32ui,             PixelType.UnsignedInt,              typeof(uint)) },
			
			#endregion

			#region RGB

			{ TextureFormat.RGB8,               (PixelFormat.Rgb,               PixelInternalFormat.Rgb8,               PixelType.UnsignedByte,             typeof(byte)) },	//8
			{ TextureFormat.RGB8i,              (PixelFormat.RgbInteger,        PixelInternalFormat.Rgb8i,              PixelType.Byte,                     typeof(sbyte)) },
			{ TextureFormat.RGB8ui,             (PixelFormat.RgbInteger,        PixelInternalFormat.Rgb8ui,             PixelType.UnsignedByte,             typeof(byte)) },
			{ TextureFormat.RGB16,              (PixelFormat.Rgb,               PixelInternalFormat.Rgb16,              PixelType.UnsignedShort,            typeof(ushort)) },	//16
			//{ TextureFormat.RGB16f,			(PixelFormat.Rgb,				PixelInternalFormat.Rgb16f,				PixelType.HalfFloat,				typeof(Half)) },
			{ TextureFormat.RGB16i,             (PixelFormat.RgbInteger,        PixelInternalFormat.Rgb16,              PixelType.Short,                    typeof(short)) },
			{ TextureFormat.RGB16ui,            (PixelFormat.RgbInteger,        PixelInternalFormat.Rgb16ui,            PixelType.UnsignedShort,            typeof(ushort)) },
			{ TextureFormat.RGB32f,             (PixelFormat.Rgb,               PixelInternalFormat.Rgb32f,             PixelType.Float,                    typeof(float)) },	//32
			{ TextureFormat.RGB32i,             (PixelFormat.RgbInteger,        PixelInternalFormat.Rgb32i,             PixelType.Int,                      typeof(int)) },
			{ TextureFormat.RGB32ui,            (PixelFormat.RgbInteger,        PixelInternalFormat.Rgb32ui,            PixelType.UnsignedInt,              typeof(uint)) },

			#endregion

			#region RGBA

			{ TextureFormat.RGBA8,              (PixelFormat.Rgba,              PixelInternalFormat.Rgba8,              PixelType.UnsignedByte,             typeof(byte)) },	//8
			{ TextureFormat.RGBA8i,             (PixelFormat.RgbaInteger,       PixelInternalFormat.Rgba8i,             PixelType.Byte,                     typeof(sbyte)) },
			{ TextureFormat.RGBA8ui,            (PixelFormat.RgbaInteger,       PixelInternalFormat.Rgba8ui,            PixelType.UnsignedByte,             typeof(byte)) },
			{ TextureFormat.RGBA16,             (PixelFormat.Rgba,              PixelInternalFormat.Rgba16,             PixelType.UnsignedShort,            typeof(ushort)) },	//16
			//{ TextureFormat.RGBA16f,			(PixelFormat.Rgba,				PixelInternalFormat.Rgba16f,			PixelType.HalfFloat,				typeof(Half)) },
			{ TextureFormat.RGBA16i,            (PixelFormat.RgbaInteger,       PixelInternalFormat.Rgba16,             PixelType.Short,                    typeof(short)) },
			{ TextureFormat.RGBA16ui,           (PixelFormat.RgbaInteger,       PixelInternalFormat.Rgba16ui,           PixelType.UnsignedShort,            typeof(ushort)) },
			{ TextureFormat.RGBA32f,            (PixelFormat.Rgba,              PixelInternalFormat.Rgba32f,            PixelType.Float,                    typeof(float)) },	//32
			{ TextureFormat.RGBA32i,            (PixelFormat.RgbaInteger,       PixelInternalFormat.Rgba32i,            PixelType.Int,                      typeof(int)) },
			{ TextureFormat.RGBA32ui,           (PixelFormat.RgbaInteger,       PixelInternalFormat.Rgba32ui,           PixelType.UnsignedInt,              typeof(uint)) },

			#endregion

			#region DepthComponent

			{ TextureFormat.Depth16,            (PixelFormat.DepthComponent,    PixelInternalFormat.DepthComponent16,   PixelType.UnsignedShort,            typeof(ushort)) },	//16
			{ TextureFormat.Depth32,            (PixelFormat.DepthComponent,    PixelInternalFormat.DepthComponent32,   PixelType.UnsignedInt,              typeof(uint)) },
			{ TextureFormat.Depth32f,           (PixelFormat.DepthComponent,    PixelInternalFormat.DepthComponent32f,  PixelType.Float,                    typeof(float)) },   //32

			#endregion

			#region DepthStencil

			{ TextureFormat.Depth24Stencil8,    (PixelFormat.DepthStencil,      PixelInternalFormat.Depth24Stencil8,    PixelType.UnsignedInt248,           typeof(int)) },		//16+8
			{ TextureFormat.Depth32fStencil8,   (PixelFormat.DepthStencil,      PixelInternalFormat.Depth32fStencil8,   PixelType.Float32UnsignedInt248Rev, typeof(float)) },	//24+8
			
			#endregion
		};
	}
}
