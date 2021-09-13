using System.Runtime.InteropServices;

namespace Dissonance.Engine
{
	public struct Vector3Int
	{
		public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Vector3Int));
		public static readonly Vector3Int Zero = default;
		public static readonly Vector3Int One = new(1, 1, 1);
		public static readonly Vector3Int UnitX = new(1, 0, 0);
		public static readonly Vector3Int UnitY = new(0, 1, 0);
		public static readonly Vector3Int UnitZ = new(0, 0, 1);
		public static readonly Vector3Int Up = new(0, 1, 0);
		public static readonly Vector3Int Down = new(0, -1, 0);
		public static readonly Vector3Int Left = new(-1, 0, 0);
		public static readonly Vector3Int Right = new(1, 0, 0);
		public static readonly Vector3Int Forward = new(0, 0, 1);
		public static readonly Vector3Int Backward = new(0, 0, -1);

		public int x;
		public int y;
		public int z;

		public Vector3Int(int X, int Y, int Z)
		{
			x = X;
			y = Y;
			z = Z;
		}

		public Vector3Int(Vector2Int XY, int Z) : this(XY.x, XY.y, Z) { }

		public Vector3Int(int XYZ) : this(XYZ, XYZ, XYZ) { }

		public Vector2Int XY {
			get => new(x, y);
			set {
				x = value.x;
				y = value.y;
			}
		}
		public Vector2Int XZ {
			get => new(x, z);
			set {
				x = value.x;
				z = value.y;
			}
		}
		public Vector2Int YZ {
			get => new(y, z);
			set {
				y = value.x;
				z = value.y;
			}
		}

		public override string ToString()
			=> $"[{x}, {y}, {z}]";

		public override int GetHashCode()
			=> x.GetHashCode() ^ y.GetHashCode() << 2 ^ z.GetHashCode() >> 2;

		public override bool Equals(object other)
			=> other is Vector3Int vector && x == vector.x && y == vector.y && z == vector.z;

		public static explicit operator Vector3(Vector3Int value) => new(value.x, value.y, value.z);

		public static explicit operator Vector3Int(Vector3 value) => new((int)value.x, (int)value.y, (int)value.z);

		public static Vector3Int operator +(Vector3Int a, Vector3Int b) => new(a.x + b.x, a.y + b.y, a.z + b.z);

		public static Vector3Int operator -(Vector3Int a, Vector3Int b) => new(a.x - b.x, a.y - b.y, a.z + b.z);

		public static Vector3Int operator -(Vector3Int a) => new(-a.x, -a.y, -a.z);

		public static Vector3Int operator *(Vector3Int a, int d) => new(a.x * d, a.y * d, a.z * d);

		public static Vector3Int operator *(int d, Vector3Int a) => new(a.x * d, a.y * d, a.z * d);

		public static Vector3Int operator /(Vector3Int a, int d) => new(a.x / d, a.y / d, a.z / d);

		public static Vector3 operator *(Vector3Int a, float d) => new(a.x * d, a.y * d, a.z * d);

		public static Vector3 operator /(Vector3Int a, float d) => new(a.x / d, a.y / d, a.z / d);

		public static bool operator ==(Vector3Int a, Vector3Int b) => a.x == b.x && a.y == b.y && a.z == b.z;

		public static bool operator !=(Vector3Int a, Vector3Int b) => a.x != b.x || a.y != b.y || a.z != b.z;

		public static unsafe implicit operator int*(Vector3Int vec) => (int*)&vec;
	}
}

