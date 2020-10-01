using System;
using Dissonance.Engine.Core;

namespace Dissonance.Engine.Structures
{
	public partial struct Matrix4x4
	{
		public const int Length = 16;

		private const string OutOfRangeMessage = "[{0},{1}] is not a valid matrix index.";

		public static readonly Matrix4x4 Identity = new Matrix4x4(Vector4.UnitX, Vector4.UnitY, Vector4.UnitZ, Vector4.UnitW);
		public static readonly Matrix4x4 Zero = new Matrix4x4(Vector4.Zero, Vector4.Zero, Vector4.Zero, Vector4.Zero);

		public float m00, m01, m02, m03, m10, m11, m12, m13, m20, m21, m22, m23, m30, m31, m32, m33;

		public float Determinant =>
			m00 * m11 * m22 * m33 - m00 * m11 * m23 * m32 + m00 * m12 * m23 * m31 - m00 * m12 * m21 * m33 +
			m00 * m13 * m21 * m32 - m00 * m13 * m22 * m31 - m01 * m12 * m23 * m30 + m01 * m12 * m20 * m33 -
			m01 * m13 * m20 * m32 + m01 * m13 * m22 * m30 - m01 * m10 * m22 * m33 + m01 * m10 * m23 * m32 +
			m02 * m13 * m20 * m31 - m02 * m13 * m21 * m30 + m02 * m10 * m21 * m33 - m02 * m10 * m23 * m31 +
			m02 * m11 * m23 * m30 - m02 * m11 * m20 * m33 - m03 * m10 * m21 * m32 + m03 * m10 * m22 * m31 -
			m03 * m11 * m22 * m30 + m03 * m11 * m20 * m32 - m03 * m12 * m20 * m31 + m03 * m12 * m21 * m30;

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
			get => new Vector4(m00, m01, m02, m03);
			set {
				m00 = value.x;
				m01 = value.y;
				m02 = value.z;
				m03 = value.w;
			}
		}
		public Vector4 Row1 {
			get => new Vector4(m10, m11, m12, m13);
			set {
				m10 = value.x;
				m11 = value.y;
				m12 = value.z;
				m13 = value.w;
			}
		}
		public Vector4 Row2 {
			get => new Vector4(m20, m21, m22, m23);
			set {
				m20 = value.x;
				m21 = value.y;
				m22 = value.z;
				m23 = value.w;
			}
		}
		public Vector4 Row3 {
			get => new Vector4(m30, m31, m32, m33);
			set {
				m30 = value.x;
				m31 = value.y;
				m32 = value.z;
				m33 = value.w;
			}
		}

