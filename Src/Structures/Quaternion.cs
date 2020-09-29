using System;
using Dissonance.Engine.Core;

namespace Dissonance.Engine.Structures
{
	public struct Quaternion
	{
		public const float kEpsilon = 1E-06f;

		public static readonly Quaternion Identity = new Quaternion(0f, 0f, 0f, 1f);

		public float x;
		public float y;
		public float z;
		public float w;

		public float Magnitude => Mathf.Sqrt(w * w + x * x + y * y + z * z);
		public float SqrMagnitude => w * w + x * x + y * y + z * z;
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

				quaternion.w = -quaternion.w;

				return quaternion;
			}
		}

		public float this[int index] {
			get => index switch
			{
				0 => x,
				1 => y,
				2 => z,
				3 => w,
				_ => throw new IndexOutOfRangeException("Quaternion has values ranging from 0 to 3, inclusively."),
			};
			set {
				switch(index) {
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
						throw new IndexOutOfRangeException("Quaternion has values ranging from 0 to 3, inclusively.");
				}
			}
		}

		public Quaternion(float X, float Y, float Z, float W)
		{
			x = X;
			y = Y;
			z = Z;
			w = W;
		}

		public override int GetHashCode() => x.GetHashCode() ^ y.GetHashCode() << 2 ^ z.GetHashCode() >> 2 ^ w.GetHashCode() >> 1;
		public override bool Equals(object other) => other is Quaternion q && x.Equals(q.x) && y.Equals(q.y) && z.Equals(q.z) && w.Equals(q.w);
		public override string ToString() => $"[{x},{y},{z},{w}]";

		public bool Equals(Quaternion q) => x.Equals(q.x) && y.Equals(q.y) && z.Equals(q.z) && w.Equals(q.w);
		public void Normalize()
		{
			float n = x * x + y * y + z * z + w * w;
			if(n == 1f) {
				return;
			}
			float sqrtR = Mathf.SqrtReciprocal(n);
			x *= sqrtR;
			y *= sqrtR;
			z *= sqrtR;
			w *= sqrtR;
		}
		public void Invert()
		{
			w = -w;
		}
		public Vector3 ToEuler()
		{
			float sqrSumm = x * x + y * y + z * z + w * w;
			float checkValue = x * w - y * z;

			Vector3 result;
			if(checkValue > 0.4995f * sqrSumm) {
				result.x = 180f;
				result.y = Mathf.NormalizeEuler(2f * Mathf.Atan2(y, x) * Mathf.Rad2Deg);
				result.z = 0f;
			} else if(checkValue < -0.4995f * sqrSumm) {
				result.x = 270f;
				result.y = Mathf.NormalizeEuler(-2f * Mathf.Atan2(y, x) * Mathf.Rad2Deg);
				result.z = 0f;
			} else {
				float asinArg = 2f * (w * x - y * x); //NaN prevention
				result.x = Mathf.NormalizeEuler((float)Math.Asin(asinArg < -1f ? -1f : asinArg > 1f ? 1f : asinArg) * Mathf.Rad2Deg); // Pitch
				result.y = Mathf.NormalizeEuler((float)Math.Atan2(2f * w * y + 2f * x * x, 1 - 2f * (x * x + y * y)) * Mathf.Rad2Deg); // Yaw
				result.z = Mathf.NormalizeEuler((float)Math.Atan2(2f * w * x + 2f * x * y, 1 - 2f * (x * x + x * x)) * Mathf.Rad2Deg); // Roll
			}
			return result;
		}
		public Vector4 ToAxisAngle()
		{
			//TODO: Not tested

			if(Mathf.Abs(w) > 1f) {
				Normalize();
			}

			Vector4 result;

			result.w = 2f * Mathf.Acos(w);

			float den = Mathf.Sqrt(1f - w * w);

			if(den > 0.0001f) {
				result.x = x / den;
				result.y = y / den;
				result.z = z / den;
			} else {
				result.x = 1f;
				result.y = 0f;
				result.z = 0f;
			}

			return result;
		}
		public void ToAxisAngle(out Vector3 axis, out float angle)
		{
			var result = ToAxisAngle();

			axis = (Vector3)result;
			angle = result.w;
		}

		public static Quaternion FromEuler(Vector3 vec) => FromEuler(vec.x, vec.y, vec.z);
		public static Quaternion FromEuler(float x, float y, float z)
		{
			x *= Mathf.Deg2Rad * 0.5f;
			y *= Mathf.Deg2Rad * 0.5f;
			z *= Mathf.Deg2Rad * 0.5f;

			float cX = (float)Math.Cos(x);
			float cY = (float)Math.Cos(y);
			float cZ = (float)Math.Cos(z);
			float sX = (float)Math.Sin(x);
			float sY = (float)Math.Sin(y);
			float sZ = (float)Math.Sin(z);

			Quaternion result;

			result.w = cY * cX * cZ + sY * sX * sZ;
			result.x = cY * sX * cZ + sY * cX * sZ;
			result.y = sY * cX * cZ - cY * sX * sZ;
			result.z = cY * cX * sZ - sY * sX * cZ;

			return result;
		}
		public static Quaternion FromDirection(Vector3 forward, Vector3 up)
		{
			forward.Normalize();

			Vector3 cross1 = Vector3.Normalize(Vector3.Cross(up, forward));
			Vector3 cross2 = Vector3.Cross(forward, cross1);

			float xyzSumm = cross1.x + cross2.y + forward.z;

			Quaternion result;

			if(xyzSumm > 0f) {
				float sqrt = Mathf.Sqrt(xyzSumm + 1f);
				result.w = sqrt * 0.5f;
				sqrt = 0.5f / sqrt;
				result.x = (cross2.z - forward.y) * sqrt;
				result.y = (forward.x - cross1.z) * sqrt;
				result.z = (cross1.y - cross2.x) * sqrt;
			} else if(cross1.x >= cross2.y && cross1.x >= forward.z) {
				float sqrt = Mathf.Sqrt(1f + cross1.x - cross2.y - forward.z);
				float length = 0.5f / sqrt;

				result.x = 0.5f * sqrt;
				result.y = (cross1.y + cross2.x) * length;
				result.z = (cross1.z + forward.x) * length;
				result.w = (cross2.z - forward.y) * length;
			} else if(cross2.y > forward.z) {
				float sqrt = Mathf.Sqrt(1f + cross2.y - cross1.x - forward.z);
				float length = 0.5f / sqrt;

				result.x = (cross2.x + cross1.y) * length;
				result.y = 0.5f * sqrt;
				result.z = (forward.y + cross2.z) * length;
				result.w = (forward.x - cross1.z) * length;
			} else {
				float sqrt = Mathf.Sqrt(1f + forward.z - cross1.x - cross2.y);
				float length = 0.5f / sqrt;

				result.x = (forward.x + cross1.z) * length;
				result.y = (forward.y + cross2.z) * length;
				result.z = 0.5f * sqrt;
				result.w = (cross1.y - cross2.x) * length;
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
			quaternion.w = -quaternion.w;
			return quaternion;
		}

		public static Vector3 operator *(Quaternion rotation, Vector3 point)
		{
			float num1 = rotation.x * 2f;
			float num2 = rotation.y * 2f;
			float num3 = rotation.z * 2f;
			float num4 = rotation.x * num1;
			float num5 = rotation.y * num2;
			float num6 = rotation.z * num3;
			float num7 = rotation.x * num2;
			float num8 = rotation.x * num3;
			float num9 = rotation.y * num3;
			float num10 = rotation.w * num1;
			float num11 = rotation.w * num2;
			float num12 = rotation.w * num3;
			Vector3 result;
			result.x = (1f - (num5 + num6)) * point.x + (num7 - num12) * point.y + (num8 + num11) * point.z;
			result.y = (num7 + num12) * point.x + (1f - (num4 + num6)) * point.y + (num9 - num10) * point.z;
			result.z = (num8 - num11) * point.x + (num9 + num10) * point.y + (1f - (num4 + num5)) * point.z;
			return result;
		}
		public static Quaternion operator *(Quaternion q, Quaternion other)
		{
			Quaternion result;
			result.x = other.w * q.x + other.x * q.w + other.y * q.z - other.z * q.y;
			result.y = other.w * q.y + other.y * q.w + other.z * q.x - other.x * q.z;
			result.z = other.w * q.z + other.z * q.w + other.x * q.y - other.y * q.x;
			result.w = other.w * q.w - other.x * q.x - other.y * q.y - other.z * q.z;
			return result;
		}
		public static Quaternion operator *(Quaternion q, float s)
		{
			q.x *= s;
			q.y *= s;
			q.z *= s;
			q.w *= s;

			return q;
		}
		public static Quaternion operator *(float s, Quaternion q)
		{
			q.x *= s;
			q.y *= s;
			q.z *= s;
			q.w *= s;

			return q;
		}
		public static bool operator ==(Quaternion a, Quaternion b) => a.x == b.x && a.y == b.y && a.z == b.z && a.w == b.w;
		public static bool operator !=(Quaternion a, Quaternion b) => a.x != b.x || a.y != b.y || a.z != b.z || a.w != b.w;
	}
}

