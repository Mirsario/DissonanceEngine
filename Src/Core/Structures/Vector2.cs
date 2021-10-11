using System;
using System.Runtime.InteropServices;

namespace Dissonance.Engine
{
	public struct Vector2
	{
		public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Vector2));
		public static readonly Vector2 Zero = default;
		public static readonly Vector2 One = new(1f, 1f);
		public static readonly Vector2 UnitX = new(1f, 0f);
		public static readonly Vector2 UnitY = new(0f, 1f);
		public static readonly Vector2 Up = new(0f, 1f);
		public static readonly Vector2 Down = new(0f, -1f);
		public static readonly Vector2 Left = new(-1f, 0f);
		public static readonly Vector2 Right = new(1f, 0f);

		public float X;
		public float Y;

		public Vector2 Normalized => Normalize(this);
		public float Magnitude => MathF.Sqrt(X * X + Y * Y);
		public float SqrMagnitude => X * X + Y * Y;
		public bool HasNaNs => float.IsNaN(X) || float.IsNaN(Y);

		public float this[int index] {
			get => index switch {
				0 => X,
				1 => Y,
				_ => throw new IndexOutOfRangeException("Indices for Vector2 run from 0 to 1, inclusively."),
			};
			set {
				switch (index) {
					case 0:
						X = value;
						return;
					case 1:
						Y = value;
						return;
					default:
						throw new IndexOutOfRangeException("Indices for Vector2 run from 0 to 1, inclusively.");
				}
			}
		}

		public Vector2(float x, float y)
		{
			X = x;
			Y = y;
		}

		public Vector2(float xy) : this(xy, xy) { }

		public override int GetHashCode()
			=> X.GetHashCode() ^ Y.GetHashCode() << 2;

		public override bool Equals(object other)
			=> other is Vector2 vector && X == vector.X && Y == vector.Y;

		public override string ToString()
			=> $"[{X}, {Y}]";

		public float[] ToArray()
			=> new[] { X, Y };

		public void Normalize()
		{
			float magnitude = Magnitude;

			if (magnitude != 0f) {
				float num = 1f / magnitude;
				X *= num;
				Y *= num;
			}
		}

		public void Normalize(out float magnitude)
		{
			magnitude = Magnitude;

			if (magnitude != 0f) {
				float num = 1f / magnitude;
				X *= num;
				Y *= num;
			}
		}

		public static float Dot(Vector2 a, Vector2 b) => a.X * b.X + a.Y * b.Y;

		public static float Distance(Vector2 a, Vector2 b) => (a - b).Magnitude;

		public static float SqrDistance(Vector2 a, Vector2 b) => (a - b).SqrMagnitude;

		public static Vector2 Normalize(Vector2 vec)
		{
			float magnitude = vec.Magnitude;

			if (magnitude != 0f) {
				float num = 1f / magnitude;
				vec.X *= num;
				vec.Y *= num;
			}

			return vec;
		}

		public static Vector2 Normalize(Vector2 vec, out float magnitude)
		{
			magnitude = vec.Magnitude;

			if (magnitude != 0f) {
				float num = 1f / magnitude;
				vec.X *= num;
				vec.Y *= num;
			}

			return vec;
		}

		public static Vector2 StepTowards(Vector2 vec, Vector2 goal, float step)
		{
			vec.X = MathHelper.StepTowards(vec.X, goal.X, step);
			vec.Y = MathHelper.StepTowards(vec.Y, goal.Y, step);

			return vec;
		}

		public static Vector2 SnapToGrid(Vector2 vec, float step)
		{
			vec.X = MathHelper.SnapToGrid(vec.X, step);
			vec.Y = MathHelper.SnapToGrid(vec.Y, step);

			return vec;
		}

		public static Vector2 Floor(Vector2 vec)
		{
			vec.X = MathF.Floor(vec.X);
			vec.Y = MathF.Floor(vec.Y);

			return vec;
		}

		public static Vector2 Ceil(Vector2 vec)
		{
			vec.X = MathF.Ceiling(vec.X);
			vec.Y = MathF.Ceiling(vec.Y);

			return vec;
		}

		public static Vector2 Round(Vector2 vec)
		{
			vec.X = MathF.Round(vec.X);
			vec.Y = MathF.Round(vec.Y);

			return vec;
		}

		public static Vector2 Rotate(Vector2 vec, float angle)
		{
			float sin = MathF.Sin(angle * MathHelper.Deg2Rad);
			float cos = MathF.Cos(angle * MathHelper.Deg2Rad);

			vec.X = cos * vec.X - sin * vec.Y;
			vec.Y = sin * vec.X + cos * vec.Y;

			return vec;
		}

		public static Vector2 Lerp(Vector2 from, Vector2 to, float t)
		{
			if (t < 0f) {
				t = 0f;
			} else if (t > 1f) {
				t = 1f;
			}

			Vector2 result;
			result.X = from.X + (to.X - from.X) * t;
			result.Y = from.Y + (to.Y - from.Y) * t;
			return result;
		}

		// Operations

		// Int
		public static Vector2 operator *(Vector2 a, int d) => new(a.X * d, a.Y * d);

		public static Vector2 operator *(int d, Vector2 a) => new(a.X * d, a.Y * d);

		public static Vector2 operator /(Vector2 a, int d) => new(a.X / d, a.Y / d);

		// Float

		public static Vector2 operator *(Vector2 a, float d) => new(a.X * d, a.Y * d);

		public static Vector2 operator *(float d, Vector2 a) => new(a.X * d, a.Y * d);

		public static Vector2 operator /(Vector2 a, float d) => new(a.X / d, a.Y / d);

		// Vector2
		public static bool operator ==(Vector2 a, Vector2 b) => (a - b).SqrMagnitude < 9.99999944E-11f;

		public static bool operator !=(Vector2 a, Vector2 b) => (a - b).SqrMagnitude >= 9.99999944E-11f;

		public static Vector2 operator -(Vector2 a) => new(-a.X, -a.Y);

		public static Vector2 operator +(Vector2 a, Vector2 b) => new(a.X + b.X, a.Y + b.Y);

		public static Vector2 operator -(Vector2 a, Vector2 b) => new(a.X - b.X, a.Y - b.Y);

		public static Vector2 operator *(Vector2 a, Vector2 b) => new(a.X * b.X, a.Y * b.Y);

		public static Vector2 operator /(Vector2 a, Vector2 b) => new(a.X / b.X, a.Y / b.Y);

		// Vector2Int

		public static Vector2 operator +(Vector2Int a, Vector2 b) => new(a.X + b.X, a.Y + b.Y);

		public static Vector2 operator +(Vector2 a, Vector2Int b) => new(a.X + b.X, a.Y + b.Y);

		public static Vector2 operator -(Vector2Int a, Vector2 b) => new(a.X - b.X, a.Y - b.Y);

		public static Vector2 operator -(Vector2 a, Vector2Int b) => new(a.X - b.X, a.Y - b.Y);

		public static Vector2 operator *(Vector2Int a, Vector2 b) => new(a.X * b.X, a.Y * b.Y);

		public static Vector2 operator *(Vector2 a, Vector2Int b) => new(a.X * b.X, a.Y * b.Y);

		public static Vector2 operator /(Vector2Int a, Vector2 b) => new(a.X / b.X, a.Y / b.Y);

		public static Vector2 operator /(Vector2 a, Vector2Int b) => new(a.X / b.X, a.Y / b.Y);

		// Vector2UShort

		public static Vector2 operator +(Vector2UShort a, Vector2 b) => new(a.X + b.X, a.Y + b.Y);

		public static Vector2 operator +(Vector2 a, Vector2UShort b) => new(a.X + b.X, a.Y + b.Y);

		public static Vector2 operator -(Vector2UShort a, Vector2 b) => new(a.X - b.X, a.Y - b.Y);

		public static Vector2 operator -(Vector2 a, Vector2UShort b) => new(a.X - b.X, a.Y - b.Y);

		public static Vector2 operator *(Vector2UShort a, Vector2 b) => new(a.X * b.X, a.Y * b.Y);

		public static Vector2 operator *(Vector2 a, Vector2UShort b) => new(a.X * b.X, a.Y * b.Y);

		public static Vector2 operator /(Vector2UShort a, Vector2 b) => new(a.X / b.X, a.Y / b.Y);

		public static Vector2 operator /(Vector2 a, Vector2UShort b) => new(a.X / b.X, a.Y / b.Y);

		// Casts

		// float*

		public static unsafe implicit operator float*(Vector2 vec) => (float*)&vec;

		// System.Numerics.Vector2

		public static implicit operator Vector2(System.Numerics.Vector2 value) => new(value.X, value.Y);

		public static implicit operator System.Numerics.Vector2(Vector2 value) => new(value.X, value.Y);

		// Vector2Int

		public static explicit operator Vector2(Vector2Int value) => new(value.X, value.Y);

		// Vector2UShort

		public static explicit operator Vector2(Vector2UShort value) => new(value.X, value.Y);
	}
}

