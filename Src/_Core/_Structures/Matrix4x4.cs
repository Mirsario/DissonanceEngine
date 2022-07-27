using System;

namespace Dissonance.Engine;

public partial struct Matrix4x4
{
	public const int Length = 16;

	private const string OutOfRangeMessage = "[{0},{1}] is not a valid matrix index.";

	public static readonly Matrix4x4 Identity = new(Vector4.UnitX, Vector4.UnitY, Vector4.UnitZ, Vector4.UnitW);
	public static readonly Matrix4x4 Zero = new(Vector4.Zero, Vector4.Zero, Vector4.Zero, Vector4.Zero);

	public float m00, m01, m02, m03, m10, m11, m12, m13, m20, m21, m22, m23, m30, m31, m32, m33;

	public float Determinant =>
		m00 * m11 * m22 * m33 - m00 * m11 * m23 * m32 + m00 * m12 * m23 * m31 - m00 * m12 * m21 * m33 +
		m00 * m13 * m21 * m32 - m00 * m13 * m22 * m31 - m01 * m12 * m23 * m30 + m01 * m12 * m20 * m33 -
		m01 * m13 * m20 * m32 + m01 * m13 * m22 * m30 - m01 * m10 * m22 * m33 + m01 * m10 * m23 * m32 +
		m02 * m13 * m20 * m31 - m02 * m13 * m21 * m30 + m02 * m10 * m21 * m33 - m02 * m10 * m23 * m31 +
		m02 * m11 * m23 * m30 - m02 * m11 * m20 * m33 - m03 * m10 * m21 * m32 + m03 * m10 * m22 * m31 -
		m03 * m11 * m22 * m30 + m03 * m11 * m20 * m32 - m03 * m12 * m20 * m31 + m03 * m12 * m21 * m30;

