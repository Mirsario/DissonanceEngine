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

		public float x;
		public float y;
		public float z;
		public float w;

		public float Magnitude => MathF.Sqrt(x * x + y * y + z * z + w * w);
		public float SqrMagnitude => x * x + y * y + z * z + w * w;
		public bool HasNaNs => float.IsNaN(x) || float.IsNaN(y) || float.IsNaN(z);
		public Vector2 XY => new(x, y);
		public Vector3 XYZ => new(x, y, z);

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
				0 => x,
				1 => y,
				2 => z,
				3 => w,
				_ => throw new IndexOutOfRangeException("Indices for Vector4 run from 0 to 3,inclusive."),
			};
			set {
				switch (index) {
					case 0:
						x = value;
						return;
					case 1:
						y = value;
						return;
					case 2:
						z = value;
						return;
					case 3:
						w = value;
						return;
					default:
						throw new IndexOutOfRangeException("Indices for Vector4 run from 0 to 3,inclusive.");
				}
			}
		}

		public Vector4(float X, float Y, float Z, float W)
		{
			x = X;
			y = Y;
			z = Z;
			w = W;
		}

		public Vector4(float X, float Y, float Z) : this(X, Y, Z, 0) { }

		public Vector4(float X, float Y) : this(X, Y, 0, 0) { }

		public Vector4(Vector3 XYZ, float W) : this(XYZ.x, XYZ.y, XYZ.z, W) { }

		public Vector4(Vector2 XY, Vector2 ZW) : this(XY.x, XY.y, ZW.x, ZW.y) { }

		public override string ToString()
			=> $"[{x}, {y}, {z}, {w}]";

		public override int GetHashCode()
			=> x.GetHashCode() ^ y.GetHashCode() << 2 ^ z.GetHashCode() >> 2 ^ w.GetHashCode() >> 1;

		public override bool Equals(object other)
			=> other is Vector4 vector && x == vector.x && y == vector.y && z == vector.z && w == vector.w;

		public float[] ToArray()
		{
			return new[] {
				x,y,z,w
			};
		}

		public void Normalize()
		{
			float mag = Magnitude;
			if (mag != 0f) {
				float num = 1f / mag;
				x *= num;
				y *= num;
				z *= num;
				w *= num;
			}
		}

		public void Normalize(out float magnitude)
		{
			magnitude = Magnitude;
			if (magnitude != 0f) {
				float num = 1f / magnitude;
				x *= num;
				y *= num;
				z *= num;
				w *= num;
			}
		}

		public static Vector4 Lerp(Vector4 from, Vector4 to, float t)
		{
			t = MathHelper.Clamp01(t);

			return new Vector4(
				from.x + (to.x - from.x) * t,
				from.y + (to.y - from.y) * t,
				from.z + (to.z - from.z) * t,
				from.w + (to.w - from.w) * t
			);
		}

		public static Vector4 BiLerp(Vector4 valueTopLeft, Vector4 valueTopRight, Vector4 valueBottomLeft, Vector4 valueBottomRight, Vector2 topLeft, Vector2 bottomRight, Vector2 point)
		{
			float x2x1, y2y1, x2x, y2y, yy1, xx1;
			x2x1 = bottomRight.x - topLeft.x;
			y2y1 = bottomRight.y - topLeft.y;
			x2x = bottomRight.x - point.x;
			y2y = bottomRight.y - point.y;
			yy1 = point.y - topLeft.y;
			xx1 = point.x - topLeft.x;
			float mul = 1f / (x2x1 * y2y1);
			float mulTopLeft = x2x * yy1;
			float mulTopRight = xx1 * yy1;
			float mulBottomLeft = x2x * y2y;
			float mulBottomRight = xx1 * y2y;
			return new Vector4(
				mul * (valueTopLeft.x * mulTopLeft + valueTopRight.x * mulTopRight + valueBottomLeft.x * mulBottomLeft + valueBottomRight.x * mulBottomRight),
				mul * (valueTopLeft.y * mulTopLeft + valueTopRight.y * mulTopRight + valueBottomLeft.y * mulBottomLeft + valueBottomRight.y * mulBottomRight),
				mul * (valueTopLeft.z * mulTopLeft + valueTopRight.z * mulTopRight + valueBottomLeft.z * mulBottomLeft + valueBottomRight.z * mulBottomRight),
				mul * (valueTopLeft.w * mulTopLeft + valueTopRight.w * mulTopRight + valueBottomLeft.w * mulBottomLeft + valueBottomRight.w * mulBottomRight)
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
			vec.x = MathF.Floor(vec.x);
			vec.y = MathF.Floor(vec.y);
			vec.z = MathF.Floor(vec.z);
			vec.w = MathF.Floor(vec.w);
			return vec;
		}

		public static Vector4 Ceil(Vector4 vec)
		{
			vec.x = MathF.Ceiling(vec.x);
			vec.y = MathF.Ceiling(vec.y);
			vec.z = MathF.Ceiling(vec.z);
			vec.w = MathF.Ceiling(vec.w);
			return vec;
		}

		public static Vector4 Round(Vector4 vec)
		{
			vec.x = MathF.Round(vec.x);
			vec.y = MathF.Round(vec.y);
			vec.z = MathF.Round(vec.z);
			vec.w = MathF.Round(vec.w);
			return vec;
		}

		public static explicit operator Vector4(Vector3 value) => new(value.x, value.y, value.z, 0f);

		public static explicit operator Vector3(Vector4 value) => new(value.x, value.y, value.z);

		public static explicit operator Vector2(Vector4 value) => new(value.x, value.y);

		// Vector4

		public static Vector4 operator +(Vector4 a, Vector4 b) => new(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);

		public static Vector4 operator -(Vector4 a, Vector4 b) => new(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);

		public static Vector4 operator *(Vector4 a, Vector4 b) => new(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);

		public static Vector4 operator /(Vector4 a, Vector4 b) => new(a.x / b.x, a.y / b.y, a.z / b.z, a.w / b.w);

		public static Vector4 operator -(Vector4 a) => new(-a.x, -a.y, -a.z, -a.w);

		public static Vector4 operator *(Vector4 a, float d) => new(a.x * d, a.y * d, a.z * d, a.w * d);

		public static Vector4 operator *(float d, Vector4 a) => new(a.x * d, a.y * d, a.z * d, a.w * d);

		public static Vector4 operator /(Vector4 a, float d) => new(a.x / d, a.y / d, a.z / d, a.w / d);

		public static bool operator ==(Vector4 a, Vector4 b) => (a - b).SqrMagnitude < 9.99999944E-11f;

		public static bool operator !=(Vector4 a, Vector4 b) => (a - b).SqrMagnitude >= 9.99999944E-11f;

		// Casts

		// float*
		public static unsafe implicit operator float*(Vector4 vec) => (float*)&vec;

		// System.Numerics.Vector3
		public static implicit operator Vector4(System.Numerics.Vector4 value) => new(value.X, value.Y, value.Z, value.W);
		public static implicit operator System.Numerics.Vector4(Vector4 value) => new(value.x, value.y, value.z, value.w);
	}
}

