using System;
namespace GameEngine
{
	public struct Matrix4x4
	{
		//TODO: Sort this with regions

		public static readonly Matrix4x4 identity = new Matrix4x4(Vector4.unitX,Vector4.unitY,Vector4.unitZ,Vector4.unitW);
		public static readonly Matrix4x4 zero = new Matrix4x4(Vector4.zero,Vector4.zero,Vector4.zero,Vector4.zero);
		
		public float
			m00,m01,m02,m03,
			m10,m11,m12,m13,
			m20,m21,m22,m23,
			m30,m31,m32,m33;

		public Vector4 Row0 {
			get => new Vector4(m00,m01,m02,m03);
			set {
				m00 = value.x;
				m01 = value.y;
				m02 = value.z;
				m03 = value.w;
			}
		}
		public Vector4 Row1 {
			get => new Vector4(m10,m11,m12,m13);
			set {
				m10 = value.x;
				m11 = value.y;
				m12 = value.z;
				m13 = value.w;
			}
		}
		public Vector4 Row2 {
			get => new Vector4(m20,m21,m22,m23);
			set {
				m20 = value.x;
				m21 = value.y;
				m22 = value.z;
				m23 = value.w;
			}
		}
		public Vector4 Row3 {
			get => new Vector4(m30,m31,m32,m33);
			set {
				m30 = value.x;
				m31 = value.y;
				m32 = value.z;
				m33 = value.w;
			}
		}
		
		public Matrix4x4(Vector4 row0,Vector4 row1,Vector4 row2,Vector4 row3)
		{
			m00 = row0.x; m01 = row0.y; m02 = row0.z; m03 = row0.w;
			m10 = row1.x; m11 = row1.y; m12 = row1.z; m13 = row1.w;
			m20 = row2.x; m21 = row2.y; m22 = row2.z; m23 = row2.w;
			m30 = row3.x; m31 = row3.y; m32 = row3.z; m33 = row3.w;
		}
		public Matrix4x4(
			float m00,float m01,float m02,float m03,
			float m10,float m11,float m12,float m13,
			float m20,float m21,float m22,float m23,
			float m30,float m31,float m32,float m33)
		{
			this.m00 = m00; this.m01 = m01; this.m02 = m02; this.m03 = m03;
			this.m10 = m10; this.m11 = m11; this.m12 = m12; this.m13 = m13;
			this.m20 = m20; this.m21 = m21; this.m22 = m22; this.m23 = m23;
			this.m30 = m30; this.m31 = m31; this.m32 = m32; this.m33 = m33;
		}
		
