using System;
namespace GameEngine
{
	public struct Matrix4x4
	{
		//Constants
		public const int Length = 16;

		//Read-only fields
		public static readonly Matrix4x4 Identity = new Matrix4x4(Vector4.UnitX,Vector4.UnitY,Vector4.UnitZ,Vector4.UnitW);
		public static readonly Matrix4x4 Zero = new Matrix4x4(Vector4.Zero,Vector4.Zero,Vector4.Zero,Vector4.Zero);

		//Fields
		public float m00,m01,m02,m03,m10,m11,m12,m13,m20,m21,m22,m23,m30,m31,m32,m33;

		//Properties
		public float Determinant =>
			m00*m11*m22*m33-m00*m11*m23*m32+m00*m12*m23*m31-m00*m12*m21*m33+
			m00*m13*m21*m32-m00*m13*m22*m31-m01*m12*m23*m30+m01*m12*m20*m33-
			m01*m13*m20*m32+m01*m13*m22*m30-m01*m10*m22*m33+m01*m10*m23*m32+
			m02*m13*m20*m31-m02*m13*m21*m30+m02*m10*m21*m33-m02*m10*m23*m31+
			m02*m11*m23*m30-m02*m11*m20*m33-m03*m10*m21*m32+m03*m10*m22*m31-
			m03*m11*m22*m30+m03*m11*m20*m32-m03*m12*m20*m31+m03*m12*m21*m30;
		public Matrix4x4 Transpose => throw new NotImplementedException(); //Matrix4x4.Transpose(this);
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

