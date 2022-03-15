namespace Dissonance.Engine
{
	partial struct Matrix4x4
	{
		// Matrix4x4

		public static bool operator ==(Matrix4x4 left, Matrix4x4 right) => left.Equals(right);

		public static bool operator !=(Matrix4x4 left, Matrix4x4 right) => !left.Equals(right);

		public static Matrix4x4 operator *(Matrix4x4 lhs, Matrix4x4 rhs)
		{
			Matrix4x4 result;

			result.m00 = lhs.m00 * rhs.m00 + lhs.m01 * rhs.m10 + lhs.m02 * rhs.m20 + lhs.m03 * rhs.m30;
			result.m01 = lhs.m00 * rhs.m01 + lhs.m01 * rhs.m11 + lhs.m02 * rhs.m21 + lhs.m03 * rhs.m31;
			result.m02 = lhs.m00 * rhs.m02 + lhs.m01 * rhs.m12 + lhs.m02 * rhs.m22 + lhs.m03 * rhs.m32;
			result.m03 = lhs.m00 * rhs.m03 + lhs.m01 * rhs.m13 + lhs.m02 * rhs.m23 + lhs.m03 * rhs.m33;
			result.m10 = lhs.m10 * rhs.m00 + lhs.m11 * rhs.m10 + lhs.m12 * rhs.m20 + lhs.m13 * rhs.m30;
			result.m11 = lhs.m10 * rhs.m01 + lhs.m11 * rhs.m11 + lhs.m12 * rhs.m21 + lhs.m13 * rhs.m31;
			result.m12 = lhs.m10 * rhs.m02 + lhs.m11 * rhs.m12 + lhs.m12 * rhs.m22 + lhs.m13 * rhs.m32;
			result.m13 = lhs.m10 * rhs.m03 + lhs.m11 * rhs.m13 + lhs.m12 * rhs.m23 + lhs.m13 * rhs.m33;
			result.m20 = lhs.m20 * rhs.m00 + lhs.m21 * rhs.m10 + lhs.m22 * rhs.m20 + lhs.m23 * rhs.m30;
			result.m21 = lhs.m20 * rhs.m01 + lhs.m21 * rhs.m11 + lhs.m22 * rhs.m21 + lhs.m23 * rhs.m31;
			result.m22 = lhs.m20 * rhs.m02 + lhs.m21 * rhs.m12 + lhs.m22 * rhs.m22 + lhs.m23 * rhs.m32;
			result.m23 = lhs.m20 * rhs.m03 + lhs.m21 * rhs.m13 + lhs.m22 * rhs.m23 + lhs.m23 * rhs.m33;
			result.m30 = lhs.m30 * rhs.m00 + lhs.m31 * rhs.m10 + lhs.m32 * rhs.m20 + lhs.m33 * rhs.m30;
			result.m31 = lhs.m30 * rhs.m01 + lhs.m31 * rhs.m11 + lhs.m32 * rhs.m21 + lhs.m33 * rhs.m31;
			result.m32 = lhs.m30 * rhs.m02 + lhs.m31 * rhs.m12 + lhs.m32 * rhs.m22 + lhs.m33 * rhs.m32;
			result.m33 = lhs.m30 * rhs.m03 + lhs.m31 * rhs.m13 + lhs.m32 * rhs.m23 + lhs.m33 * rhs.m33;

			return result;
		}

		// Vector2

		public static Vector2 operator *(Vector2 vec, Matrix4x4 matrix)
			=> new(vec.X * matrix.m00 + vec.Y * matrix.m10 + matrix.m30, vec.X * matrix.m01 + vec.Y * matrix.m11 + matrix.m31);
		
		public static Vector3 operator *(Matrix4x4 m, Vector3 v)
		{
			Vector3 result;

			result.X = m.m00 * v.X + m.m10 * v.Y + m.m20 * v.Z + m.m30;
			result.Y = m.m01 * v.X + m.m11 * v.Y + m.m21 * v.Z + m.m31;
			result.Z = m.m02 * v.X + m.m12 * v.Y + m.m22 * v.Z + m.m32;

			return result;
		}

		public static Vector4 operator *(Matrix4x4 lhs, Vector4 v)
		{
			Vector4 result;

			result.X = lhs.m00 * v.X + lhs.m01 * v.Y + lhs.m02 * v.Z + lhs.m03 * v.W;
			result.Y = lhs.m10 * v.X + lhs.m11 * v.Y + lhs.m12 * v.Z + lhs.m13 * v.W;
			result.Z = lhs.m20 * v.X + lhs.m21 * v.Y + lhs.m22 * v.Z + lhs.m23 * v.W;
			result.W = lhs.m30 * v.X + lhs.m31 * v.Y + lhs.m32 * v.Z + lhs.m33 * v.W;

			return result;
		}

		// Casts

		public static implicit operator Matrix4x4(BulletSharp.Math.Matrix v) => new(
			v.M11, v.M12, v.M13, v.M14,
			v.M21, v.M22, v.M23, v.M24,
			v.M31, v.M32, v.M33, v.M34,
			v.M41, v.M42, v.M43, v.M44
		);

		public static implicit operator BulletSharp.Math.Matrix(Matrix4x4 v) => new() {
			M11 = v.m00,
			M12 = v.m01,
			M13 = v.m02,
			M14 = v.m03,
			M21 = v.m10,
			M22 = v.m11,
			M23 = v.m12,
			M24 = v.m13,
			M31 = v.m20,
			M32 = v.m21,
			M33 = v.m22,
			M34 = v.m23,
			M41 = v.m30,
			M42 = v.m31,
			M43 = v.m32,
			M44 = v.m33
		};

		public static implicit operator double[](Matrix4x4 value)
		{
			var output = new double[16];

			for (int i = 0; i < 16; i++) {
				output[i] = value[i];
			}

			return output;
		}
	}
}
