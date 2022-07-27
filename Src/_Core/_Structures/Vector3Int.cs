using System.Runtime.InteropServices;

namespace Dissonance.Engine;

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

	public int X;
	public int Y;
	public int Z;

	public Vector3Int(int x, int y, int z)
	{
		X = x;
		Y = y;
		Z = z;
	}

	public Vector3Int(int xyz) : this(xyz, xyz, xyz) { }

	public Vector3Int(Vector2Int xy, int z) : this(xy.X, xy.Y, z) { }

	public Vector2Int XY {
		get => new(X, Y);
		set {
			X = value.X;
			Y = value.Y;
		}
	}
	public Vector2Int XZ {
		get => new(X, Z);
		set {
			X = value.X;
			Z = value.Y;
		}
	}
	public Vector2Int YZ {
		get => new(Y, Z);
		set {
			Y = value.X;
			Z = value.Y;
		}
	}

	public override string ToString()
		=> $"[{X}, {Y}, {Z}]";

	public override int GetHashCode()
		=> X.GetHashCode() ^ Y.GetHashCode() << 2 ^ Z.GetHashCode() >> 2;

	public override bool Equals(object other)
		=> other is Vector3Int vector && X == vector.X && Y == vector.Y && Z == vector.Z;

	// Vector3

	public static explicit operator Vector3(Vector3Int value) => new(value.X, value.Y, value.Z);

	public static explicit operator Vector3Int(Vector3 value) => new((int)value.X, (int)value.Y, (int)value.Z);

	// Vector3Int

	public static Vector3Int operator -(Vector3Int a) => new(-a.X, -a.Y, -a.Z);

	public static Vector3Int operator -(Vector3Int a, Vector3Int b) => new(a.X - b.X, a.Y - b.Y, a.Z + b.Z);

	public static Vector3Int operator +(Vector3Int a, Vector3Int b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

	public static Vector3Int operator *(Vector3Int a, Vector3Int b) => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z);

	public static Vector3Int operator /(Vector3Int a, Vector3Int b) => new(a.X / b.X, a.Y / b.Y, a.Z / b.Z);

	public static bool operator ==(Vector3Int a, Vector3Int b) => a.X == b.X && a.Y == b.Y && a.Z == b.Z;

	public static bool operator !=(Vector3Int a, Vector3Int b) => a.X != b.X || a.Y != b.Y || a.Z != b.Z;

	// int

	public static Vector3Int operator *(Vector3Int a, int d) => new(a.X * d, a.Y * d, a.Z * d);

	public static Vector3Int operator *(int d, Vector3Int a) => new(a.X * d, a.Y * d, a.Z * d);
	
	public static Vector3Int operator /(Vector3Int a, int d) => new(a.X / d, a.Y / d, a.Z / d);

	// float

	public static Vector3 operator *(Vector3Int a, float d) => new(a.X * d, a.Y * d, a.Z * d);

	public static Vector3 operator /(Vector3Int a, float d) => new(a.X / d, a.Y / d, a.Z / d);

	// int*

	public static unsafe implicit operator int*(Vector3Int vec) => (int*)&vec;
}