		//Indexers
		public float this[int row,int column] {
			get {
				return column switch
				{
					0 => row switch
					{
						0 => m00,
						1 => m01,
						2 => m02,
						3 => m03,
						_ => throw new IndexOutOfRangeException("["+row+","+column+"] is not a valid matrix index."),
					},
					1 => row switch
					{
						0 => m10,
						1 => m11,
						2 => m12,
						3 => m13,
						_ => throw new IndexOutOfRangeException("["+row+","+column+"] is not a valid matrix index."),
					},
					2 => row switch
					{
						0 => m20,
						1 => m21,
						2 => m22,
						3 => m23,
						_ => throw new IndexOutOfRangeException("["+row+","+column+"] is not a valid matrix index."),
					},
					3 => row switch
					{
						0 => m30,
						1 => m31,
						2 => m32,
						3 => m33,
						_ => throw new IndexOutOfRangeException("["+row+","+column+"] is not a valid matrix index."),
					},
					_ => throw new IndexOutOfRangeException("["+row+","+column+"] is not a valid matrix index."),
				};
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
		public float this[int id] {
			get {
				return id switch
				{
					0 => m00,
					1 => m01,
					2 => m02,
					3 => m03,
					4 => m10,
					5 => m11,
					6 => m12,
					7 => m13,
					8 => m20,
					9 => m21,
					10 => m22,
					11 => m23,
					12 => m30,
					13 => m31,
					14 => m32,
					15 => m33,
					_ => throw new IndexOutOfRangeException("["+id+"] single matrix index must range from 0 to 15."),
				};
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

		//Constructors
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
			float m30,float m31,float m32,float m33
		){
			this.m00 = m00; this.m01 = m01; this.m02 = m02; this.m03 = m03;
			this.m10 = m10; this.m11 = m11; this.m12 = m12; this.m13 = m13;
			this.m20 = m20; this.m21 = m21; this.m22 = m22; this.m23 = m23;
			this.m30 = m30; this.m31 = m31; this.m32 = m32; this.m33 = m33;
		}

		//Override Instance Methods
		public override string ToString() => $"[{m00}, {m01}, {m02}, {m03},\n {m10}, {m11}, {m12}, {m13},\n {m20}, {m21}, {m22}, {m23},\n {m30}, {m31}, {m32}, {m33}]";
		public override int GetHashCode() => Row0.GetHashCode()^Row1.GetHashCode()<<2^Row2.GetHashCode()>>2^Row3.GetHashCode()>>1;
		public override bool Equals(object other) => other is Matrix4x4 matrix && Equals(matrix);
		
		//Instance Methods
		public bool Equals(Matrix4x4 o) => m00==o.m00 && m01==o.m01 && m02==o.m02 && m03==o.m03 && m10==o.m10 && m11==o.m11 && m12==o.m12 && m13==o.m13 && m20==o.m20 && m21==o.m21 && m22==o.m22 && m23==o.m23;
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
			for(int i = 0;i<4;i++) {
				float maxPivot = 0f;
				for(int j = 0;j<4;j++) {
					if(pivotIdx[j]!=0) {
						for(int k = 0;k<4;++k) {
							if(pivotIdx[k]==-1) {
								float absVal = Math.Abs(inverse[j,k]);
								if(absVal>maxPivot) {
									maxPivot = absVal;
									irow = j;
									icol = k;
								}
							} else if(pivotIdx[k]>0) {
								return;
							}
						}
					}
				}
				++pivotIdx[icol];
				if(irow!=icol) {
					for(int k = 0;k<4;++k) {
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
				for(int k = 0;k<4;++k) {
					inverse[icol,k] *= oneOverPivot;
				}
				for(int j = 0;j<4;++j) {
					if(icol!=j) {
						float f = inverse[j,icol];
						inverse[j,icol] = 0f;
						for(int k = 0;k<4;++k) {
							inverse[j,k] -= inverse[icol,k]*f;
						}
					}
				}
			}
			for(int j = 3;j>=0;--j) {
				int ir = rowIdx[j];
				int ic = colIdx[j];
				for(int k = 0;k<4;++k) {
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
		//Clear
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
		//Extract
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
			float trace = 0.25f*(row0.x+row1.y+row2.z+1f);

			if(trace>0) {
				float sq = Mathf.Sqrt(trace);

				q.w = sq;
				sq = 1f/(4f*sq);
				q.x = (row1.z-row2.y)*sq;
				q.y = (row2.x-row0.z)*sq;
				q.z = (row0.y-row1.x)*sq;
			}else if(row0.x>row1.y && row0.x>row2.z) {
				float sq = 2f*Mathf.Sqrt(1f+row0.x-row1.y-row2.z);

				q.x = 0.25f*sq;
				sq = 1f/sq;
				q.w = (row2.y-row1.z)*sq;
				q.y = (row1.x+row0.y)*sq;
				q.z = (row2.x+row0.z)*sq;
			}else if(row1.y>row2.z) {
				float sq = 2f*Mathf.Sqrt(1f+row1.y-row0.x-row2.z);

				q.y = 0.25f*sq;
				sq = 1f/sq;
				q.w = (row2.x-row0.z)*sq;
				q.x = (row1.x+row0.y)*sq;
				q.z = (row2.y+row1.z)*sq;
			}else{
				float sq = 2f*Mathf.Sqrt(1f+row2.z-row0.x-row1.y);

				q.z = 0.25f*sq;
				sq = 1f/sq;
				q.w = (row1.x-row0.y)*sq;
				q.x = (row2.x+row0.z)*sq;
				q.y = (row2.y+row1.z)*sq;
			}
			q.Normalize();
			return q;
		}
		public Vector3 ExtractEuler()
		{
			float v1,v2,v3;
			if(m21<-1f) { //up
				v1 = -Mathf.HalfPI;
				v2 = 0f;
				v3 = Mathf.Atan2(m02,m00);
			}else if(m21>1f) { //down
				v1 = Mathf.HalfPI;
				v2 = 0f;
				v3 = -Mathf.Atan2(m02,m00);
			}else{
				v1 = Mathf.Asin(m21);
				v2 = Mathf.Atan2(-m20,m22);
				v3 = Mathf.Atan2(-m01,m11);
			}
			return new Vector3(-v1,-v2,v3)*Mathf.Rad2Deg;
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
		//Set
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
		public void SetRotation(Quaternion q) => SetRotationAndScale(q,ExtractScale());
		public void SetRotationAndScale(Quaternion q,Vector3 scale)
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

			m00 = (1f-(yy+zz))*scale.x;	m01 = (xy+wz)*scale.x;		m02 = (xz-wy)*scale.x;
			m10 = (xy-wz)*scale.y;		m11 = (1f-(xx+zz))*scale.y;	m12 = (yz+wx)*scale.y;
			m20 = (xz+wy)*scale.z;		m21 = (yz-wx)*scale.z;		m22 = (1f-(xx+yy))*scale.z;
		}

		//Static Methods
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
		public static Matrix4x4 CreateRotation(Vector3 vec) => CreateRotation(vec.x,vec.y,vec.z);
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
				cY*cZ-sX*sY*sZ,	-cX*sZ,			cZ*sY+cY*sX*sZ,	0f,
				cZ*sX*sY+cY*sZ,	cX*cZ,			-cY*cZ*sX+sY*sZ,0f,
				-cX*sY,			sX,				cX*cY,			0f,
				0f,				0f,				0f,				1f
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
				1-(yy+zz),	xy+wz,		xz-wy,		0f,
				xy-wz,		1f-(xx+zz),	yz+wx,		0f,
				xz+wy,		yz-wx,		1f-(xx+yy),	0f,
				0f,			0f,			0f,			1f
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
			//No difference between LH and RH here
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
			var result = Identity;
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

		//Operators
		public static Matrix4x4 operator *(Matrix4x4 lhs,Matrix4x4 rhs)
		{
			Matrix4x4 result;
			result.m00 = lhs.m00*rhs.m00+lhs.m01*rhs.m10+lhs.m02*rhs.m20+lhs.m03*rhs.m30;
			result.m01 = lhs.m00*rhs.m01+lhs.m01*rhs.m11+lhs.m02*rhs.m21+lhs.m03*rhs.m31;
			result.m02 = lhs.m00*rhs.m02+lhs.m01*rhs.m12+lhs.m02*rhs.m22+lhs.m03*rhs.m32;
			result.m03 = lhs.m00*rhs.m03+lhs.m01*rhs.m13+lhs.m02*rhs.m23+lhs.m03*rhs.m33;
			result.m10 = lhs.m10*rhs.m00+lhs.m11*rhs.m10+lhs.m12*rhs.m20+lhs.m13*rhs.m30;
			result.m11 = lhs.m10*rhs.m01+lhs.m11*rhs.m11+lhs.m12*rhs.m21+lhs.m13*rhs.m31;
			result.m12 = lhs.m10*rhs.m02+lhs.m11*rhs.m12+lhs.m12*rhs.m22+lhs.m13*rhs.m32;
			result.m13 = lhs.m10*rhs.m03+lhs.m11*rhs.m13+lhs.m12*rhs.m23+lhs.m13*rhs.m33;
			result.m20 = lhs.m20*rhs.m00+lhs.m21*rhs.m10+lhs.m22*rhs.m20+lhs.m23*rhs.m30;
			result.m21 = lhs.m20*rhs.m01+lhs.m21*rhs.m11+lhs.m22*rhs.m21+lhs.m23*rhs.m31;
			result.m22 = lhs.m20*rhs.m02+lhs.m21*rhs.m12+lhs.m22*rhs.m22+lhs.m23*rhs.m32;
			result.m23 = lhs.m20*rhs.m03+lhs.m21*rhs.m13+lhs.m22*rhs.m23+lhs.m23*rhs.m33;
			result.m30 = lhs.m30*rhs.m00+lhs.m31*rhs.m10+lhs.m32*rhs.m20+lhs.m33*rhs.m30;
			result.m31 = lhs.m30*rhs.m01+lhs.m31*rhs.m11+lhs.m32*rhs.m21+lhs.m33*rhs.m31;
			result.m32 = lhs.m30*rhs.m02+lhs.m31*rhs.m12+lhs.m32*rhs.m22+lhs.m33*rhs.m32;
			result.m33 = lhs.m30*rhs.m03+lhs.m31*rhs.m13+lhs.m32*rhs.m23+lhs.m33*rhs.m33;
			return result;
		}
		public static Vector2 operator *(Vector2 vec,Matrix4x4 matrix) => new Vector2(vec.x*matrix.m00+vec.y*matrix.m10+matrix.m30,vec.x*matrix.m01+vec.y*matrix.m11+matrix.m31);
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
		public static bool operator ==(Matrix4x4 left,Matrix4x4 right) => left.Equals(right);
		public static bool operator !=(Matrix4x4 left,Matrix4x4 right) => !left.Equals(right);

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
