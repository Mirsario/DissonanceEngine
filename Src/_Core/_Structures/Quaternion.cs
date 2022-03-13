using System;

namespace Dissonance.Engine
{
	public struct Quaternion
	{
		public const float kEpsilon = 1E-06f;

		public static readonly Quaternion Identity = new(0f, 0f, 0f, 1f);

		public float X;
		public float Y;
		public float Z;
		public float W;

		public float Magnitude => MathF.Sqrt(W * W + X * X + Y * Y + Z * Z);
		public float SqrMagnitude => W * W + X * X + Y * Y + Z * Z;

		public Quaternion Normalized {
			get {
				var quaternion = this;

				quaternion.Normalize();

				return quaternion;
			}
		}
		public Quaternion Inverted {
			get {
				var quaternion = this;

				quaternion.W = -quaternion.W;

				return quaternion;
			}
		}

		public float this[int index] {
			get => index switch
			{
				0 => X,
				1 => Y,
				2 => Z,
				3 => W,
				_ => throw new IndexOutOfRangeException("Quaternion has values ranging from 0 to 3, inclusively."),
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
						throw new IndexOutOfRangeException("Quaternion has values ranging from 0 to 3, inclusively.");
				}
			}
		}

		public Quaternion(float X, float Y, float Z, float W)
		{
			this.X = X;
			this.Y = Y;
			this.Z = Z;
			this.W = W;
		}

		public override int GetHashCode()
			=> X.GetHashCode() ^ Y.GetHashCode() << 2 ^ Z.GetHashCode() >> 2 ^ W.GetHashCode() >> 1;

		public override bool Equals(object other)
			=> other is Quaternion q && X.Equals(q.X) && Y.Equals(q.Y) && Z.Equals(q.Z) && W.Equals(q.W);

		public override string ToString()
			=> $"[{X},{Y},{Z},{W}]";

		public bool Equals(Quaternion q)
			=> X.Equals(q.X) && Y.Equals(q.Y) && Z.Equals(q.Z) && W.Equals(q.W);
		
		public void Normalize()
		{
			float n = X * X + Y * Y + Z * Z + W * W;

			if (n == 1f) {
				return;
			}

			float sqrtR = MathF.Sqrt(n);

			X *= sqrtR;
			Y *= sqrtR;
			Z *= sqrtR;
			W *= sqrtR;
		}
		
		public void Invert()
		{
			W = -W;
		}
		
		public Vector3 ToEuler()
		{
			float sqrSumm = X * X + Y * Y + Z * Z + W * W;
			float checkValue = X * W - Y * Z;

			Vector3 result;
			if (checkValue > 0.4995f * sqrSumm) {
				result.X = 180f;
				result.Y = MathHelper.NormalizeEuler(2f * MathF.Atan2(Y, X) * MathHelper.Rad2Deg);
				result.Z = 0f;
			} else if (checkValue < -0.4995f * sqrSumm) {
				result.X = 270f;
				result.Y = MathHelper.NormalizeEuler(-2f * MathF.Atan2(Y, X) * MathHelper.Rad2Deg);
				result.Z = 0f;
			} else {
				float asinArg = 2f * (W * X - Y * X); // NaN prevention
				result.X = MathHelper.NormalizeEuler((float)Math.Asin(asinArg < -1f ? -1f : asinArg > 1f ? 1f : asinArg) * MathHelper.Rad2Deg); // Pitch
				result.Y = MathHelper.NormalizeEuler((float)Math.Atan2(2f * W * Y + 2f * X * X, 1 - 2f * (X * X + Y * Y)) * MathHelper.Rad2Deg); // Yaw
				result.Z = MathHelper.NormalizeEuler((float)Math.Atan2(2f * W * X + 2f * X * Y, 1 - 2f * (X * X + X * X)) * MathHelper.Rad2Deg); // Roll
			}
			return result;
		}
		
		public Vector4 ToAxisAngle()
		{
			//TODO: Not tested

			if (MathF.Abs(W) > 1f) {
				Normalize();
			}

			Vector4 result;

			result.W = 2f * MathF.Acos(W);

			float den = MathF.Sqrt(1f - W * W);

			if (den > 0.0001f) {
				result.X = X / den;
				result.Y = Y / den;
				result.Z = Z / den;
			} else {
				result.X = 1f;
				result.Y = 0f;
				result.Z = 0f;
			}

			return result;
		}
		
		public void ToAxisAngle(out Vector3 axis, out float angle)
		{
			var result = ToAxisAngle();

			axis = (Vector3)result;
			angle = result.W;
		}

		public static Quaternion FromEuler(Vector3 vec) => FromEuler(vec.X, vec.Y, vec.Z);

