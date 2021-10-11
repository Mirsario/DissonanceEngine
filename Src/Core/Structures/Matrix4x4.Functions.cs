using System;

namespace Dissonance.Engine
{
	partial struct Matrix4x4
	{
		// Translation
		
		public static Matrix4x4 CreateTranslation(Vector3 vec) => CreateTranslation(vec.X, vec.Y, vec.Z);
		
		public static Matrix4x4 CreateTranslation(float x, float y, float z)
		{
			var result = Identity;

			result.m30 = x;
			result.m31 = y;
			result.m32 = z;

			return result;
		}
		
		// Rotation
		
		public static Matrix4x4 CreateRotationX(float eulerAngle)
		{
			float angle = eulerAngle * MathHelper.Deg2Rad;
			float cos = MathF.Cos(angle);
			float sin = MathF.Sin(angle);

			var result = Identity;

			result.m11 = cos;
			result.m12 = sin;
			result.m21 = -sin;
			result.m22 = cos;

			return result;
		}
		
		public static Matrix4x4 CreateRotationY(float eulerAngle)
		{
			float angle = eulerAngle * MathHelper.Deg2Rad;
			float cos = MathF.Cos(angle);
			float sin = MathF.Sin(angle);

			var result = Identity;

			result.m00 = cos;
			result.m02 = -sin;
			result.m20 = sin;
			result.m22 = cos;

			return result;
		}
		
		public static Matrix4x4 CreateRotationZ(float eulerAngle)
		{
			float angle = eulerAngle * MathHelper.Deg2Rad;
			float cos = MathF.Cos(angle);
			float sin = MathF.Sin(angle);

			var result = Identity;

			result.m00 = cos;
			result.m01 = sin;
			result.m10 = -sin;
			result.m11 = cos;

			return result;
		}
		
		public static Matrix4x4 CreateRotation(Vector3 vec)
			=> CreateRotation(vec.X, vec.Y, vec.Z);
		
		public static Matrix4x4 CreateRotation(float eulerRotX, float eulerRotY, float eulerRotZ)
		{
			eulerRotX *= MathHelper.Deg2Rad;
			eulerRotY *= MathHelper.Deg2Rad;
			eulerRotZ *= MathHelper.Deg2Rad;

			float cX = MathF.Cos(-eulerRotX);
			float sX = MathF.Sin(-eulerRotX);
			float cY = MathF.Cos(-eulerRotY);
			float sY = MathF.Sin(-eulerRotY);
			float cZ = MathF.Cos(eulerRotZ);
			float sZ = MathF.Sin(eulerRotZ);

			// ZXY
			return new Matrix4x4(
				cY * cZ - sX * sY * sZ, -cX * sZ, cZ * sY + cY * sX * sZ, 0f,
				cZ * sX * sY + cY * sZ, cX * cZ, -cY * cZ * sX + sY * sZ, 0f,
				-cX * sY, sX, cX * cY, 0f,
				0f, 0f, 0f, 1f
			);
		}
		
		public static Matrix4x4 CreateRotation(Quaternion q)
		{
			float x = q.X * 2f;
			float y = q.Y * 2f;
			float z = q.Z * 2f;

			float xx = q.X * x;
			float yy = q.Y * y;
			float zz = q.Z * z;

			float xy = q.X * y;
			float xz = q.X * z;
			float yz = q.Y * z;

			float wx = q.W * x;
			float wy = q.W * y;
			float wz = q.W * z;

			return new Matrix4x4(
				1 - (yy + zz), xy + wz, xz - wy, 0f,
				xy - wz, 1f - (xx + zz), yz + wx, 0f,
				xz + wy, yz - wx, 1f - (xx + yy), 0f,
				0f, 0f, 0f, 1f
			);
		}
		
		public static Matrix4x4 CreateFromAxisAngle(Vector3 axis, float angle)
		{
			var result = Identity;

			axis.Normalize();

			float axisX = axis.X;
			float axisY = axis.Y;
			float axisZ = axis.Z;

			float cos = MathF.Cos(-angle);
			float sin = MathF.Sin(-angle);
			float t = 1f - cos;

			float tXX = t * axisX * axisX;
			float tXY = t * axisX * axisY;
			float tXZ = t * axisX * axisZ;

			float tYY = t * axisY * axisY;
			float tYZ = t * axisY * axisZ;
			float tZZ = t * axisZ * axisZ;

			float sinX = sin * axisX;
			float sinY = sin * axisY;
			float sinZ = sin * axisZ;

			result.m00 = tXX + cos;
			result.m01 = tXY - sinZ;
			result.m02 = tXZ + sinY;
			result.m03 = 0;

			result.m10 = tXY + sinZ;
			result.m11 = tYY + cos;
			result.m12 = tYZ - sinX;
			result.m13 = 0;

			result.m20 = tXZ - sinY;
			result.m21 = tYZ + sinX;
			result.m22 = tZZ + cos;
			result.m23 = 0;

			result.Row3 = Vector4.UnitW;

			return result;
		}
		
