using System;
using System.Runtime.InteropServices;
using Dissonance.Engine.IO;
using Newtonsoft.Json;

namespace Dissonance.Engine
{
	[JsonConverter(typeof(Vector3JsonConverter))]
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

		public float X;
		public float Y;
		public float Z;

		public float Magnitude => MathF.Sqrt(X * X + Y * Y + Z * Z);
		public float SqrMagnitude => X * X + Y * Y + Z * Z;
		public bool HasNaNs => float.IsNaN(X) || float.IsNaN(Y) || float.IsNaN(Z);

		public Vector2 XY {
			get => new(X, Y);
			set {
				X = value.X;
				Y = value.Y;
			}
		}
		public Vector2 XZ {
			get => new(X, Z);
			set {
				X = value.X;
				Z = value.Y;
			}
		}
		public Vector2 YZ {
			get => new(Y, Z);
			set {
				Y = value.X;
				Z = value.Y;
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
				0 => X,
				1 => Y,
				2 => Z,
				_ => throw new IndexOutOfRangeException($"Indices for {nameof(Vector3)} run from 0 to 2 (inclusive)."),
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
					default:
						throw new IndexOutOfRangeException($"Indices for {nameof(Vector3)} run from 0 to 2 (inclusive).");
				}
			}
		}

		public Vector3(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public Vector3(float xyz) : this(xyz, xyz, xyz) { }

		public Vector3(Vector2 xy, float z) : this(xy.X, xy.Y, z) { }

		public override int GetHashCode()
			=> X.GetHashCode() ^ Y.GetHashCode() << 2 ^ Z.GetHashCode() >> 2;

		public override bool Equals(object other)
			=> other is Vector3 vector && X == vector.X && Y == vector.Y && Z == vector.Z;

		public override string ToString()
			=> $"[{X}, {Y}, {Z}]";

		public float[] ToArray()
			=> new[] { X, Y, Z };

		public void Normalize()
		{
			float mag = Magnitude;

			if (mag != 0f) {
				float num = 1f / mag;

				X *= num;
				Y *= num;
				Z *= num;
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
			}
		}

		public void NormalizeEuler()
		{
			X = MathHelper.Repeat(X, 360f);
			Y = MathHelper.Repeat(Y, 360f);
			Z = MathHelper.Repeat(Z, 360f);
		}

		//TODO: Rewrite without matrices.
		public Vector3 Rotate(Vector3 rot) => Matrix4x4.CreateRotation(rot) * this;

		public Vector3 RotatedBy(Vector3 vec) => Matrix4x4.CreateRotation(-vec.X, vec.Y, -vec.Z) * this;

		public Vector3 RotatedBy(float x, float y, float z) => Matrix4x4.CreateRotation(-x, y, -z) * this;

		public static Vector3 Min(Vector3 a, Vector3 b)
		{
			return new Vector3(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
		}

		public static Vector3 Max(Vector3 a, Vector3 b)
		{
			return new Vector3(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));
		}

		public static Vector3 StepTowards(Vector3 val, Vector3 goal, float step)
		{
			return new Vector3(
				MathHelper.StepTowards(val.X, goal.X, step),
				MathHelper.StepTowards(val.Y, goal.Y, step),
				MathHelper.StepTowards(val.Z, goal.Z, step)
			);
		}

		public static Vector3 EulerToDirection(Vector3 euler)
		{
			float pitch = euler.X * MathHelper.Deg2Rad;
			float yaw = euler.Y * MathHelper.Deg2Rad;

			float cX = MathF.Cos(pitch);
			float sX = MathF.Sin(pitch);
			float cY = MathF.Cos(yaw);
			float sY = MathF.Sin(yaw);

			return new Vector3(
				cX * sY,
				-sX,
				cX * cY
			);
		}

		public static Vector3 DirectionToEuler(Vector3 direction)
		{
			float xzLength = MathF.Sqrt(direction.X * direction.X + direction.Z * direction.Z);
			float pitch = MathF.Atan2(xzLength, direction.Y) - MathHelper.HalfPI;
			float yaw = MathF.Atan2(direction.X, direction.Z);

			return new Vector3(
				pitch * MathHelper.Rad2Deg,
				yaw * MathHelper.Rad2Deg,
				0f
			);
		}

		public static Vector3 Repeat(Vector3 vec, float length) => new(
			vec.X - MathF.Floor(vec.X / length) * length,
			vec.Y - MathF.Floor(vec.Y / length) * length,
			vec.Z - MathF.Floor(vec.Z / length) * length
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
			vec.X = MathF.Floor(vec.X);
			vec.Y = MathF.Floor(vec.Y);
			vec.Z = MathF.Floor(vec.Z);

			return vec;
		}

		public static Vector3 Ceil(Vector3 vec)
		{
			vec.X = MathF.Ceiling(vec.X);
			vec.Y = MathF.Ceiling(vec.Y);
			vec.Z = MathF.Ceiling(vec.Z);

			return vec;
		}

		public static Vector3 Round(Vector3 vec)
		{
			vec.X = MathF.Round(vec.X);
			vec.Y = MathF.Round(vec.Y);
			vec.Z = MathF.Round(vec.Z);

			return vec;
		}

		public static Vector3 Lerp(Vector3 from, Vector3 to, float t)
		{
			t = MathHelper.Clamp01(t);

			return new Vector3(from.X + (to.X - from.X) * t, from.Y + (to.Y - from.Y) * t, from.Z + (to.Z - from.Z) * t);
		}

		public static Vector3 LerpAngle(Vector3 from, Vector3 to, float t)
		{
			// Could be sped up.

			return new Vector3(
				MathHelper.LerpAngle(from.X, to.X, t),
				MathHelper.LerpAngle(from.Y, to.Y, t),
				MathHelper.LerpAngle(from.Z, to.Z, t)
			);
		}

		public static void Cross(ref Vector3 left, ref Vector3 right, out Vector3 result)
		{
			result = new Vector3(
				left.Y * right.Z - left.Z * right.Y,
				left.Z * right.X - left.X * right.Z,
				left.X * right.Y - left.Y * right.X
			);
		}

		public static Vector3 Cross(Vector3 left, Vector3 right)
		{
			Cross(ref left, ref right, out var result);

			return result;
		}

		public static float Dot(Vector3 left, Vector3 right)
			=> left.X * right.X + left.Y * right.Y + left.Z * right.Z;

		public static float Angle(Vector3 from, Vector3 to)
		{
			float denominator = MathF.Sqrt(from.SqrMagnitude * to.SqrMagnitude);

			if (denominator < kEpsilonNormalSqrt) {
				return 0f;
			}

			float dot = MathHelper.Clamp(Dot(from, to) / denominator, -1F, 1F);

			return MathF.Acos(dot) * MathHelper.Rad2Deg;
		}

		public static float Distance(Vector3 a, Vector3 b)
			=> (a - b).Magnitude;

		public static float SqrDistance(Vector3 a, Vector3 b)
			=> (a - b).SqrMagnitude;

		// Vector3

		public static Vector3 operator -(Vector3 a) => new(-a.X, -a.Y, -a.Z);

		public static Vector3 operator -(Vector3 a, Vector3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

		public static Vector3 operator +(Vector3 a, Vector3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

		public static Vector3 operator *(Vector3 a, Vector3 b) => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z);

		public static Vector3 operator /(Vector3 a, Vector3 b) => new(a.X / b.X, a.Y / b.Y, a.Z / b.Z);

		public static bool operator ==(Vector3 a, Vector3 b) => (a - b).SqrMagnitude < 9.99999944E-11f;

		public static bool operator !=(Vector3 a, Vector3 b) => (a - b).SqrMagnitude >= 9.99999944E-11f;

		// float

		public static Vector3 operator *(Vector3 a, float d) => new(a.X * d, a.Y * d, a.Z * d);

		public static Vector3 operator *(float d, Vector3 a) => new(a.X * d, a.Y * d, a.Z * d);

		public static Vector3 operator /(Vector3 a, float d) => new(a.X / d, a.Y / d, a.Z / d);

		// float*

		public static unsafe implicit operator float*(Vector3 vec) => (float*)&vec;

		// System.Numerics.Vector3

		public static implicit operator Vector3(System.Numerics.Vector3 value) => new(value.X, value.Y, value.Z);

		public static implicit operator System.Numerics.Vector3(Vector3 value) => new(value.X, value.Y, value.Z);

		// BulletSharp.Math.Vector3

		public static implicit operator BulletSharp.Math.Vector3(Vector3 value) => new(value.X, value.Y, value.Z);

		public static implicit operator Vector3(BulletSharp.Math.Vector3 value) => new(value.X, value.Y, value.Z);
	}
}