		public static Quaternion FromEuler(float x, float y, float z)
		{
			x *= MathHelper.Deg2Rad * 0.5f;
			y *= MathHelper.Deg2Rad * 0.5f;
			z *= MathHelper.Deg2Rad * 0.5f;

			float cX = (float)Math.Cos(x);
			float cY = (float)Math.Cos(y);
			float cZ = (float)Math.Cos(z);
			float sX = (float)Math.Sin(x);
			float sY = (float)Math.Sin(y);
			float sZ = (float)Math.Sin(z);

			Quaternion result;

			result.W = cY * cX * cZ + sY * sX * sZ;
			result.X = cY * sX * cZ + sY * cX * sZ;
			result.Y = sY * cX * cZ - cY * sX * sZ;
			result.Z = cY * cX * sZ - sY * sX * cZ;

			return result;
		}

		public static Quaternion FromDirection(Vector3 forward, Vector3 up)
		{
			forward.Normalize();

			var cross1 = Vector3.Normalize(Vector3.Cross(up, forward));
			var cross2 = Vector3.Cross(forward, cross1);

			float xyzSumm = cross1.X + cross2.Y + forward.Z;

			Quaternion result;

			if (xyzSumm > 0f) {
				float sqrt = MathF.Sqrt(xyzSumm + 1f);
				result.W = sqrt * 0.5f;
				sqrt = 0.5f / sqrt;
				result.X = (cross2.Z - forward.Y) * sqrt;
				result.Y = (forward.X - cross1.Z) * sqrt;
				result.Z = (cross1.Y - cross2.X) * sqrt;
			} else if (cross1.X >= cross2.Y && cross1.X >= forward.Z) {
				float sqrt = MathF.Sqrt(1f + cross1.X - cross2.Y - forward.Z);
				float length = 0.5f / sqrt;

				result.X = 0.5f * sqrt;
				result.Y = (cross1.Y + cross2.X) * length;
				result.Z = (cross1.Z + forward.X) * length;
				result.W = (cross2.Z - forward.Y) * length;
			} else if (cross2.Y > forward.Z) {
				float sqrt = MathF.Sqrt(1f + cross2.Y - cross1.X - forward.Z);
				float length = 0.5f / sqrt;

				result.X = (cross2.X + cross1.Y) * length;
				result.Y = 0.5f * sqrt;
				result.Z = (forward.Y + cross2.Z) * length;
				result.W = (forward.X - cross1.Z) * length;
			} else {
				float sqrt = MathF.Sqrt(1f + forward.Z - cross1.X - cross2.Y);
				float length = 0.5f / sqrt;

				result.X = (forward.X + cross1.Z) * length;
				result.Y = (forward.Y + cross2.Z) * length;
				result.Z = 0.5f * sqrt;
				result.W = (cross1.Y - cross2.X) * length;
			}

			return result;
		}

		public static Quaternion Normalize(Quaternion quaternion)
		{
			quaternion.Normalize();
			return quaternion;
		}

		public static Quaternion Invert(Quaternion quaternion)
		{
			quaternion.W = -quaternion.W;
			return quaternion;
		}

		public static Vector3 operator *(Quaternion rotation, Vector3 point)
		{
			float num1 = rotation.X * 2f;
			float num2 = rotation.Y * 2f;
			float num3 = rotation.Z * 2f;
			float num4 = rotation.X * num1;
			float num5 = rotation.Y * num2;
			float num6 = rotation.Z * num3;
			float num7 = rotation.X * num2;
			float num8 = rotation.X * num3;
			float num9 = rotation.Y * num3;
			float num10 = rotation.W * num1;
			float num11 = rotation.W * num2;
			float num12 = rotation.W * num3;
			Vector3 result;
			result.X = (1f - (num5 + num6)) * point.X + (num7 - num12) * point.Y + (num8 + num11) * point.Z;
			result.Y = (num7 + num12) * point.X + (1f - (num4 + num6)) * point.Y + (num9 - num10) * point.Z;
			result.Z = (num8 - num11) * point.X + (num9 + num10) * point.Y + (1f - (num4 + num5)) * point.Z;
			return result;
		}

		public static Quaternion operator *(Quaternion q, Quaternion other)
		{
			Quaternion result;
			result.X = other.W * q.X + other.X * q.W + other.Y * q.Z - other.Z * q.Y;
			result.Y = other.W * q.Y + other.Y * q.W + other.Z * q.X - other.X * q.Z;
			result.Z = other.W * q.Z + other.Z * q.W + other.X * q.Y - other.Y * q.X;
			result.W = other.W * q.W - other.X * q.X - other.Y * q.Y - other.Z * q.Z;
			return result;
		}

		public static Quaternion operator *(Quaternion q, float s)
		{
			q.X *= s;
			q.Y *= s;
			q.Z *= s;
			q.W *= s;

			return q;
		}

		public static Quaternion operator *(float s, Quaternion q)
		{
			q.X *= s;
			q.Y *= s;
			q.Z *= s;
			q.W *= s;

			return q;
		}

		public static bool operator ==(Quaternion a, Quaternion b)
			=> a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W;

		public static bool operator !=(Quaternion a, Quaternion b)
			=> a.X != b.X || a.Y != b.Y || a.Z != b.Z || a.W != b.W;
	}
}