		public float this[int row,int column]
		{
			get {
				switch(column) {
					case 0: switch(row) {
						case 0:	return m00;
						case 1:	return m01;
						case 2:	return m02;
						case 3:	return m03;
						default: throw new IndexOutOfRangeException("["+row+","+column+"] is not a valid matrix index.");
					}
					case 1: switch(row) {
						case 0:	return m10;
						case 1:	return m11;
						case 2:	return m12;
						case 3:	return m13;
						default: throw new IndexOutOfRangeException("["+row+","+column+"] is not a valid matrix index.");
					}
					case 2: switch(row) {
						case 0:	return m20;
						case 1:	return m21;
						case 2:	return m22;
						case 3:	return m23;
						default: throw new IndexOutOfRangeException("["+row+","+column+"] is not a valid matrix index.");
					}
					case 3: switch(row) {
						case 0:	return m30;
						case 1:	return m31;
						case 2:	return m32;
						case 3:	return m33;
						default: throw new IndexOutOfRangeException("["+row+","+column+"] is not a valid matrix index.");
					}
					default: throw new IndexOutOfRangeException("["+row+","+column+"] is not a valid matrix index.");
				}
			}
			set {
				switch(column) {
					case 0: switch(row) {
						case 0:	m00 = value; return;
						case 1:	m01 = value; return;
						case 2:	m02 = value; return;
						case 3:	m03 = value; return;
						default: throw new IndexOutOfRangeException("["+row+","+column+"] is not a valid matrix index.");
					}
					case 1: switch(row) {
						case 0:	m10 = value; return;
						case 1:	m11 = value; return;
						case 2:	m12 = value; return;
						case 3:	m13 = value; return;
						default: throw new IndexOutOfRangeException("["+row+","+column+"] is not a valid matrix index.");
					}
					case 2: switch(row) {
						case 0:	m20 = value; return;
						case 1:	m21 = value; return;
						case 2:	m22 = value; return;
						case 3:	m23 = value; return;
						default: throw new IndexOutOfRangeException("["+row+","+column+"] is not a valid matrix index.");
					}
					case 3: switch(row) {
						case 0:	m30 = value; return;
						case 1:	m31 = value; return;
						case 2:	m32 = value; return;
						case 3:	m33 = value; return;
						default: throw new IndexOutOfRangeException("["+row+","+column+"] is not a valid matrix index.");
					}
					default: throw new IndexOutOfRangeException("["+row+","+column+"] is not a valid matrix index.");
				}
			}
		}
		public float this[int id]
		{
			get {
				switch(id) {
					case 0:	return m00;
					case 1:	return m01;
					case 2:	return m02;
					case 3:	return m03;
					case 4:	return m10;
					case 5:	return m11;
					case 6:	return m12;
					case 7:	return m13;
					case 8:	return m20;
					case 9:	return m21;
					case 10: return m22;
					case 11: return m23;
					case 12: return m30;
					case 13: return m31;
					case 14: return m32;
					case 15: return m33;
					default: throw new IndexOutOfRangeException("["+id+"] single matrix index must range from 0 to 15.");
				}
			}
			set {
				switch(id) {
					case 0:	m00 = value; break;
					case 1:	m01 = value; break;
					case 2:	m02 = value; break;
					case 3:	m03 = value; break;
					case 4:	m10 = value; break;
					case 5:	m11 = value; break;
					case 6:	m12 = value; break;
					case 7:	m13 = value; break;
					case 8:	m20 = value; break;
					case 9:	m21 = value; break;
					case 10: m22 = value; break;
					case 11: m23 = value; break;
					case 12: m30 = value; break;
					case 13: m31 = value; break;
					case 14: m32 = value; break;
					case 15: m33 = value; break;
					default: throw new IndexOutOfRangeException("["+id+"] single matrix index must range from 0 to 15.");
				}
			}
		}
		//TODO:
		//This is slow,should store values in these variables
		//and have rows be like what this currently is
		
		public float Determinant
		{
			get {
				float m11 = m00,m12 = m01,m13 = m02,m14 = m03;
				float m21 = m10,m22 = m11,m23 = m12,m24 = m13;
				float m31 = m20,m32 = m21,m33 = m22,m34 = m23;
				float m41 = m30,m42 = m31,m43 = m32,m44 = m33;
				return	m11*m22*m33*m44-m11*m22*m34*m43+m11*m23*m34*m42-m11*m23*m32*m44+
						m11*m24*m32*m43-m11*m24*m33*m42-m12*m23*m34*m41+m12*m23*m31*m44-
						m12*m24*m31*m43+m12*m24*m33*m41-m12*m21*m33*m44+m12*m21*m34*m43+
						m13*m24*m31*m42-m13*m24*m32*m41+m13*m21*m32*m44-m13*m21*m34*m42+
						m13*m22*m34*m41-m13*m22*m31*m44-m14*m21*m32*m43+m14*m21*m33*m42-
						m14*m22*m33*m41+m14*m22*m31*m43-m14*m23*m31*m42+m14*m23*m32*m41;
			}
		}
		public Matrix4x4 Normalized {
			get {
				var matrix = this;
				matrix.Normalize();
				return matrix;
			}
		}
		public Matrix4x4 Inverted {
			get {
				var matrix = this;
				matrix.Invert();
				return matrix;
			}
		}
		public Matrix4x4 Transpose => identity;//Matrix4x4.Transpose(this);

