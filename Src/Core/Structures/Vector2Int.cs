using System.Runtime.InteropServices;

namespace Dissonance.Engine
{
	public struct Vector2Int
	{
		public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Vector2Int));
		public static readonly Vector2Int Zero = default;
		public static readonly Vector2Int One = new(1, 1);
		public static readonly Vector2Int UnitX = new(1, 0);
		public static readonly Vector2Int UnitY = new(0, 1);
		public static readonly Vector2Int Up = new(0, 1);
		public static readonly Vector2Int Down = new(0, -1);
		public static readonly Vector2Int Left = new(-1, 0);
		public static readonly Vector2Int Right = new(1, 0);

		public int x;
		public int y;

		public Vector2Int(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public Vector2Int(int xy) : this(xy, xy) { }

		public override int GetHashCode()
			=> x ^ y << 2;

		public override bool Equals(object other)
			=> other is Vector2Int point && x == point.x && y == point.y;

		public override string ToString()
			=> $"[{x}, {y}]";

		// Operations

		// int

		public static Vector2Int operator *(Vector2Int a, int d) => new(a.x * d, a.y * d);

		public static Vector2Int operator *(int d, Vector2Int a) => new(a.x * d, a.y * d);

		public static Vector2Int operator /(Vector2Int a, int d) => new(a.x / d, a.y / d);

		// float

		public static Vector2 operator *(Vector2Int a, float d) => new(a.x * d, a.y * d);

		public static Vector2 operator *(float d, Vector2Int a) => new(d * a.x, d * a.y);

		public static Vector2 operator /(Vector2Int a, float d) => new(a.x / d, a.y / d);

		// Vector2Int

		public static Vector2Int operator +(Vector2Int a, Vector2Int b) => new(a.x + b.x, a.y + b.y);

		public static Vector2Int operator -(Vector2Int a, Vector2Int b) => new(a.x - b.x, a.y - b.y);

		public static Vector2Int operator *(Vector2Int a, Vector2Int b) => new(a.x * b.x, a.y * b.y);

		public static Vector2Int operator /(Vector2Int a, Vector2Int b) => new(a.x / b.x, a.y / b.y);

		public static Vector2Int operator -(Vector2Int a) => new(-a.x, -a.y);

		public static bool operator ==(Vector2Int a, Vector2Int b) => a.x == b.x && a.y == b.y;

		public static bool operator !=(Vector2Int a, Vector2Int b) => a.x != b.x || a.y != b.y;

		// Vector2

		public static bool operator ==(Vector2Int a, Vector2 b) => a.x == b.x && a.y == b.y;

		public static bool operator ==(Vector2 a, Vector2Int b) => a.x == b.x && a.y == b.y;

		public static bool operator !=(Vector2Int a, Vector2 b) => a.x != b.x || a.y != b.y;

		public static bool operator !=(Vector2 a, Vector2Int b) => a.x != b.x || a.y != b.y;

		// Vector2UShort

		public static Vector2Int operator +(Vector2Int a, Vector2UShort b) => new(a.x + b.x, a.y + b.y);

		public static Vector2Int operator +(Vector2UShort a, Vector2Int b) => new(a.x + b.x, a.y + b.y);

		public static Vector2Int operator -(Vector2Int a, Vector2UShort b) => new(a.x - b.x, a.y - b.y);

		public static Vector2Int operator -(Vector2UShort a, Vector2Int b) => new(a.x - b.x, a.y - b.y);

		public static Vector2Int operator *(Vector2Int a, Vector2UShort b) => new(a.x * b.x, a.y * b.y);

		public static Vector2Int operator *(Vector2UShort a, Vector2Int b) => new(a.x * b.x, a.y * b.y);

		public static Vector2Int operator /(Vector2Int a, Vector2UShort b) => new(a.x / b.x, a.y / b.y);

		public static Vector2Int operator /(Vector2UShort a, Vector2Int b) => new(a.x / b.x, a.y / b.y);

		// Casts

		// int*
		public static unsafe implicit operator int*(Vector2Int vec) => (int*)&vec;

		// System.Drawing.Point

		public static implicit operator System.Drawing.Point(Vector2Int value) => new(value.x, value.y);

		public static implicit operator Vector2Int(System.Drawing.Point value) => new(value.X, value.Y);

		// Vector2Int

		public static explicit operator Vector2Int(Vector2 value) => new((int)value.x, (int)value.y);

		// Vector2UShort

		public static explicit operator Vector2Int(Vector2UShort value) => new(value.x, value.y);
	}
}

