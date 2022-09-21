using System;

namespace Dissonance.Engine;

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

	public static unsafe implicit operator Matrix4x4(System.Numerics.Matrix4x4 v)
		=> *(Matrix4x4*)(void*)&v;

	public static unsafe implicit operator System.Numerics.Matrix4x4(Matrix4x4 v)
		=> *(System.Numerics.Matrix4x4*)(void*)&v;

	public static unsafe implicit operator Span<float>(Matrix4x4 value)
		=> new(&value, 16);

	public static unsafe implicit operator ReadOnlySpan<float>(Matrix4x4 value)
		=> new(&value, 16);
}
