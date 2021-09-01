using System.Runtime.InteropServices;

namespace Dissonance.Engine
{
	public struct Vector4Int
	{
		public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Vector4Int));
		public static readonly Vector4Int Zero = default;
		public static readonly Vector4Int One = new(1, 1, 1, 1);
		public static readonly Vector4Int UnitX = new(1, 0, 0, 0);
		public static readonly Vector4Int UnitY = new(0, 1, 0, 0);
		public static readonly Vector4Int UnitZ = new(0, 0, 1, 0);
		public static readonly Vector4Int UnitW = new(0, 0, 0, 1);

		public int x;
		public int y;
		public int z;
		public int w;

		public Vector4Int(int x, int y, int z, int w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public override string ToString() => $"[{x}, {y}, {z}, {w}]";

		public override int GetHashCode()
			=> x.GetHashCode() ^ y.GetHashCode() << 2 ^ z.GetHashCode() >> 2 ^ w.GetHashCode() >> 1;

		public override bool Equals(object other)
			=> other is Vector4Int point && x == point.x && y == point.y && z == point.z && w == point.w;

		// Vector4Int

		public static Vector4Int operator +(Vector4Int a, Vector4Int b) => new(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);

		public static Vector4Int operator -(Vector4Int a, Vector4Int b) => new(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);

		public static Vector4Int operator *(Vector4Int a, Vector4Int b) => new(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);

		public static Vector4Int operator /(Vector4Int a, Vector4Int b) => new(a.x / b.x, a.y / b.y, a.z / b.z, a.w / b.w);

		public static Vector4Int operator -(Vector4Int a) => new(-a.x, -a.y, -a.z, -a.w);

		public static bool operator ==(Vector4Int a, Vector4Int b) => a.x == b.x && a.y == b.y && a.z == b.z && a.w == b.w;

		public static bool operator !=(Vector4Int a, Vector4Int b) => a.x != b.x || a.y != b.y || a.z != b.z || a.w != b.w;

		// int

		public static Vector4Int operator *(Vector4Int a, int d) => new(a.x * d, a.y * d, a.z * d, a.w * d);

		public static Vector4Int operator *(int d, Vector4Int a) => new(a.x * d, a.y * d, a.z * d, a.w * d);

		public static Vector4Int operator /(Vector4Int a, int d) => new(a.x / d, a.y / d, a.z / d, a.w / d);

		// float

		public static Vector4 operator *(Vector4Int a, float d) => new(a.x * d, a.y * d, a.z * d, a.w * d);

		public static Vector4 operator *(float d, Vector4Int a) => new(d * a.x, d * a.y, d * a.z, d * a.w);

		public static Vector4 operator /(Vector4Int a, float d) => new(a.x / d, a.y / d, a.z / d, a.w / d);

		public static unsafe implicit operator int*(Vector4Int vec) => (int*)&vec;
	}
}