		// Scale
		
		public static Matrix4x4 CreateScale(float xyz) => CreateScale(xyz, xyz, xyz);
		
		public static Matrix4x4 CreateScale(Vector3 vec) => CreateScale(vec.X, vec.Y, vec.Z);
		
		public static Matrix4x4 CreateScale(float x, float y, float z)
		{
			var result = Identity;

			result.m00 = x;
			result.m11 = y;
			result.m22 = z;

			return result;
		}
		
		// Projection
		
		public static Matrix4x4 CreateOrthographic(float width, float height, float zNear, float zFar) => CreateOrthographicOffCenter(-width / 2, width / 2, -height / 2, height / 2, zNear, zFar);
		
		public static Matrix4x4 CreateOrthographicOffCenter(float left, float right, float bottom, float top, float zNear, float zFar)
		{
			Matrix4x4 result;

			result.m00 = 2f / (right - left);
			result.m01 = 0f;
			result.m02 = 0f;
			result.m03 = 0f;

			result.m10 = 0f;
			result.m11 = 2f / (top - bottom);
			result.m12 = 0f;
			result.m13 = 0f;

			result.m20 = 0f;
			result.m21 = 0f;
			result.m22 = 1f / (zFar - zNear);
			result.m23 = 0f;

			result.m30 = (left + right) / (left - right);
			result.m31 = (top + bottom) / (bottom - top);
			result.m32 = -zNear / (zFar - zNear);
			result.m33 = 1f;

			return result;
		}
		
		public static Matrix4x4 CreatePerspectiveFOV(float fovY, float aspect, float zNear, float zFar)
		{
			if (fovY <= 0 || fovY > Math.PI) {
				throw new ArgumentOutOfRangeException(nameof(fovY));
			}

			if (aspect <= 0) {
				throw new ArgumentOutOfRangeException(nameof(aspect));
			}

			if (zNear <= 0) {
				throw new ArgumentOutOfRangeException(nameof(zNear));
			}

			if (zFar <= 0) {
				throw new ArgumentOutOfRangeException(nameof(zFar));
			}

			float yMax = zNear * (float)Math.Tan(0.5f * fovY);
			float yMin = -yMax;
			float xMin = yMin * aspect;
			float xMax = yMax * aspect;

			return CreatePerspective(xMin, xMax, yMin, yMax, zNear, zFar);
		}
		
		public static Matrix4x4 CreatePerspective(float left, float right, float bottom, float top, float zNear, float zFar)
		{
			if (zNear <= 0) {
				throw new ArgumentOutOfRangeException(nameof(zNear));
			}

			if (zFar <= 0) {
				throw new ArgumentOutOfRangeException(nameof(zFar));
			}

			if (zNear >= zFar) {
				throw new ArgumentOutOfRangeException(nameof(zNear));
			}

			float x = 2f * zNear / (right - left);
			float y = 2f * zNear / (top - bottom);
			float a = (right + left) / (right - left);
			float b = (top + bottom) / (top - bottom);
			float c = -(zFar + zNear) / (zFar - zNear);
			float d = -(2f * zFar * zNear) / (zFar - zNear);

			Matrix4x4 result;

			result.m00 = x;
			result.m01 = 0f;
			result.m02 = 0f;
			result.m03 = 0f;

			result.m10 = 0f;
			result.m11 = y;
			result.m12 = 0f;
			result.m13 = 0f;

			result.m20 = -a;
			result.m21 = -b;
			result.m22 = -c;
			result.m23 = 1f;

			result.m30 = 0f;
			result.m31 = 0f;
			result.m32 = d;
			result.m33 = 0f;

			return result;
		}
		
		public static Matrix4x4 LookAt(Vector3 eye, Vector3 target, Vector3 up)
		{
			var z = Vector3.Normalize(target - eye);
			var x = Vector3.Normalize(Vector3.Cross(up, z));
			var y = Vector3.Normalize(Vector3.Cross(z, x));

			Matrix4x4 result;

			result.m00 = x.X;
			result.m01 = y.X;
			result.m02 = z.X;
			result.m03 = 0f;

			result.m10 = x.Y;
			result.m11 = y.Y;
			result.m12 = z.Y;
			result.m13 = 0f;

			result.m20 = x.Z;
			result.m21 = y.Z;
			result.m22 = z.Z;
			result.m23 = 0f;

			result.m30 = -(x.X * eye.X + x.Y * eye.Y + x.Z * eye.Z);
			result.m31 = -(y.X * eye.X + y.Y * eye.Y + y.Z * eye.Z);
			result.m32 = -(z.X * eye.X + z.Y * eye.Y + z.Z * eye.Z);
			result.m33 = 1f;

			return result;
		}
		
		// Etc

		public static Matrix4x4 Normalize(Matrix4x4 matrix)
		{
			matrix.Normalize();

			return matrix;
		}

		public static Matrix4x4 Invert(Matrix4x4 matrix)
		{
			matrix.Invert();

			return matrix;
		}
	}
}
