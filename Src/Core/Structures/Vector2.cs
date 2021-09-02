using System;
using System.Runtime.InteropServices;

namespace Dissonance.Engine
{
	public struct Vector2
	{
		public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Vector2));
		public static readonly Vector2 Zero = default;
		public static readonly Vector2 One = new Vector2(1f, 1f);
		public static readonly Vector2 UnitX = new Vector2(1f, 0f);
		public static readonly Vector2 UnitY = new Vector2(0f, 1f);
		public static readonly Vector2 Up = new Vector2(0f, 1f);
		public static readonly Vector2 Down = new Vector2(0f, -1f);
		public static readonly Vector2 Left = new Vector2(-1f, 0f);
		public static readonly Vector2 Right = new Vector2(1f, 0f);

		public float x;
		public float y;

		public Vector2 Normalized => Normalize(this);
		public float Magnitude => MathF.Sqrt(x * x + y * y);
		public float SqrMagnitude => x * x + y * y;
		public bool HasNaNs => float.IsNaN(x) || float.IsNaN(y);

		public float this[int index] {
			get => index switch {
				0 => x,
				1 => y,
				_ => throw new IndexOutOfRangeException("Indices for Vector2 run from 0 to 1, inclusively."),
			};
			set {
				switch (index) {
					case 0:
						x = value;
						return;
					case 1:
						y = value;
						return;
					default:
						throw new IndexOutOfRangeException("Indices for Vector2 run from 0 to 1, inclusively.");
				}
			}
		}

		public Vector2(float X, float Y)
		{
			x = X;
			y = Y;
		}

		public override int GetHashCode()
			=> x.GetHashCode() ^ y.GetHashCode() << 2;

		public override bool Equals(object other)
			=> other is Vector2 vector && x == vector.x && y == vector.y;

		public override string ToString()
			=> $"[{x}, {y}]";

		public float[] ToArray()
			=> new[] { x, y };

		public void Normalize()
		{
			float magnitude = Magnitude;

			if (magnitude != 0f) {
				float num = 1f / magnitude;
				x *= num;
				y *= num;
			}
		}

		public void Normalize(out float magnitude)
		{
			magnitude = Magnitude;

			if (magnitude != 0f) {
				float num = 1f / magnitude;
				x *= num;
				y *= num;
			}
		}

		public static float Dot(Vector2 a, Vector2 b) => a.x * b.x + a.y * b.y;

		public static float Distance(Vector2 a, Vector2 b) => (a - b).Magnitude;

		public static float SqrDistance(Vector2 a, Vector2 b) => (a - b).SqrMagnitude;

		public static Vector2 Normalize(Vector2 vec)
		{
			float magnitude = vec.Magnitude;

			if (magnitude != 0f) {
				float num = 1f / magnitude;
				vec.x *= num;
				vec.y *= num;
			}

			return vec;
		}

		public static Vector2 Normalize(Vector2 vec, out float magnitude)
		{
			magnitude = vec.Magnitude;

			if (magnitude != 0f) {
				float num = 1f / magnitude;
				vec.x *= num;
				vec.y *= num;
			}

			return vec;
		}

		public static Vector2 StepTowards(Vector2 vec, Vector2 goal, float step)
		{
			vec.x = MathHelper.StepTowards(vec.x, goal.x, step);
			vec.y = MathHelper.StepTowards(vec.y, goal.y, step);

			return vec;
		}

		public static Vector2 SnapToGrid(Vector2 vec, float step)
		{
			vec.x = MathHelper.SnapToGrid(vec.x, step);
			vec.y = MathHelper.SnapToGrid(vec.y, step);

			return vec;
		}

		public static Vector2 Floor(Vector2 vec)
		{
			vec.x = MathF.Floor(vec.x);
			vec.y = MathF.Floor(vec.y);

			return vec;
		}

		public static Vector2 Ceil(Vector2 vec)
		{
			vec.x = MathF.Ceiling(vec.x);
			vec.y = MathF.Ceiling(vec.y);

			return vec;
		}

		public static Vector2 Round(Vector2 vec)
		{
			vec.x = MathF.Round(vec.x);
			vec.y = MathF.Round(vec.y);

			return vec;
		}

		public static Vector2 Rotate(Vector2 vec, float angle)
		{
			float sin = MathF.Sin(angle * MathHelper.Deg2Rad);
			float cos = MathF.Cos(angle * MathHelper.Deg2Rad);

			vec.x = cos * vec.x - sin * vec.y;
			vec.y = sin * vec.x + cos * vec.y;

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
			result.x = from.x + (to.x - from.x) * t;
			result.y = from.y + (to.y - from.y) * t;
			return result;
		}

		// Operations

		// Int
		public static Vector2 operator *(Vector2 a, int d) => new Vector2(a.x * d, a.y * d);

		public static Vector2 operator *(int d, Vector2 a) => new Vector2(a.x * d, a.y * d);

		public static Vector2 operator /(Vector2 a, int d) => new Vector2(a.x / d, a.y / d);

		// Float

		public static Vector2 operator *(Vector2 a, float d) => new Vector2(a.x * d, a.y * d);

		public static Vector2 operator *(float d, Vector2 a) => new Vector2(a.x * d, a.y * d);

		public static Vector2 operator /(Vector2 a, float d) => new Vector2(a.x / d, a.y / d);

		// Vector2
		public static bool operator ==(Vector2 a, Vector2 b) => (a - b).SqrMagnitude < 9.99999944E-11f;

		public static bool operator !=(Vector2 a, Vector2 b) => (a - b).SqrMagnitude >= 9.99999944E-11f;

		public static Vector2 operator -(Vector2 a) => new(-a.x, -a.y);

		public static Vector2 operator +(Vector2 a, Vector2 b) => new(a.x + b.x, a.y + b.y);

		public static Vector2 operator -(Vector2 a, Vector2 b) => new(a.x - b.x, a.y - b.y);

		public static Vector2 operator *(Vector2 a, Vector2 b) => new(a.x * b.x, a.y * b.y);

		public static Vector2 operator /(Vector2 a, Vector2 b) => new(a.x / b.x, a.y / b.y);

		// Vector2Int

		public static Vector2 operator +(Vector2Int a, Vector2 b) => new(a.x + b.x, a.y + b.y);

		public static Vector2 operator +(Vector2 a, Vector2Int b) => new(a.x + b.x, a.y + b.y);

		public static Vector2 operator -(Vector2Int a, Vector2 b) => new(a.x - b.x, a.y - b.y);

		public static Vector2 operator -(Vector2 a, Vector2Int b) => new(a.x - b.x, a.y - b.y);

		public static Vector2 operator *(Vector2Int a, Vector2 b) => new(a.x * b.x, a.y * b.y);

		public static Vector2 operator *(Vector2 a, Vector2Int b) => new(a.x * b.x, a.y * b.y);

		public static Vector2 operator /(Vector2Int a, Vector2 b) => new(a.x / b.x, a.y / b.y);

		public static Vector2 operator /(Vector2 a, Vector2Int b) => new(a.x / b.x, a.y / b.y);

		// Vector2UShort

		public static Vector2 operator +(Vector2UShort a, Vector2 b) => new(a.x + b.x, a.y + b.y);

		public static Vector2 operator +(Vector2 a, Vector2UShort b) => new(a.x + b.x, a.y + b.y);

		public static Vector2 operator -(Vector2UShort a, Vector2 b) => new(a.x - b.x, a.y - b.y);

		public static Vector2 operator -(Vector2 a, Vector2UShort b) => new(a.x - b.x, a.y - b.y);

		public static Vector2 operator *(Vector2UShort a, Vector2 b) => new(a.x * b.x, a.y * b.y);

		public static Vector2 operator *(Vector2 a, Vector2UShort b) => new(a.x * b.x, a.y * b.y);

		public static Vector2 operator /(Vector2UShort a, Vector2 b) => new(a.x / b.x, a.y / b.y);

		public static Vector2 operator /(Vector2 a, Vector2UShort b) => new(a.x / b.x, a.y / b.y);

		// Casts

		// float*

		public static unsafe implicit operator float*(Vector2 vec) => (float*)&vec;

		// System.Numerics.Vector2

		public static implicit operator Vector2(System.Numerics.Vector2 value) => new Vector2(value.X, value.Y);

		public static implicit operator System.Numerics.Vector2(Vector2 value) => new System.Numerics.Vector2(value.x, value.y);

		// Vector2Int

		public static explicit operator Vector2(Vector2Int value) => new Vector2(value.x, value.y);

		// Vector2UShort

		public static explicit operator Vector2(Vector2UShort value) => new Vector2(value.x, value.y);
	}
}