		public float this[int row, int column] {
			get => column switch
			{
				0 => row switch
				{
					0 => m00,
					1 => m01,
					2 => m02,
					3 => m03,
					_ => throw new IndexOutOfRangeException(string.Format(OutOfRangeMessage, row, column)),
				},

				1 => row switch
				{
					0 => m10,
					1 => m11,
					2 => m12,
					3 => m13,
					_ => throw new IndexOutOfRangeException(string.Format(OutOfRangeMessage, row, column)),
				},

				2 => row switch
				{
					0 => m20,
					1 => m21,
					2 => m22,
					3 => m23,
					_ => throw new IndexOutOfRangeException(string.Format(OutOfRangeMessage, row, column)),
				},

				3 => row switch
				{
					0 => m30,
					1 => m31,
					2 => m32,
					3 => m33,
					_ => throw new IndexOutOfRangeException(string.Format(OutOfRangeMessage, row, column)),
				},

				_ => throw new IndexOutOfRangeException(string.Format(OutOfRangeMessage, row, column)),
			};
			set {
				switch(column) {
					case 0:
						switch(row) {
							case 0:
								m00 = value;
								return;
							case 1:
								m01 = value;
								return;
							case 2:
								m02 = value;
								return;
							case 3:
								m03 = value;
								return;
							default:
								throw new IndexOutOfRangeException(string.Format(OutOfRangeMessage, row, column));
						}

					case 1:
						switch(row) {
							case 0:
								m10 = value;
								return;
							case 1:
								m11 = value;
								return;
							case 2:
								m12 = value;
								return;
							case 3:
								m13 = value;
								return;
							default:
								throw new IndexOutOfRangeException(string.Format(OutOfRangeMessage, row, column));
						}

					case 2:
						switch(row) {
							case 0:
								m20 = value;
								return;
							case 1:
								m21 = value;
								return;
							case 2:
								m22 = value;
								return;
							case 3:
								m23 = value;
								return;
							default:
								throw new IndexOutOfRangeException(string.Format(OutOfRangeMessage, row, column));
						}
					case 3:
						switch(row) {
							case 0:
								m30 = value;
								return;
							case 1:
								m31 = value;
								return;
							case 2:
								m32 = value;
								return;
							case 3:
								m33 = value;
								return;
							default:
								throw new IndexOutOfRangeException(string.Format(OutOfRangeMessage, row, column));
						}

					default:
						throw new IndexOutOfRangeException(string.Format(OutOfRangeMessage, row, column));
				}
			}
		}
		public float this[int id] {
			get => id switch
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
				_ => throw new IndexOutOfRangeException($"[{id}] single matrix index must be range of [0..15]."),
			};
			set {
				switch(id) {
					case 0:
						m00 = value;
						break;
					case 1:
						m01 = value;
						break;
					case 2:
						m02 = value;
						break;
					case 3:
						m03 = value;
						break;
					case 4:
						m10 = value;
						break;
					case 5:
						m11 = value;
						break;
					case 6:
						m12 = value;
						break;
					case 7:
						m13 = value;
						break;
					case 8:
						m20 = value;
						break;
					case 9:
						m21 = value;
						break;
					case 10:
						m22 = value;
						break;
					case 11:
						m23 = value;
						break;
					case 12:
						m30 = value;
						break;
					case 13:
						m31 = value;
						break;
					case 14:
						m32 = value;
						break;
					case 15:
						m33 = value;
						break;
					default:
						throw new IndexOutOfRangeException($"[{id}] single matrix index must be in [0..15] range.");
				}
			}
		}

		public Matrix4x4(float m00, float m01, float m02, float m03, float m10, float m11, float m12, float m13, float m20, float m21, float m22, float m23, float m30, float m31, float m32, float m33)
		{
			this.m00 = m00;
			this.m01 = m01;
			this.m02 = m02;
			this.m03 = m03;
			this.m10 = m10;
			this.m11 = m11;
			this.m12 = m12;
			this.m13 = m13;
			this.m20 = m20;
			this.m21 = m21;
			this.m22 = m22;
			this.m23 = m23;
			this.m30 = m30;
			this.m31 = m31;
			this.m32 = m32;
			this.m33 = m33;
		}
		public Matrix4x4(Vector4 row0, Vector4 row1, Vector4 row2, Vector4 row3)
		{
			m00 = row0.x;
			m01 = row0.y;
			m02 = row0.z;
			m03 = row0.w;
			m10 = row1.x;
			m11 = row1.y;
			m12 = row1.z;
			m13 = row1.w;
			m20 = row2.x;
			m21 = row2.y;
			m22 = row2.z;
			m23 = row2.w;
			m30 = row3.x;
			m31 = row3.y;
			m32 = row3.z;
			m33 = row3.w;
		}

		public override string ToString() => $"[{m00}, {m01}, {m02}, {m03},\n {m10}, {m11}, {m12}, {m13},\n {m20}, {m21}, {m22}, {m23},\n {m30}, {m31}, {m32}, {m33}]";
		public override int GetHashCode() => Row0.GetHashCode() ^ Row1.GetHashCode() << 2 ^ Row2.GetHashCode() >> 2 ^ Row3.GetHashCode() >> 1;
		public override bool Equals(object other) => other is Matrix4x4 matrix && Equals(matrix);

		//Clear
		public void ClearTranslation()
		{
			m30 = 0f;
			m31 = 0f;
			m32 = 0f;
		}
		public void ClearScale()
		{
			Row0 = new Vector4(((Vector3)Row0).Normalized, m03);
			Row1 = new Vector4(((Vector3)Row1).Normalized, m13);
			Row2 = new Vector4(((Vector3)Row2).Normalized, m23);
		}
		public void ClearRotation()
		{
			float mag0 = ((Vector3)Row0).Magnitude;
			float mag1 = ((Vector3)Row1).Magnitude;
			float mag2 = ((Vector3)Row2).Magnitude;

			Row0 = new Vector4(mag0, 0f, 0f, m03);
			Row1 = new Vector4(0f, mag1, 0f, m13);
			Row2 = new Vector4(0f, 0f, mag2, m23);
		}
		//Extract
		public Vector3 ExtractTranslation() => new Vector3(m30, m31, m32);
		public Vector3 ExtractScale()
		{
			Vector3 result;

			result.x = Mathf.Sqrt(m00 * m00 + m01 * m01 + m02 * m02);
			result.y = Mathf.Sqrt(m10 * m10 + m11 * m11 + m12 * m12);
			result.z = Mathf.Sqrt(m20 * m20 + m21 * m21 + m22 * m22);

			return result;
		}
		public Vector3 ExtractEuler()
		{
			float v1, v2, v3;

			if(m21 < -1f) { //up
				v1 = -Mathf.HalfPI;
				v2 = 0f;
				v3 = Mathf.Atan2(m02, m00);
			} else if(m21 > 1f) { //down
				v1 = Mathf.HalfPI;
				v2 = 0f;
				v3 = -Mathf.Atan2(m02, m00);
			} else {
				v1 = Mathf.Asin(m21);
				v2 = Mathf.Atan2(-m20, m22);
				v3 = Mathf.Atan2(-m01, m11);
			}

			return new Vector3(-v1, -v2, v3) * Mathf.Rad2Deg;
		}
		public float ExtractEulerX()
		{
			if(m21 < -1f) {
				return -Mathf.HalfPI;
			} else if(m21 > 1f) {
				return Mathf.HalfPI;
			}
			
			return Mathf.Asin(m21);
		}
		public float ExtractEulerY()
		{
			if(m21 < -1f || m21 > 1f) {
				return 0f;
			}

			return Mathf.Atan2(-m20, m22);
		}
		public float ExtractEulerZ()
		{
			if(m21 < -1f) {
				return Mathf.Atan2(m02, m00);
			} else if(m21 > 1f) {
				return -Mathf.Atan2(m02, m00);
			}

			return Mathf.Atan2(-m01, m11);
		}
		public Quaternion ExtractQuaternion()
		{
			var row0 = Row0.XYZ.Normalized;
			var row1 = Row1.XYZ.Normalized;
			var row2 = Row2.XYZ.Normalized;

			var q = new Quaternion();
			float trace = 0.25f * (row0.x + row1.y + row2.z + 1f);

			if(trace > 0) {
				float sq = Mathf.Sqrt(trace);

				q.w = sq;
				sq = 1f / (4f * sq);
				q.x = (row1.z - row2.y) * sq;
				q.y = (row2.x - row0.z) * sq;
				q.z = (row0.y - row1.x) * sq;
			} else if(row0.x > row1.y && row0.x > row2.z) {
				float sq = 2f * Mathf.Sqrt(1f + row0.x - row1.y - row2.z);

				q.x = 0.25f * sq;
				sq = 1f / sq;
				q.w = (row2.y - row1.z) * sq;
				q.y = (row1.x + row0.y) * sq;
				q.z = (row2.x + row0.z) * sq;
			} else if(row1.y > row2.z) {
				float sq = 2f * Mathf.Sqrt(1f + row1.y - row0.x - row2.z);

				q.y = 0.25f * sq;
				sq = 1f / sq;
				q.w = (row2.x - row0.z) * sq;
				q.x = (row1.x + row0.y) * sq;
				q.z = (row2.y + row1.z) * sq;
			} else {
				float sq = 2f * Mathf.Sqrt(1f + row2.z - row0.x - row1.y);

				q.z = 0.25f * sq;
				sq = 1f / sq;
				q.w = (row1.x - row0.y) * sq;
				q.x = (row2.x + row0.z) * sq;
				q.y = (row2.y + row1.z) * sq;
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
		public void SetRotation(Quaternion q)
			=> SetRotationAndScale(q, ExtractScale());
		public void SetRotationAndScale(Quaternion q, Vector3 scale)
		{
			float x = q.x * 2f;
			float y = q.y * 2f;
			float z = q.z * 2f;

			float xx = q.x * x;
			float yy = q.y * y;
			float zz = q.z * z;

			float xy = q.x * y;
			float xz = q.x * z;
			float yz = q.y * z;
			float wx = q.w * x;
			float wy = q.w * y;
			float wz = q.w * z;

			m00 = (1f - (yy + zz)) * scale.x;
			m01 = (xy + wz) * scale.x;
			m02 = (xz - wy) * scale.x;
			m10 = (xy - wz) * scale.y;
			m11 = (1f - (xx + zz)) * scale.y;
			m12 = (yz + wx) * scale.y;
			m20 = (xz + wy) * scale.z;
			m21 = (yz - wx) * scale.z;
			m22 = (1f - (xx + yy)) * scale.z;
		}
		//Etc
		public bool Equals(Matrix4x4 o)
			=> m00 == o.m00 && m01 == o.m01 && m02 == o.m02 && m03 == o.m03
			&& m10 == o.m10 && m11 == o.m11 && m12 == o.m12 && m13 == o.m13
			&& m20 == o.m20 && m21 == o.m21 && m22 == o.m22 && m23 == o.m23;
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

			int[] columnIndices = { 0, 0, 0, 0 };
			int[] rowIndices = { 0, 0, 0, 0 };
			int[] pivotIndices = { -1, -1, -1, -1 };

			float[,] inverse =  {
				{ m00,m01,m02,m03 },
				{ m10,m11,m12,m13 },
				{ m20,m21,m22,m23 },
				{ m30,m31,m32,m33 }
			};

			int iColumn = 0;
			int iRow = 0;

			for(int i = 0; i < 4; i++) {
				float maxPivot = 0f;

				for(int j = 0; j < 4; j++) {
					if(pivotIndices[j] == 0) {
						continue;
					}

					for(int k = 0; k < 4; ++k) {
						if(pivotIndices[k] == -1) {
							float absVal = Math.Abs(inverse[j, k]);

							if(absVal > maxPivot) {
								maxPivot = absVal;
								iRow = j;
								iColumn = k;
							}
						} else if(pivotIndices[k] > 0) {
							return;
						}
					}
				}

				pivotIndices[iColumn]++;

				if(iRow != iColumn) {
					for(int k = 0; k < 4; ++k) {
						float f = inverse[iRow, k];

						inverse[iRow, k] = inverse[iColumn, k];
						inverse[iColumn, k] = f;
					}
				}

				rowIndices[i] = iRow;
				columnIndices[i] = iColumn;

				float pivot = inverse[iColumn, iColumn];
				if(pivot == 0f) {
					return; //Matrix is singular and cannot be inverted.
				}

				float oneOverPivot = 1f / pivot;

				inverse[iColumn, iColumn] = 1f;

				for(int k = 0; k < 4; ++k) {
					inverse[iColumn, k] *= oneOverPivot;
				}

				for(int j = 0; j < 4; ++j) {
					if(iColumn == j) {
						continue;
					}

					float f = inverse[j, iColumn];

					inverse[j, iColumn] = 0f;

					for(int k = 0; k < 4; ++k) {
						inverse[j, k] -= inverse[iColumn, k] * f;
					}
				}
			}

			for(int j = 3; j >= 0; --j) {
				int ir = rowIndices[j];
				int ic = columnIndices[j];

				for(int k = 0; k < 4; ++k) {
					float f = inverse[k, ir];

					inverse[k, ir] = inverse[k, ic];
					inverse[k, ic] = f;
				}
			}

			m00 = inverse[0, 0];
			m01 = inverse[0, 1];
			m02 = inverse[0, 2];
			m03 = inverse[0, 3];
			m10 = inverse[1, 0];
			m11 = inverse[1, 1];
			m12 = inverse[1, 2];
			m13 = inverse[1, 3];
			m20 = inverse[2, 0];
			m21 = inverse[2, 1];
			m22 = inverse[2, 2];
			m23 = inverse[2, 3];
			m30 = inverse[3, 0];
			m31 = inverse[3, 1];
			m32 = inverse[3, 2];
			m33 = inverse[3, 3];
		}
	}
}
