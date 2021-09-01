using System;
using System.Runtime.InteropServices;

namespace Dissonance.Engine
{
	public struct Vector3
	{
		public const float kEpsilon = 0.00001F;
		public const float kEpsilonNormalSqrt = 1e-15F;

		public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Vector3));
		public static readonly Vector3 Zero = default;
		public static readonly Vector3 One = new(1f, 1f, 1f);
		public static readonly Vector3 UnitX = new(1f, 0f, 0f);
		public static readonly Vector3 UnitY = new(0f, 1f, 0f);
		public static readonly Vector3 UnitZ = new(0f, 0f, 1f);
		public static readonly Vector3 Up = new(0f, 1f, 0f);
		public static readonly Vector3 Down = new(0f, -1f, 0f);
		public static readonly Vector3 Left = new(-1f, 0f, 0f);
		public static readonly Vector3 Right = new(1f, 0f, 0f);
		public static readonly Vector3 Forward = new(0f, 0f, 1f);
		public static readonly Vector3 Backward = new(0f, 0f, -1f);

		public float x;
		public float y;
		public float z;

		public float Magnitude => Mathf.Sqrt(x * x + y * y + z * z);
		public float SqrMagnitude => x * x + y * y + z * z;
		public bool HasNaNs => float.IsNaN(x) || float.IsNaN(y) || float.IsNaN(z);

		public Vector2 XY {
			get => new(x, y);
			set {
				x = value.x;
				y = value.y;
			}
		}
		public Vector2 XZ {
			get => new(x, z);
			set {
				x = value.x;
				z = value.y;
			}
		}
		public Vector3 Normalized {
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
				_ => throw new IndexOutOfRangeException($"Indices for {nameof(Vector3)} run from 0 to 2 (inclusive)."),
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
					default:
						throw new IndexOutOfRangeException($"Indices for {nameof(Vector3)} run from 0 to 2 (inclusive).");
				}
			}
		}

		public Vector3(float X, float Y, float Z)
		{
			x = X;
			y = Y;
			z = Z;
		}

		public Vector3(float XYZ)
		{
			x = XYZ;
			y = XYZ;
			z = XYZ;
		}

		public override int GetHashCode()
			=> x.GetHashCode() ^ y.GetHashCode() << 2 ^ z.GetHashCode() >> 2;

		public override bool Equals(object other)
			=> other is Vector3 vector && x == vector.x && y == vector.y && z == vector.z;

		public override string ToString()
			=> $"[{x}, {y}, {z}]";

		public float[] ToArray()
			=> new[] { x, y, z };

		public void Normalize()
		{
			float mag = Magnitude;

			if (mag != 0f) {
				float num = 1f / mag;

				x *= num;
				y *= num;
				z *= num;
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
			}
		}

		public void NormalizeEuler()
		{
			//TODO: Rewrite without loops, sigh.

			while (x >= 360f) {
				x -= 360f;
			}

			while (x < 0f) {
				x += 360f;
			}

			while (y >= 360f) {
				y -= 360f;
			}

			while (y < 0f) {
				y += 360f;
			}

			while (z >= 360f) {
				z -= 360f;
			}

			while (z < 0f) {
				z += 360f;
			}
		}

		//TODO: Rewrite without matrices.
		public Vector3 Rotate(Vector3 rot) => Matrix4x4.CreateRotation(rot) * this;

		public Vector3 RotatedBy(Vector3 vec) => Matrix4x4.CreateRotation(-vec.x, vec.y, -vec.z) * this;

		public Vector3 RotatedBy(float x, float y, float z) => Matrix4x4.CreateRotation(-x, y, -z) * this;

		public static Vector3 Min(Vector3 a, Vector3 b)
		{
			return new Vector3(Math.Min(a.x, b.x), Math.Min(a.y, b.y), Math.Min(a.z, b.z));
		}

		public static Vector3 Max(Vector3 a, Vector3 b)
		{
			return new Vector3(Math.Max(a.x, b.x), Math.Max(a.y, b.y), Math.Max(a.z, b.z));
		}

		public static Vector3 StepTowards(Vector3 val, Vector3 goal, float step)
		{
			return new Vector3(
				Mathf.StepTowards(val.x, goal.x, step),
				Mathf.StepTowards(val.y, goal.y, step),
				Mathf.StepTowards(val.z, goal.z, step)
			);
		}

		public static Vector3 EulerToDirection(Vector3 euler)
		{
			float pitch = euler.x * Mathf.Deg2Rad;
			float yaw = euler.y * Mathf.Deg2Rad;

			float cX = Mathf.Cos(pitch);
			float sX = Mathf.Sin(pitch);
			float cY = Mathf.Cos(yaw);
			float sY = Mathf.Sin(yaw);

			return new Vector3(
				cX * sY,
				-sX,
				cX * cY
			);
		}

		public static Vector3 DirectionToEuler(Vector3 direction)
		{
			float xzLength = Mathf.Sqrt(direction.x * direction.x + direction.z * direction.z);
			float pitch = Mathf.Atan2(xzLength, direction.y) - Mathf.HalfPI;
			float yaw = Mathf.Atan2(direction.x, direction.z);

			return new Vector3(
				pitch * Mathf.Rad2Deg,
				yaw * Mathf.Rad2Deg,
				0f
			);
		}

		public static Vector3 Repeat(Vector3 vec, float length) => new(
			vec.x - Mathf.Floor(vec.x / length) * length,
			vec.y - Mathf.Floor(vec.y / length) * length,
			vec.z - Mathf.Floor(vec.z / length) * length
		);

		public static Vector3 Rotate(Vector3 vec, Vector3 rot)
			=> Matrix4x4.CreateRotation(rot) * vec;

		public static Vector3 Normalize(Vector3 vec)
		{
			float mag = vec.Magnitude;

			if (mag != 0f) {
				vec *= 1f / mag;
			}

			return vec;
		}

		public static Vector3 Floor(Vector3 vec)
		{
			vec.x = Mathf.Floor(vec.x);
			vec.y = Mathf.Floor(vec.y);
			vec.z = Mathf.Floor(vec.z);

			return vec;
		}

		public static Vector3 Ceil(Vector3 vec)
		{
			vec.x = Mathf.Ceil(vec.x);
			vec.y = Mathf.Ceil(vec.y);
			vec.z = Mathf.Ceil(vec.z);

			return vec;
		}

		public static Vector3 Round(Vector3 vec)
		{
			vec.x = Mathf.Round(vec.x);
			vec.y = Mathf.Round(vec.y);
			vec.z = Mathf.Round(vec.z);

			return vec;
		}

		public static Vector3 Lerp(Vector3 from, Vector3 to, float t)
		{
			t = Mathf.Clamp01(t);

			return new Vector3(from.x + (to.x - from.x) * t, from.y + (to.y - from.y) * t, from.z + (to.z - from.z) * t);
		}

		public static Vector3 LerpAngle(Vector3 from, Vector3 to, float t)
		{
			// Could be sped up.

			return new Vector3(
				Mathf.LerpAngle(from.x, to.x, t),
				Mathf.LerpAngle(from.y, to.y, t),
				Mathf.LerpAngle(from.z, to.z, t)
			);
		}

		public static void Cross(ref Vector3 left, ref Vector3 right, out Vector3 result)
		{
			result = new Vector3(
				left.y * right.z - left.z * right.y,
				left.z * right.x - left.x * right.z,
				left.x * right.y - left.y * right.x
			);
		}

		public static Vector3 Cross(Vector3 left, Vector3 right)
		{
			Cross(ref left, ref right, out var result);

			return result;
		}

		public static float Dot(Vector3 left, Vector3 right)
			=> left.x * right.x + left.y * right.y + left.z * right.z;

		public static float Angle(Vector3 from, Vector3 to)
		{
			float denominator = Mathf.Sqrt(from.SqrMagnitude * to.SqrMagnitude);

			if (denominator < kEpsilonNormalSqrt) {
				return 0f;
			}

			float dot = Mathf.Clamp(Dot(from, to) / denominator, -1F, 1F);

			return Mathf.Acos(dot) * Mathf.Rad2Deg;
		}

		public static float Distance(Vector3 a, Vector3 b)
			=> (a - b).Magnitude;

		public static float SqrDistance(Vector3 a, Vector3 b)
			=> (a - b).SqrMagnitude;

		// Operations

		// Vector3
		public static Vector3 operator *(Vector3 a, Vector3 b) => new(a.x * b.x, a.y * b.y, a.z * b.z);

		public static Vector3 operator /(Vector3 a, Vector3 b) => new(a.x / b.x, a.y / b.y, a.z / b.z);

		public static Vector3 operator +(Vector3 a, Vector3 b) => new(a.x + b.x, a.y + b.y, a.z + b.z);

		public static Vector3 operator -(Vector3 a, Vector3 b) => new(a.x - b.x, a.y - b.y, a.z - b.z);

		public static Vector3 operator -(Vector3 a) => new(-a.x, -a.y, -a.z);

		public static bool operator ==(Vector3 a, Vector3 b) => (a - b).SqrMagnitude < 9.99999944E-11f;

		public static bool operator !=(Vector3 a, Vector3 b) => (a - b).SqrMagnitude >= 9.99999944E-11f;

		// Float

		public static Vector3 operator *(Vector3 a, float d) => new(a.x * d, a.y * d, a.z * d);

		public static Vector3 operator *(float d, Vector3 a) => new(a.x * d, a.y * d, a.z * d);

		public static Vector3 operator /(Vector3 a, float d) => new(a.x / d, a.y / d, a.z / d);

		// Casts

		// float*

		public static unsafe implicit operator float*(Vector3 vec) => (float*)&vec;

		// System.Numerics.Vector3

		public static implicit operator Vector3(System.Numerics.Vector3 value) => new(value.X, value.Y, value.Z);

		public static implicit operator System.Numerics.Vector3(Vector3 value) => new(value.x, value.y, value.z);

		// BulletSharp.Math.Vector3

		public static implicit operator BulletSharp.Math.Vector3(Vector3 value) => new(value.x, value.y, value.z);

		public static implicit operator Vector3(BulletSharp.Math.Vector3 value) => new(value.X, value.Y, value.Z);
	}
}

