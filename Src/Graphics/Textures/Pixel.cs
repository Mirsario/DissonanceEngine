using System;
using System.Drawing;
using Dissonance.Engine.Structures;

namespace Dissonance.Engine.Graphics.Textures
{
	public struct Pixel
	{
		public byte r;
		public byte g;
		public byte b;
		public byte a;

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
				return index switch
				{
					0 => r,
					1 => g,
					2 => b,
					3 => a,
					_ => throw new IndexOutOfRangeException("Indices for Pixel run from 0 to 3,inclusive."),
				};
			}
			set {
				switch(index) {
					case 0:
						r = value;
						return;
					case 1:
						g = value;
						return;
					case 2:
						b = value;
						return;
					case 3:
						a = value;
						return;
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

		public override string ToString() => $"[{R}, {G}, {B}, {A}]";

		public static implicit operator Color(Pixel value) => Color.FromArgb(value.a,value.r,value.g,value.b);
		public static implicit operator Pixel(Color value) => new Pixel(value.R,value.G,value.B,value.A);
		public static implicit operator Vector4(Pixel value) => new Vector4(value.R,value.G,value.B,value.A);
		public static implicit operator Pixel(Vector4 value) => new Pixel(value.x,value.y,value.z,value.w);
	}
}