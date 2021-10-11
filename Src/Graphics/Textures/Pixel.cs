using System;
using System.Drawing;

namespace Dissonance.Engine.Graphics
{
	public struct Pixel
	{
		public byte R;
		public byte G;
		public byte B;
		public byte A;

		public byte this[int index] {
			get {
				return index switch {
					0 => R,
					1 => G,
					2 => B,
					3 => A,
					_ => throw new IndexOutOfRangeException("Indices for Pixel run from 0 to 3,inclusive."),
				};
			}
			set {
				switch (index) {
					case 0:
						R = value;
						return;
					case 1:
						G = value;
						return;
					case 2:
						B = value;
						return;
					case 3:
						A = value;
						return;
					default:
						throw new IndexOutOfRangeException("Indices for Pixel run from 0 to 3,inclusive.");
				}
			}
		}

		public Pixel(byte r, byte g, byte b, byte a)
		{
			R = r;
			G = g;
			B = b;
			A = a;
		}

		public Pixel(float r, float g, float b, float a)
		{
			R = (byte)(r / 255);
			G = (byte)(g / 255);
			B = (byte)(b / 255);
			A = (byte)(a / 255);
		}

		public override string ToString() => $"[{R}, {G}, {B}, {A}]";

		public static implicit operator Color(Pixel value) => Color.FromArgb(value.A, value.R, value.G, value.B);

		public static implicit operator Pixel(Color value) => new(value.R, value.G, value.B, value.A);

		public static explicit operator Vector4(Pixel value) => new(value.R / 255f, value.G / 255f, value.B / 255f, value.A / 255f);

		public static explicit operator Pixel(Vector4 value) => new(value.X, value.Y, value.Z, value.W);
	}
}