		public static Matrix4x4 CreateTranslation(float x,float y,float z)
		{
			var result = identity;
			result.m30 = x;
			result.m31 = y;
			result.m32 = z;
			return result;
		}
		public static Matrix4x4 CreateTranslation(Vector3 vec)
		{
			return CreateTranslation(vec.x,vec.y,vec.z);
		}
		public static Matrix4x4 CreateRotationX(float eulerAngle)
		{
			float angle = eulerAngle*Mathf.Deg2Rad;
			float cos = Mathf.Cos(angle);
			float sin = Mathf.Sin(angle);
			var result = identity;
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
			var result = identity;
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
			var result = identity;
			result.m00 = cos;
			result.m01 = sin;
			result.m10 = -sin;
			result.m11 = cos;
			return result;
		}
		public static Matrix4x4 CreateRotation(float eulerRotX,float eulerRotY,float eulerRotZ)
		{
			return CreateRotation(new Vector3(eulerRotX,eulerRotY,eulerRotZ));
		}
		public static Matrix4x4 CreateRotation(Vector3 eulerRot)
		{
			eulerRot *= Mathf.Deg2Rad;

			float cX = Mathf.Cos(-eulerRot.x);
			float sX = Mathf.Sin(-eulerRot.x);
			float cY = Mathf.Cos(-eulerRot.y);
			float sY = Mathf.Sin(-eulerRot.y);
			float cZ = Mathf.Cos(eulerRot.z);
			float sZ = Mathf.Sin(eulerRot.z); 
			
			//zxy-totally best one,same as unity uses
			return new Matrix4x4(
				cY*cZ-sX*sY*sZ,	-cX*sZ,			cZ*sY+cY*sX*sZ,	0f,
				cZ*sX*sY+cY*sZ,	cX*cZ,			-cY*cZ*sX+sY*sZ,	0f,
				-cX*sY,			sX,				cX*cY,			0f,
				0f,				0f,				0f,				1f
			);
		}
		public static Matrix4x4 CreateRotation(Quaternion q)
		{
			q.Normalize();
			/*return new Matrix4x4(
				1f-(2f*q.y*q.y)-(2f*q.z*q.z),	2f*q.x*q.y-2f*q.z*q.w,		2f*q.x*q.z+2f*q.y*q.w,		0f,
				2f*q.x*q.y+2f*q.z*q.w,			1f-2f*q.x*q.x-2f*q.z*q.z,	2f*q.y*q.z-2f*q.x*q.w,		0f,
				2f*q.x*q.z-2f*q.y*q.w,			2f*q.y*q.z+2f*q.x*q.w,		1f-2f*q.x*q.x-2f*q.y*q.y,	0f,
				0f,								0f,							0f,							1f
			);*/
			float x = q.x;
			float y = q.y;
			float z = q.z;
			float w = q.w;
			float xx = q.x*q.x;
			float yy = q.y*q.y;
			float zz = q.z*q.z;
			float ww = q.w*q.w;
			return new Matrix4x4(
				ww+xx-yy-zz,		2f*(-w*z+x*y),		2f*(w*y+x*z),		0f,
				2f*(w*z+x*y),		ww-xx+yy-zz,		2f*(-w*x+y*z),		0f,
				2f*(-w*y+x*z),		2f*(w*x+y*z),		ww-xx-yy+zz,		0f,
				0f,					0f,					0f,					ww+xx+yy+zz);
			/*return new Matrix4x4(
				1f-(2f*q.y*q.y)-(2f*q.z*q.z),	(2f*q.x*q.y)+(2f*q.w*q.z),		(2f*q.x*q.z)-(2f*q.w*q.y),		0f,
				(2f*q.x*q.y)-(2f*q.w*q.z),		1f-(2f*q.x*q.x)-(2f*q.z*q.z),	(2f*q.y*q.z)+(2f*q.w*q.x),		0f,
				(2f*q.x*q.z)+(2f*q.w*q.y),		(2f*q.y*q.z)+(2f*q.w*q.x),		1f-(2f*q.x*q.x)-(2f*q.y*q.y),	0f,
				0f,								0f,								0f,								1f
			);*/
		}
		public static Matrix4x4 CreateFromAxisAngle(Vector3 axis,float angle)
		{
			var result = identity;
			axis.Normalize();
			float axisX = axis.x;
			float axisY = axis.y;
			float axisZ = axis.z;
			float cos = Mathf.Cos(-angle);
			float sin = Mathf.Sin(-angle);
			float t = 1f-cos;
			float tXX = t*axisX*axisX,
				tXY = t*axisX*axisY,
				tXZ = t*axisX*axisZ,
				tYY = t*axisY*axisY,
				tYZ = t*axisY*axisZ,
				tZZ = t*axisZ*axisZ;
			float sinX = sin*axisX,
				sinY = sin*axisY,
				sinZ = sin*axisZ;
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
			result.Row3 = Vector4.unitW;
			return result;
		}
		public static Matrix4x4 CreateFromQuaternion(Quaternion q)
		{
			q.ToAxisAngle(out var axis,out float angle);
			return CreateFromAxisAngle(axis,angle);
		}
		#region CreateScale
		public static Matrix4x4 CreateScale(float xyz)
		{
			var result = identity;
			result.m00 = xyz;
			result.m11 = xyz;
			result.m22 = xyz;
			return result;
		}
		public static Matrix4x4 CreateScale(float x,float y,float z)
		{
			var result = identity;
			result.m00 = x;
			result.m11 = y;
			result.m22 = z;
			return result;
		}
		public static Matrix4x4 CreateScale(Vector3 vec)
		{
			var result = identity;
			result.m00 = vec.x;
			result.m11 = vec.y;
			result.m22 = vec.z;
			return result;
		}
		#endregion
		public static Matrix4x4 CreateOrthographicOffCenter(float left,float right,float bottom,float top,float zNear,float zFar)
		{
			var result = identity;
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
		public static Matrix4x4 CreateOrthographic(float width,float height,float zNear,float zFar)
		{
			return CreateOrthographicOffCenter(-width/2,width/2,-height/2,height/2,zNear,zFar);
		}
		public static Matrix4x4 CreatePerspectiveFOV(float fovY,float aspect,float zNear,float zFar)
		{
			//No difference between LH and RH here
			if(fovY<=0 || fovY>Math.PI) {
				throw new ArgumentOutOfRangeException("fovY");
			}
			if(aspect<=0) {
				throw new ArgumentOutOfRangeException("aspect");
			}
			if(zNear<=0) {
				throw new ArgumentOutOfRangeException("zNear");
			}
			if(zFar<=0) {
				throw new ArgumentOutOfRangeException("zFar");
			}
			float yMax = zNear*(float)Math.Tan(0.5f*fovY);
			float yMin = -yMax;
			float xMin = yMin*aspect;
			float xMax = yMax*aspect;
			return CreatePerspective(xMin,xMax,yMin,yMax,zNear,zFar);
		}
		public static Matrix4x4 CreatePerspective(float left,float right,float bottom,float top,float zNear,float zFar)
		{
			var result = identity;
			if(zNear<=0) {
				throw new ArgumentOutOfRangeException("zNear");
			}
			if(zFar<=0) {
				throw new ArgumentOutOfRangeException("zFar");
			}
			if(zNear>=zFar) {
				throw new ArgumentOutOfRangeException("zNear");
			}
			float x = 2f*zNear/(right-left);
			float y = 2f*zNear/(top-bottom);
			float a = (right+left)/(right-left);
			float b = (top+bottom)/(top-bottom);
			float c = -(zFar+zNear)/(zFar-zNear);
			float d = -(2f*zFar*zNear)/(zFar-zNear);
			result.m00 = x;
			result.m01 = 0;
			result.m02 = 0;
			result.m03 = 0;
			result.m10 = 0;
			result.m11 = y;
			result.m12 = 0;
			result.m13 = 0;
			result.m20 = -a;	//
			result.m21 = -b;	//
			result.m22 = -c;	//
			result.m23 = 1;	//
			result.m30 = 0;
			result.m31 = 0;
			result.m32 = d;
			result.m33 = 0;
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
		
		public void Normalize()
		{
			float d = Determinant;
			Row0 /= d;
			Row1 /= d;
			Row2 /= d;
			Row3 /= d;
		}
		public void Invert()
		{
			//TODO: Optimize this???
			int[] colIdx = { 0,0,0,0 };
			int[] rowIdx = { 0,0,0,0 };
			int[] pivotIdx = { -1,-1,-1,-1 };
			float[,] inverse =  {
				{ m00,m01,m02,m03 },
				{ m10,m11,m12,m13 },
				{ m20,m21,m22,m23 },
				{ m30,m31,m32,m33 }
			};
			int icol = 0;
			int irow = 0;
			for(int i=0;i<4;i++) {
				float maxPivot = 0f;
				for(int j=0;j<4;j++) {
					if(pivotIdx[j]!=0) {
						for(int k=0;k<4;++k) {
							if(pivotIdx[k]==-1) {
								float absVal = Math.Abs(inverse[j,k]);
								if(absVal>maxPivot) {
									maxPivot = absVal;
									irow = j;
									icol = k;
								}
							}else if(pivotIdx[k]>0) {
								return;
							}
						}
					}
				}
				++pivotIdx[icol];
				if(irow!=icol) {
					for(int k=0;k<4;++k) {
						float f = inverse[irow,k];
						inverse[irow,k] = inverse[icol,k];
						inverse[icol,k] = f;
					}
				}
				rowIdx[i] = irow;
				colIdx[i] = icol;
				float pivot = inverse[icol,icol];
				if(pivot==0f) {
					return;
					//throw new InvalidOperationException("Matrix is singular and cannot be inverted.");
				}
				float oneOverPivot = 1f/pivot;
				inverse[icol,icol] = 1f;
				for(int k=0;k<4;++k) {
					inverse[icol,k] *= oneOverPivot;
				}
				for(int j=0;j<4;++j) {
					if(icol!=j) {
						float f = inverse[j,icol];
						inverse[j,icol] = 0f;
						for(int k=	0; k<4;++k) {
							inverse[j,k] -= inverse[icol,k]*f;
						}
					}
				}
			}
			for(int j=3;j>=0;--j) {
				int ir = rowIdx[j];
				int ic = colIdx[j];
				for(int k=0;k<4;++k) {
					float f = inverse[k,ir];
					inverse[k,ir] = inverse[k,ic];
					inverse[k,ic] = f;
				}
			}
			m00 = inverse[0,0];
			m01 = inverse[0,1];
			m02 = inverse[0,2];
			m03 = inverse[0,3];
			m10 = inverse[1,0];
			m11 = inverse[1,1];
			m12 = inverse[1,2];
			m13 = inverse[1,3];
			m20 = inverse[2,0];
			m21 = inverse[2,1];
			m22 = inverse[2,2];
			m23 = inverse[2,3];
			m30 = inverse[3,0];
			m31 = inverse[3,1];
			m32 = inverse[3,2];
			m33 = inverse[3,3];
		}
		/*public Matrix4x4 Invert2()
		{
			Matrix4x4 m = new Matrix4x4();

			float s0 = m00*m11-m10*m01;
			float s1 = m00*m12-m10*m02;
			float s2 = m00*m13-m10*m03;
			float s3 = m01*m12-m11*m02;
			float s4 = m01*m13-m11*m03;
			float s5 = m02*m13-m12*m03;

			float c5 = m22*m33-m32*m23;
			float c4 = m21*m33-m31*m23;
			float c3 = m21*m32-m31*m22;
			float c2 = m20*m33-m30*m23;
			float c1 = m20*m32-m30*m22;
			float c0 = m20*m31-m30*m21;

			//Should check for 0 determinant
			float det = 1f/(s0*c5-s1*c4+s2*c3+s3*c2-s4*c1+s5*c0);
			if(float.IsNaN(det)) {
				det = 1f;
			}

			m.m00 = ( m11*c5-m12*c4+m13*c3)*det;
			m.m01 = (-m01*c5+m02*c4-m03*c3)*det;
			m.m02 = ( m31*s5-m32*s4+m33*s3)*det;
			m.m03 = (-m21*s5+m22*s4-m23*s3)*det;

			m.m10 = (-m10*c5+m12*c2-m13*c1)*det;
			m.m11 = ( m00*c5-m02*c2+m03*c1)*det;
			m.m12 = (-m30*s5+m32*s2-m33*s1)*det;
			m.m13 = ( m20*s5-m22*s2+m23*s1)*det;

			m.m20 = ( m10*c4-m11*c2+m13*c0)*det;
			m.m21 = (-m00*c4+m01*c2-m03*c0)*det;
			m.m22 = ( m30*s4-m31*s2+m33*s0)*det;
			m.m23 = (-m20*s4+m21*s2-m23*s0)*det;

			m.m30 = (-m10*c3+m11*c1-m12*c0)*det;
			m.m31 = ( m00*c3-m01*c1+m02*c0)*det;
			m.m32 = (-m30*s3+m31*s1-m32*s0)*det;
			m.m33 = ( m20*s3-m21*s1+m22*s0)*det;

			return m;
		}*/
		public void ClearTranslation()
		{
			m30 = 0f;
			m31 = 0f;
			m32 = 0f;
		}
		public void ClearScale()
		{
			Row0 = new Vector4(((Vector3)Row0).Normalized,m03);
			Row1 = new Vector4(((Vector3)Row1).Normalized,m13);
			Row2 = new Vector4(((Vector3)Row2).Normalized,m23);
		}
		public void ClearRotation()
		{
			float mag0 = ((Vector3)Row0).Magnitude;
			float mag1 = ((Vector3)Row1).Magnitude;
			float mag2 = ((Vector3)Row2).Magnitude;
			Row0 = new Vector4(mag0,0f,0f,m03);
			Row1 = new Vector4(0f,mag1,0f,m13);
			Row2 = new Vector4(0f,0f,mag2,m23);
		}
		
		public Vector3 ExtractTranslation() => new Vector3(m30,m31,m32);
		public Vector3 ExtractScale() => new Vector3(
			Mathf.Sqrt(m00*m00+m01*m01+m02*m02),
			Mathf.Sqrt(m10*m10+m11*m11+m12*m12),
			Mathf.Sqrt(m20*m20+m21*m21+m22*m22)
		);
		public Quaternion ExtractQuaternion()
		{
			var row0 = Row0.XYZ.Normalized;
			var row1 = Row1.XYZ.Normalized;
			var row2 = Row2.XYZ.Normalized;

			//code below adapted from Blender

			var q = new Quaternion();
			float trace = 0.25f*(row0[0]+row1[1]+row2[2]+1f);

			if(trace>0) {
				float sq = Mathf.Sqrt(trace);

				q.w = sq;
				sq = 1f/(4f*sq);
				q.x = (row1[2]-row2[1])*sq;
				q.y = (row2[0]-row0[2])*sq;
				q.z = (row0[1]-row1[0])*sq;
			}else if(row0[0]>row1[1] && row0[0]>row2[2]) {
				float sq = 2f*Mathf.Sqrt(1f+row0[0]-row1[1]-row2[2]);

				q.x = 0.25f*sq;
				sq = 1f/sq;
				q.w = (row2[1]-row1[2])*sq;
				q.y = (row1[0]+row0[1])*sq;
				q.z = (row2[0]+row0[2])*sq;
			}else if(row1[1]>row2[2]) {
				float sq = 2f*Mathf.Sqrt(1f+row1[1]-row0[0]-row2[2]);

				q.y = 0.25f*sq;
				sq = 1f/sq;
				q.w = (row2[0]-row0[2])*sq;
				q.x = (row1[0]+row0[1])*sq;
				q.z = (row2[1]+row1[2])*sq;
			}else{
				float sq = 2f*Mathf.Sqrt(1f+row2[2]-row0[0]-row1[1]);

				q.z = 0.25f*sq;
				sq = 1f/sq;
				q.w = (row1[0]-row0[1])*sq;
				q.x = (row2[0]+row0[2])*sq;
				q.y = (row2[1]+row1[2])*sq;
			}
			q.Normalize();
			return q;
		}
		public Vector3 ExtractEuler()
		{
			float v1,v2,v3;
			if(m21<-1f) { //up
				v1 = -Mathf.PI/2f;
				v2 = 0f;
				v3 = Mathf.Atan2(m02,m00);
			}else if(m21>1f) { //down
				v1 = Mathf.PI/2f;
				v2 = 0f;
				v3 = -Mathf.Atan2(m02,m00);
			}else{
				v1 = Mathf.Asin(m21);
				v2 = Mathf.Atan2(-m20,m22);
				v3 = Mathf.Atan2(-m01,m11);
			}
			return new Vector3(-v1,-v2,v3)*(180f/Mathf.PI);
		}
		public Quaternion ExtractQuaternion2(bool row_normalise = true)
		{
			var tempRow0 = (Vector3)Row0;
			var tempRow1 = (Vector3)Row1;
			var tempRow2 = (Vector3)Row2;
			if(row_normalise) {
				tempRow0.Normalize();
				tempRow1.Normalize();
				tempRow2.Normalize();
			}
			var q = new Quaternion();
			float trace = 0.25f*(tempRow0[0]+tempRow1[1]+tempRow2[2]+1f);
			if(trace>0) {
				float sq = Mathf.Sqrt(trace);
				q.w = sq;
				sq = 1f/(4f*sq);
				q.x = (tempRow1[2]-tempRow2[1])*sq;
				q.y = (tempRow2[0]-tempRow0[2])*sq;
				q.z = (tempRow0[1]-tempRow1[0])*sq;
			}else if(tempRow0[0]>tempRow1[1] && tempRow0[0]>tempRow2[2]) {
				float sq = 2f*Mathf.Sqrt(1f+tempRow0[0]-tempRow1[1]-tempRow2[2]);
				q.x = 0.25f*sq;
				sq = 1f/sq;
				q.w = (tempRow2[1]-tempRow1[2])*sq;
				q.y = (tempRow1[0]+tempRow0[1])*sq;
				q.z = (tempRow2[0]+tempRow0[2])*sq;
			}else if(tempRow1[1]>tempRow2[2]) {
				float sq = 2f*Mathf.Sqrt(1f+tempRow1[1]-tempRow0[0]-tempRow2[2]);
				q.y = 0.25f*sq;
				sq = 1f/sq;
				q.w = (tempRow2[0]-tempRow0[2])*sq;
				q.x = (tempRow1[0]+tempRow0[1])*sq;
				q.z = (tempRow2[1]+tempRow1[2])*sq;
			}else{
				float sq = 2f*Mathf.Sqrt(1f+tempRow2[2]-tempRow0[0]-tempRow1[1]);
				q.z = 0.25f*sq;
				sq = 1f/sq;
				q.w = (tempRow1[0]-tempRow0[1])*sq;
				q.x = (tempRow2[0]+tempRow0[2])*sq;
				q.y = (tempRow2[1]+tempRow1[2])*sq;
			}
			q.Normalize();
			return q;
		}
		
		public void SetTranslation(Vector3 translation)
		{
			m30 = translation.x;
			m31 = translation.y;
			m32 = translation.z;
		}
		public void SetScale(Vector3 scale)
		{
			ClearScale();
			m00 *= scale.x;
			m01 *= scale.x;
			m02 *= scale.x;
			m10 *= scale.y;
			m11 *= scale.y;
			m12 *= scale.y;
			m20 *= scale.z;
			m21 *= scale.z;
			m22 *= scale.z;
		}
		
		public override string ToString()
		{
			return string.Format(" {0:F5}\t {1:F5}\t {2:F5}\t {3:F5}\n {4:F5}\t {5:F5}\t {6:F5}\t {7:F5}\n {8:F5}\t {9:F5}\t {10:F5}\t {11:F5}\n {12:F5}\t {13:F5}\t {14:F5}\t {15:F5}",new object[]
			{
				m00,m01,m02,m03,
				m10,m11,m12,m13,
				m20,m21,m22,m23,
				m30,m31,m32,m33
			}).Replace("-","-");
		}
		public override int GetHashCode()
		{
			return Row0.GetHashCode()^Row1.GetHashCode()<<2^Row2.GetHashCode()>>2^Row3.GetHashCode()>>1;
		}
		public override bool Equals(object other)
		{
			if(!(other is Matrix4x4)) {
				return false;
			}
			return Equals((Matrix4x4)other);
		}
		public bool Equals(Matrix4x4 other)
		{
			return 
				Row0==other.Row0 && Row1==other.Row1 && Row2==other.Row2 && Row3==other.Row3;
		}

		public static Matrix4x4 operator *(Matrix4x4 lhs,Matrix4x4 rhs) => new Matrix4x4 {
			m00 = lhs.m00*rhs.m00+lhs.m01*rhs.m10+lhs.m02*rhs.m20+lhs.m03*rhs.m30,
			m01 = lhs.m00*rhs.m01+lhs.m01*rhs.m11+lhs.m02*rhs.m21+lhs.m03*rhs.m31,
			m02 = lhs.m00*rhs.m02+lhs.m01*rhs.m12+lhs.m02*rhs.m22+lhs.m03*rhs.m32,
			m03 = lhs.m00*rhs.m03+lhs.m01*rhs.m13+lhs.m02*rhs.m23+lhs.m03*rhs.m33,
			m10 = lhs.m10*rhs.m00+lhs.m11*rhs.m10+lhs.m12*rhs.m20+lhs.m13*rhs.m30,
			m11 = lhs.m10*rhs.m01+lhs.m11*rhs.m11+lhs.m12*rhs.m21+lhs.m13*rhs.m31,
			m12 = lhs.m10*rhs.m02+lhs.m11*rhs.m12+lhs.m12*rhs.m22+lhs.m13*rhs.m32,
			m13 = lhs.m10*rhs.m03+lhs.m11*rhs.m13+lhs.m12*rhs.m23+lhs.m13*rhs.m33,
			m20 = lhs.m20*rhs.m00+lhs.m21*rhs.m10+lhs.m22*rhs.m20+lhs.m23*rhs.m30,
			m21 = lhs.m20*rhs.m01+lhs.m21*rhs.m11+lhs.m22*rhs.m21+lhs.m23*rhs.m31,
			m22 = lhs.m20*rhs.m02+lhs.m21*rhs.m12+lhs.m22*rhs.m22+lhs.m23*rhs.m32,
			m23 = lhs.m20*rhs.m03+lhs.m21*rhs.m13+lhs.m22*rhs.m23+lhs.m23*rhs.m33,
			m30 = lhs.m30*rhs.m00+lhs.m31*rhs.m10+lhs.m32*rhs.m20+lhs.m33*rhs.m30,
			m31 = lhs.m30*rhs.m01+lhs.m31*rhs.m11+lhs.m32*rhs.m21+lhs.m33*rhs.m31,
			m32 = lhs.m30*rhs.m02+lhs.m31*rhs.m12+lhs.m32*rhs.m22+lhs.m33*rhs.m32,
			m33 = lhs.m30*rhs.m03+lhs.m31*rhs.m13+lhs.m32*rhs.m23+lhs.m33*rhs.m33
		};
		public static Vector2 operator *(Vector2 vec,Matrix4x4 matrix)
		{
			return new Vector2(vec.x*matrix.m00+vec.y*matrix.m10+matrix.m30,vec.x*matrix.m01+vec.y*matrix.m11+matrix.m31);
		}
		public static Vector3 operator *(Matrix4x4 m,Vector3 v)
		{
			Vector3 result;
			result.x = m.m00*v.x+m.m01*v.y+m.m02*v.z;
			result.y = m.m10*v.x+m.m11*v.y+m.m12*v.z;
			result.z = m.m20*v.x+m.m21*v.y+m.m22*v.z;
			return result;
		}
		public static Vector4 operator *(Matrix4x4 lhs,Vector4 v) => new Vector4(
			lhs.m00*v.x+lhs.m01*v.y+lhs.m02*v.z+lhs.m03*v.w,
			lhs.m10*v.x+lhs.m11*v.y+lhs.m12*v.z+lhs.m13*v.w,
			lhs.m20*v.x+lhs.m21*v.y+lhs.m22*v.z+lhs.m23*v.w,
			lhs.m30*v.x+lhs.m31*v.y+lhs.m32*v.z+lhs.m33*v.w
		);
		public static bool operator==(Matrix4x4 left,Matrix4x4 right)
		{
			return left.Equals(right);
		}
		public static bool operator!=(Matrix4x4 left,Matrix4x4 right)
		{
			return !left.Equals(right);
		}

		public static implicit operator OpenTK.Matrix4(Matrix4x4 v) => new OpenTK.Matrix4(v.Row0,v.Row1,v.Row2,v.Row3);
		public static implicit operator Matrix4x4(OpenTK.Matrix4 v) => new Matrix4x4(v.Row0,v.Row1,v.Row2,v.Row3);
		public static implicit operator BulletSharp.Matrix(Matrix4x4 v) => new BulletSharp.Matrix {
			M11 = v.m00,M12 = v.m01,M13 = v.m02,M14 = v.m03,
			M21 = v.m10,M22 = v.m11,M23 = v.m12,M24 = v.m13,
			M31 = v.m20,M32 = v.m21,M33 = v.m22,M34 = v.m23,
			M41 = v.m30,M42 = v.m31,M43 = v.m32,M44 = v.m33
		};
		public static implicit operator Matrix4x4(BulletSharp.Matrix v) => new Matrix4x4(v.M11,v.M12,v.M13,v.M14,v.M21,v.M22,v.M23,v.M24,v.M31,v.M32,v.M33,v.M34,v.M41,v.M42,v.M43,v.M44);
		public static implicit operator double[](Matrix4x4 value)
		{
			var output = new double[16];
			for(int i=0;i<16;i++) {
				output[i] = value[i];
			}
			return output;
		}
	}
}
