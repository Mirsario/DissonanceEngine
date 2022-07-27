using System.Runtime.InteropServices;

namespace Dissonance.Engine;

public struct Vector4Int
{
	public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Vector4Int));
	public static readonly Vector4Int Zero = default;
	public static readonly Vector4Int One = new(1, 1, 1, 1);
	public static readonly Vector4Int UnitX = new(1, 0, 0, 0);
	public static readonly Vector4Int UnitY = new(0, 1, 0, 0);
	public static readonly Vector4Int UnitZ = new(0, 0, 1, 0);
	public static readonly Vector4Int UnitW = new(0, 0, 0, 1);

	public int X;
	public int Y;
	public int Z;
	public int W;

	public Vector2Int XY => new(X, Y);
	public Vector3Int XYZ => new(X, Y, Z);

	public Vector4Int(int x, int y, int z, int w)
	{
		X = x;
		Y = y;
		Z = z;
		W = w;
	}

	public Vector4Int(int xyzw) : this(xyzw, xyzw, xyzw, xyzw) { }

	public Vector4Int(Vector3Int xyz, int w) : this(xyz.X, xyz.Y, xyz.Z, w) { }

	public Vector4Int(Vector2Int xy, int z, int w) : this(xy.X, xy.Y, z, w) { }

	public Vector4Int(Vector2Int xy, Vector2Int zw) : this(xy.X, xy.Y, zw.X, zw.Y) { }

	public override string ToString() => $"[{X}, {Y}, {Z}, {W}]";

	public override int GetHashCode()
		=> X.GetHashCode() ^ Y.GetHashCode() << 2 ^ Z.GetHashCode() >> 2 ^ W.GetHashCode() >> 1;

	public override bool Equals(object other)
		=> other is Vector4Int point && X == point.X && Y == point.Y && Z == point.Z && W == point.W;

	// Vector4Int

	public static Vector4Int operator +(Vector4Int a, Vector4Int b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);

	public static Vector4Int operator -(Vector4Int a, Vector4Int b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);

	public static Vector4Int operator *(Vector4Int a, Vector4Int b) => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W);

	public static Vector4Int operator /(Vector4Int a, Vector4Int b) => new(a.X / b.X, a.Y / b.Y, a.Z / b.Z, a.W / b.W);

	public static Vector4Int operator -(Vector4Int a) => new(-a.X, -a.Y, -a.Z, -a.W);

	public static bool operator ==(Vector4Int a, Vector4Int b) => a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W;

	public static bool operator !=(Vector4Int a, Vector4Int b) => a.X != b.X || a.Y != b.Y || a.Z != b.Z || a.W != b.W;

	// int

	public static Vector4Int operator *(Vector4Int a, int d) => new(a.X * d, a.Y * d, a.Z * d, a.W * d);

	public static Vector4Int operator *(int d, Vector4Int a) => new(a.X * d, a.Y * d, a.Z * d, a.W * d);

	public static Vector4Int operator /(Vector4Int a, int d) => new(a.X / d, a.Y / d, a.Z / d, a.W / d);

	// float

	public static Vector4 operator *(Vector4Int a, float d) => new(a.X * d, a.Y * d, a.Z * d, a.W * d);

	public static Vector4 operator *(float d, Vector4Int a) => new(d * a.X, d * a.Y, d * a.Z, d * a.W);

	public static Vector4 operator /(Vector4Int a, float d) => new(a.X / d, a.Y / d, a.Z / d, a.W / d);

	public static unsafe implicit operator int*(Vector4Int vec) => (int*)&vec;
}

