using System;
using System.Runtime.InteropServices;

namespace Dissonance.Engine
{
	public struct Vector4
	{
		public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Vector4));
		public static readonly Vector4 Zero = default;
		public static readonly Vector4 One = new(1f, 1f, 1f, 1f);
		public static readonly Vector4 UnitX = new(1f, 0f, 0f, 0f);
		public static readonly Vector4 UnitY = new(0f, 1f, 0f, 0f);
		public static readonly Vector4 UnitZ = new(0f, 0f, 1f, 0f);
		public static readonly Vector4 UnitW = new(0f, 0f, 0f, 1f);

		public float X;
		public float Y;
		public float Z;
		public float W;

		public float Magnitude => MathF.Sqrt(X * X + Y * Y + Z * Z + W * W);
		public float SqrMagnitude => X * X + Y * Y + Z * Z + W * W;
		public bool HasNaNs => float.IsNaN(X) || float.IsNaN(Y) || float.IsNaN(Z);
		public Vector2 XY => new(X, Y);
		public Vector3 XYZ => new(X, Y, Z);

		public Vector4 Normalized {
			get {
				float mag = Magnitude;

				if (mag != 0f) {
					return this * (1f / mag);
				}

				return this;
			}
		}

		public float this[int index] {
			get => index switch
			{
				0 => X,
				1 => Y,
				2 => Z,
				3 => W,
				_ => throw new IndexOutOfRangeException("Indices for Vector4 run from 0 to 3,inclusive."),
			};
			set {
				switch (index) {
					case 0:
						X = value;
						return;
					case 1:
						Y = value;
						return;
					case 2:
						Z = value;
						return;
					case 3:
						W = value;
						return;
					default:
						throw new IndexOutOfRangeException("Indices for Vector4 run from 0 to 3,inclusive.");
				}
			}
		}

		public Vector4(float x, float y, float z, float w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public Vector4(float xyzw) : this(xyzw, xyzw, xyzw, xyzw) { }

		public Vector4(Vector3 xyz, float w) : this(xyz.X, xyz.Y, xyz.Z, w) { }

		public Vector4(Vector2 xy, float z, float w) : this(xy.X, xy.Y, z, w) { }

		public Vector4(Vector2 xy, Vector2 zw) : this(xy.X, xy.Y, zw.X, zw.Y) { }

		public override string ToString()
			=> $"[{X}, {Y}, {Z}, {W}]";

		public override int GetHashCode()
			=> X.GetHashCode() ^ Y.GetHashCode() << 2 ^ Z.GetHashCode() >> 2 ^ W.GetHashCode() >> 1;

		public override bool Equals(object other)
			=> other is Vector4 vector && X == vector.X && Y == vector.Y && Z == vector.Z && W == vector.W;

		public float[] ToArray()
		{
			return new[] {
				X,Y,Z,W
			};
		}

		public void Normalize()
		{
			float mag = Magnitude;
			if (mag != 0f) {
				float num = 1f / mag;
				X *= num;
				Y *= num;
				Z *= num;
				W *= num;
			}
		}

		public void Normalize(out float magnitude)
		{
			magnitude = Magnitude;
			if (magnitude != 0f) {
				float num = 1f / magnitude;
				X *= num;
				Y *= num;
				Z *= num;
				W *= num;
			}
		}

		public static Vector4 Lerp(Vector4 from, Vector4 to, float t)
		{
			t = MathHelper.Clamp01(t);

			return new Vector4(
				from.X + (to.X - from.X) * t,
				from.Y + (to.Y - from.Y) * t,
				from.Z + (to.Z - from.Z) * t,
				from.W + (to.W - from.W) * t
			);
		}

		public static Vector4 BiLerp(Vector4 valueTopLeft, Vector4 valueTopRight, Vector4 valueBottomLeft, Vector4 valueBottomRight, Vector2 topLeft, Vector2 bottomRight, Vector2 point)
		{
			float x2x1, y2y1, x2x, y2y, yy1, xx1;

			x2x1 = bottomRight.X - topLeft.X;
			y2y1 = bottomRight.Y - topLeft.Y;
			x2x = bottomRight.X - point.X;
			y2y = bottomRight.Y - point.Y;
			yy1 = point.Y - topLeft.Y;
			xx1 = point.X - topLeft.X;

			float mul = 1f / (x2x1 * y2y1);
			float mulTopLeft = x2x * yy1;
			float mulTopRight = xx1 * yy1;
			float mulBottomLeft = x2x * y2y;
			float mulBottomRight = xx1 * y2y;

			return new Vector4(
				mul * (valueTopLeft.X * mulTopLeft + valueTopRight.X * mulTopRight + valueBottomLeft.X * mulBottomLeft + valueBottomRight.X * mulBottomRight),
				mul * (valueTopLeft.Y * mulTopLeft + valueTopRight.Y * mulTopRight + valueBottomLeft.Y * mulBottomLeft + valueBottomRight.Y * mulBottomRight),
				mul * (valueTopLeft.Z * mulTopLeft + valueTopRight.Z * mulTopRight + valueBottomLeft.Z * mulBottomLeft + valueBottomRight.Z * mulBottomRight),
				mul * (valueTopLeft.W * mulTopLeft + valueTopRight.W * mulTopRight + valueBottomLeft.W * mulBottomLeft + valueBottomRight.W * mulBottomRight)
			);
		}

		public static float Distance(Vector4 a, Vector4 b)
		{
			return (a - b).Magnitude;
		}

		public static float SqrDistance(Vector4 a, Vector4 b)
		{
			return (a - b).SqrMagnitude;
		}

		public static Vector4 Floor(Vector4 vec)
		{
			vec.X = MathF.Floor(vec.X);
			vec.Y = MathF.Floor(vec.Y);
			vec.Z = MathF.Floor(vec.Z);
			vec.W = MathF.Floor(vec.W);

			return vec;
		}

		public static Vector4 Ceil(Vector4 vec)
		{
			vec.X = MathF.Ceiling(vec.X);
			vec.Y = MathF.Ceiling(vec.Y);
			vec.Z = MathF.Ceiling(vec.Z);
			vec.W = MathF.Ceiling(vec.W);

			return vec;
		}

		public static Vector4 Round(Vector4 vec)
		{
			vec.X = MathF.Round(vec.X);
			vec.Y = MathF.Round(vec.Y);
			vec.Z = MathF.Round(vec.Z);
			vec.W = MathF.Round(vec.W);

			return vec;
		}

		public static explicit operator Vector4(Vector3 value) => new(value.X, value.Y, value.Z, 0f);

		public static explicit operator Vector3(Vector4 value) => new(value.X, value.Y, value.Z);

		public static explicit operator Vector2(Vector4 value) => new(value.X, value.Y);

		// Vector4

		public static Vector4 operator +(Vector4 a, Vector4 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);

		public static Vector4 operator -(Vector4 a, Vector4 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);

		public static Vector4 operator *(Vector4 a, Vector4 b) => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W);

		public static Vector4 operator /(Vector4 a, Vector4 b) => new(a.X / b.X, a.Y / b.Y, a.Z / b.Z, a.W / b.W);

		public static Vector4 operator -(Vector4 a) => new(-a.X, -a.Y, -a.Z, -a.W);

		public static Vector4 operator *(Vector4 a, float d) => new(a.X * d, a.Y * d, a.Z * d, a.W * d);

		public static Vector4 operator *(float d, Vector4 a) => new(a.X * d, a.Y * d, a.Z * d, a.W * d);

		public static Vector4 operator /(Vector4 a, float d) => new(a.X / d, a.Y / d, a.Z / d, a.W / d);

		public static bool operator ==(Vector4 a, Vector4 b) => (a - b).SqrMagnitude < 9.99999944E-11f;

		public static bool operator !=(Vector4 a, Vector4 b) => (a - b).SqrMagnitude >= 9.99999944E-11f;

		// Casts

		// float*
		public static unsafe implicit operator float*(Vector4 vec) => (float*)&vec;

		// System.Numerics.Vector3
		public static implicit operator Vector4(System.Numerics.Vector4 value) => new(value.X, value.Y, value.Z, value.W);
		public static implicit operator System.Numerics.Vector4(Vector4 value) => new(value.X, value.Y, value.Z, value.W);
	}
}