	public Matrix4x4 Transpose => throw new NotImplementedException(); // Matrix4x4.Transpose(this);
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
		get => new(m00, m01, m02, m03);
		set {
			m00 = value.X;
			m01 = value.Y;
			m02 = value.Z;
			m03 = value.W;
		}
	}

	public Vector4 Row1 {
		get => new(m10, m11, m12, m13);
		set {
			m10 = value.X;
			m11 = value.Y;
			m12 = value.Z;
			m13 = value.W;
		}
	}

	public Vector4 Row2 {
		get => new(m20, m21, m22, m23);
		set {
			m20 = value.X;
			m21 = value.Y;
			m22 = value.Z;
			m23 = value.W;
		}
	}

	public Vector4 Row3 {
		get => new(m30, m31, m32, m33);
		set {
			m30 = value.X;
			m31 = value.Y;
			m32 = value.Z;
			m33 = value.W;
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
			switch (column) {
				case 0:
					switch (row) {
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
					switch (row) {
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
					switch (row) {
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
					switch (row) {
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
			switch (id) {
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
		m00 = row0.X;
		m01 = row0.Y;
		m02 = row0.Z;
		m03 = row0.W;
		m10 = row1.X;
		m11 = row1.Y;
		m12 = row1.Z;
		m13 = row1.W;
		m20 = row2.X;
		m21 = row2.Y;
		m22 = row2.Z;
		m23 = row2.W;
		m30 = row3.X;
		m31 = row3.Y;
		m32 = row3.Z;
		m33 = row3.W;
	}

	public override string ToString() => $"[{m00}, {m01}, {m02}, {m03},\n {m10}, {m11}, {m12}, {m13},\n {m20}, {m21}, {m22}, {m23},\n {m30}, {m31}, {m32}, {m33}]";

	public override int GetHashCode() => Row0.GetHashCode() ^ Row1.GetHashCode() << 2 ^ Row2.GetHashCode() >> 2 ^ Row3.GetHashCode() >> 1;

	public override bool Equals(object other) => other is Matrix4x4 matrix && Equals(matrix);

	// Clear
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

	public void ClearScale2D()
	{
		Row0 = new Vector4(((Vector3)Row0).Normalized, m03);
		Row1 = new Vector4(((Vector3)Row1).Normalized, m13);
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

	// Extract
	public Vector3 ExtractTranslation() => new(m30, m31, m32);

	public Vector3 ExtractScale()
	{
		Vector3 result;

		result.X = MathF.Sqrt(m00 * m00 + m01 * m01 + m02 * m02);
		result.Y = MathF.Sqrt(m10 * m10 + m11 * m11 + m12 * m12);
		result.Z = MathF.Sqrt(m20 * m20 + m21 * m21 + m22 * m22);

		return result;
	}

	public Vector2 ExtractScale2D()
	{
		Vector2 result;

		result.X = MathF.Sqrt(m00 * m00 + m01 * m01 + m02 * m02);
		result.Y = MathF.Sqrt(m10 * m10 + m11 * m11 + m12 * m12);

		return result;
	}

	public Vector3 ExtractEuler()
	{
		float v1, v2, v3;

		if (m21 < -1f) { // up
			v1 = -MathHelper.HalfPI;
			v2 = 0f;
			v3 = MathF.Atan2(m02, m00);
		} else if (m21 > 1f) { // down
			v1 = MathHelper.HalfPI;
			v2 = 0f;
			v3 = -MathF.Atan2(m02, m00);
		} else {
			v1 = MathF.Asin(m21);
			v2 = MathF.Atan2(-m20, m22);
			v3 = MathF.Atan2(-m01, m11);
		}

		return new Vector3(-v1, -v2, v3) * MathHelper.Rad2Deg;
	}

	public float ExtractEulerX()
	{
		if (m21 < -1f) {
			return MathHelper.HalfPI * MathHelper.Rad2Deg;
		} else if (m21 > 1f) {
			return -MathHelper.HalfPI * MathHelper.Rad2Deg;
		}

		return -MathF.Asin(m21) * MathHelper.Rad2Deg;
	}

	public float ExtractEulerY()
	{
		if (m21 < -1f || m21 > 1f) {
			return 0f;
		}

		return -MathF.Atan2(-m20, m22) * MathHelper.Rad2Deg;
	}

	public float ExtractEulerZ()
	{
		if (m21 < -1f) {
			return -MathF.Atan2(m02, m00) * MathHelper.Rad2Deg;
		} else if (m21 > 1f) {
			return MathF.Atan2(m02, m00) * MathHelper.Rad2Deg;
		}

		return -MathF.Atan2(-m01, m11) * MathHelper.Rad2Deg;
	}

	public Quaternion ExtractQuaternion()
	{
		var row0 = Row0.XYZ.Normalized;
		var row1 = Row1.XYZ.Normalized;
		var row2 = Row2.XYZ.Normalized;

		var q = new Quaternion();
		float trace = 0.25f * (row0.X + row1.Y + row2.Z + 1f);

		if (trace > 0) {
			float sq = MathF.Sqrt(trace);

			q.W = sq;
			sq = 1f / (4f * sq);
			q.X = (row1.Z - row2.Y) * sq;
			q.Y = (row2.X - row0.Z) * sq;
			q.Z = (row0.Y - row1.X) * sq;
		} else if (row0.X > row1.Y && row0.X > row2.Z) {
			float sq = 2f * MathF.Sqrt(1f + row0.X - row1.Y - row2.Z);

			q.X = 0.25f * sq;
			sq = 1f / sq;
			q.W = (row2.Y - row1.Z) * sq;
			q.Y = (row1.X + row0.Y) * sq;
			q.Z = (row2.X + row0.Z) * sq;
		} else if (row1.Y > row2.Z) {
			float sq = 2f * MathF.Sqrt(1f + row1.Y - row0.X - row2.Z);

			q.Y = 0.25f * sq;
			sq = 1f / sq;
			q.W = (row2.X - row0.Z) * sq;
			q.X = (row1.X + row0.Y) * sq;
			q.Z = (row2.Y + row1.Z) * sq;
		} else {
			float sq = 2f * MathF.Sqrt(1f + row2.Z - row0.X - row1.Y);

			q.Z = 0.25f * sq;
			sq = 1f / sq;
			q.W = (row1.X - row0.Y) * sq;
			q.X = (row2.X + row0.Z) * sq;
			q.Y = (row2.Y + row1.Z) * sq;
		}

		q.Normalize();

		return q;
	}

	// Set
	public void SetTranslation(Vector3 translation)
	{
		m30 = translation.X;
		m31 = translation.Y;
		m32 = translation.Z;
	}

	public void SetScale(Vector3 scale)
	{
		ClearScale();

		m00 *= scale.X;
		m01 *= scale.X;
		m02 *= scale.X;

		m10 *= scale.Y;
		m11 *= scale.Y;
		m12 *= scale.Y;

		m20 *= scale.Z;
		m21 *= scale.Z;
		m22 *= scale.Z;
	}

	public void SetScale2D(Vector2 scale)
	{
		ClearScale2D();

		m00 *= scale.X;
		m01 *= scale.X;
		m02 *= scale.X;

		m10 *= scale.Y;
		m11 *= scale.Y;
		m12 *= scale.Y;
	}

	public void SetRotation(Quaternion q)
		=> SetRotationAndScale(q, ExtractScale());

	public void SetRotationAndScale(Quaternion q, Vector3 scale)
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

		m00 = (1f - (yy + zz)) * scale.X;
		m01 = (xy + wz) * scale.X;
		m02 = (xz - wy) * scale.X;
		m10 = (xy - wz) * scale.Y;
		m11 = (1f - (xx + zz)) * scale.Y;
		m12 = (yz + wx) * scale.Y;
		m20 = (xz + wy) * scale.Z;
		m21 = (yz - wx) * scale.Z;
		m22 = (1f - (xx + yy)) * scale.Z;
	}

	// Etc
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

		for (int i = 0; i < 4; i++) {
			float maxPivot = 0f;

			for (int j = 0; j < 4; j++) {
				if (pivotIndices[j] == 0) {
					continue;
				}

				for (int k = 0; k < 4; ++k) {
					if (pivotIndices[k] == -1) {
						float absVal = Math.Abs(inverse[j, k]);

						if (absVal > maxPivot) {
							maxPivot = absVal;
							iRow = j;
							iColumn = k;
						}
					} else if (pivotIndices[k] > 0) {
						return;
					}
				}
			}

			pivotIndices[iColumn]++;

			if (iRow != iColumn) {
				for (int k = 0; k < 4; ++k) {
					float f = inverse[iRow, k];

					inverse[iRow, k] = inverse[iColumn, k];
					inverse[iColumn, k] = f;
				}
			}

			rowIndices[i] = iRow;
			columnIndices[i] = iColumn;

			float pivot = inverse[iColumn, iColumn];
			if (pivot == 0f) {
				return; // Matrix is singular and cannot be inverted.
			}

			float oneOverPivot = 1f / pivot;

			inverse[iColumn, iColumn] = 1f;

			for (int k = 0; k < 4; ++k) {
				inverse[iColumn, k] *= oneOverPivot;
			}

			for (int j = 0; j < 4; ++j) {
				if (iColumn == j) {
					continue;
				}

				float f = inverse[j, iColumn];

				inverse[j, iColumn] = 0f;

				for (int k = 0; k < 4; ++k) {
					inverse[j, k] -= inverse[iColumn, k] * f;
				}
			}
		}

		for (int j = 3; j >= 0; --j) {
			int ir = rowIndices[j];
			int ic = columnIndices[j];

			for (int k = 0; k < 4; ++k) {
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
