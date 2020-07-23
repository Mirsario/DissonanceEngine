using System;
using Dissonance.Engine.Core;

namespace Dissonance.Engine.Structures
{
	partial struct Matrix4x4
	{
		//Translation
		public static Matrix4x4 CreateTranslation(Vector3 vec) => CreateTranslation(vec.x,vec.y,vec.z);
		public static Matrix4x4 CreateTranslation(float x,float y,float z)
		{
			var result = Identity;

			result.m30 = x;
			result.m31 = y;
			result.m32 = z;

			return result;
		}
		//Rotation
		public static Matrix4x4 CreateRotationX(float eulerAngle)
		{
			float angle = eulerAngle*Mathf.Deg2Rad;
			float cos = Mathf.Cos(angle);
			float sin = Mathf.Sin(angle);

			var result = Identity;

			result.m11 = cos;
			result.m12 = sin;
			result.m21 = -sin;
			result.m22 = cos;

			return result;
		}
		public static Matrix4x4 CreateRotationY(float eulerAngle)
		{
			float angle = eulerAngle*Mathf.Deg2Rad;
			float cos = Mathf.Cos(angle);
			float sin = Mathf.Sin(angle);

			var result = Identity;

			result.m00 = cos;
			result.m02 = -sin;
			result.m20 = sin;
			result.m22 = cos;

			return result;
		}
		public static Matrix4x4 CreateRotationZ(float eulerAngle)
		{
			float angle = eulerAngle*Mathf.Deg2Rad;
			float cos = Mathf.Cos(angle);
			float sin = Mathf.Sin(angle);

			var result = Identity;

			result.m00 = cos;
			result.m01 = sin;
			result.m10 = -sin;
			result.m11 = cos;

			return result;
		}
		public static Matrix4x4 CreateRotation(Vector3 vec)
			=> CreateRotation(vec.x,vec.y,vec.z);
		public static Matrix4x4 CreateRotation(float eulerRotX,float eulerRotY,float eulerRotZ)
		{
			eulerRotX *= Mathf.Deg2Rad;
			eulerRotY *= Mathf.Deg2Rad;
			eulerRotZ *= Mathf.Deg2Rad;

			float cX = Mathf.Cos(-eulerRotX);
			float sX = Mathf.Sin(-eulerRotX);
			float cY = Mathf.Cos(-eulerRotY);
			float sY = Mathf.Sin(-eulerRotY);
			float cZ = Mathf.Cos(eulerRotZ);
			float sZ = Mathf.Sin(eulerRotZ);

			//ZXY
			return new Matrix4x4(
				cY*cZ-sX*sY*sZ,-cX*sZ,cZ*sY+cY*sX*sZ,0f,
				cZ*sX*sY+cY*sZ,cX*cZ,-cY*cZ*sX+sY*sZ,0f,
				-cX*sY,sX,cX*cY,0f,
				0f,0f,0f,1f
			);
		}
		public static Matrix4x4 CreateRotation(Quaternion q)
		{
			float x = q.x*2f;
			float y = q.y*2f;
			float z = q.z*2f;

			float xx = q.x*x;
			float yy = q.y*y;
			float zz = q.z*z;

			float xy = q.x*y;
			float xz = q.x*z;
			float yz = q.y*z;

			float wx = q.w*x;
			float wy = q.w*y;
			float wz = q.w*z;

			return new Matrix4x4(
				1-(yy+zz),xy+wz,xz-wy,0f,
				xy-wz,1f-(xx+zz),yz+wx,0f,
				xz+wy,yz-wx,1f-(xx+yy),0f,
				0f,0f,0f,1f
			);
		}
		public static Matrix4x4 CreateFromAxisAngle(Vector3 axis,float angle)
		{
			var result = Identity;

			axis.Normalize();

			float axisX = axis.x;
			float axisY = axis.y;
			float axisZ = axis.z;

			float cos = Mathf.Cos(-angle);
			float sin = Mathf.Sin(-angle);
			float t = 1f-cos;

			float tXX = t*axisX*axisX;
			float tXY = t*axisX*axisY;
			float tXZ = t*axisX*axisZ;

			float tYY = t*axisY*axisY;
			float tYZ = t*axisY*axisZ;
			float tZZ = t*axisZ*axisZ;

			float sinX = sin*axisX;
			float sinY = sin*axisY;
			float sinZ = sin*axisZ;

			result.m00 = tXX+cos;
			result.m01 = tXY-sinZ;
			result.m02 = tXZ+sinY;
			result.m03 = 0;

			result.m10 = tXY+sinZ;
			result.m11 = tYY+cos;
			result.m12 = tYZ-sinX;
			result.m13 = 0;

			result.m20 = tXZ-sinY;
			result.m21 = tYZ+sinX;
			result.m22 = tZZ+cos;
			result.m23 = 0;

			result.Row3 = Vector4.UnitW;

			return result;
		}
		//Scale
		public static Matrix4x4 CreateScale(float xyz) => CreateScale(xyz,xyz,xyz);
		public static Matrix4x4 CreateScale(Vector3 vec) => CreateScale(vec.x,vec.y,vec.z);
		public static Matrix4x4 CreateScale(float x,float y,float z)
		{
			var result = Identity;

			result.m00 = x;
			result.m11 = y;
			result.m22 = z;

			return result;
		}
		//Projection
		public static Matrix4x4 CreateOrthographic(float width,float height,float zNear,float zFar) => CreateOrthographicOffCenter(-width/2,width/2,-height/2,height/2,zNear,zFar);
		public static Matrix4x4 CreateOrthographicOffCenter(float left,float right,float bottom,float top,float zNear,float zFar)
		{
			var result = Identity;
			float invRL = 1f/(right-left);
			float invTB = 1f/(top-bottom);
			float invFN = 1f/(zFar-zNear);

			result.m00 = 2*invRL;
			result.m11 = 2*invTB;
			result.m22 = -2*invFN;
			result.m30 = -(right+left)*invRL;
			result.m31 = -(top+bottom)*invTB;
			result.m32 = -(zFar+zNear)*invFN;

			return result;
		}
		public static Matrix4x4 CreatePerspectiveFOV(float fovY,float aspect,float zNear,float zFar)
		{
			if(fovY<=0 || fovY>Math.PI) {
				throw new ArgumentOutOfRangeException(nameof(fovY));
			}

			if(aspect<=0) {
				throw new ArgumentOutOfRangeException(nameof(aspect));
			}

			if(zNear<=0) {
				throw new ArgumentOutOfRangeException(nameof(zNear));
			}

			if(zFar<=0) {
				throw new ArgumentOutOfRangeException(nameof(zFar));
			}

			float yMax = zNear*(float)Math.Tan(0.5f*fovY);
			float yMin = -yMax;
			float xMin = yMin*aspect;
			float xMax = yMax*aspect;

			return CreatePerspective(xMin,xMax,yMin,yMax,zNear,zFar);
		}
		public static Matrix4x4 CreatePerspective(float left,float right,float bottom,float top,float zNear,float zFar)
		{
			if(zNear<=0) {
				throw new ArgumentOutOfRangeException(nameof(zNear));
			}

			if(zFar<=0) {
				throw new ArgumentOutOfRangeException(nameof(zFar));
			}

			if(zNear>=zFar) {
				throw new ArgumentOutOfRangeException(nameof(zNear));
			}

			float x = 2f*zNear/(right-left);
			float y = 2f*zNear/(top-bottom);
			float a = (right+left)/(right-left);
			float b = (top+bottom)/(top-bottom);
			float c = -(zFar+zNear)/(zFar-zNear);
			float d = -(2f*zFar*zNear)/(zFar-zNear);

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
		public static Matrix4x4 LookAt(Vector3 eye,Vector3 target,Vector3 up)
		{
			var z = Vector3.Normalize(target-eye);
			var x = Vector3.Normalize(Vector3.Cross(up,z));
			var y = Vector3.Normalize(Vector3.Cross(z,x));

			Matrix4x4 result;

			result.m00 = x.x;
			result.m01 = y.x;
			result.m02 = z.x;
			result.m03 = 0f;

			result.m10 = x.y;
			result.m11 = y.y;
			result.m12 = z.y;
			result.m13 = 0f;

			result.m20 = x.z;
			result.m21 = y.z;
			result.m22 = z.z;
			result.m23 = 0f;

			result.m30 = -(x.x*eye.x+x.y*eye.y+x.z*eye.z);
			result.m31 = -(y.x*eye.x+y.y*eye.y+y.z*eye.z);
			result.m32 = -(z.x*eye.x+z.y*eye.y+z.z*eye.z);
			result.m33 = 1f;

			return result;
		}
		//Etc
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
