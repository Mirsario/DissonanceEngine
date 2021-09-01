namespace Dissonance.Engine
{
	public struct Vector2UShort
	{
		public ushort x;
		public ushort y;

		public Vector2UShort(ushort x, ushort y)
		{
			this.x = x;
			this.y = y;
		}

		public override string ToString()
			=> $"[{x}, {y}]";

		public override int GetHashCode()
			=> x.GetHashCode() ^ y.GetHashCode() << 2;

		public override bool Equals(object other)
			=> other is Vector2UShort point && x == point.x && y == point.y;

		// Operations

		// Int
		public static Vector2UShort operator *(Vector2UShort a, int d) => new Vector2UShort((ushort)(a.x * d), (ushort)(a.y * d));

		public static Vector2UShort operator *(int d, Vector2UShort a) => new Vector2UShort((ushort)(a.x * d), (ushort)(a.y * d));

		public static Vector2UShort operator /(Vector2UShort a, int d) => new Vector2UShort((ushort)(a.x / d), (ushort)(a.y / d));

		// UShort

		public static Vector2UShort operator *(Vector2UShort a, ushort d) => new Vector2UShort((ushort)(a.x * d), (ushort)(a.y * d));

		public static Vector2UShort operator *(ushort d, Vector2UShort a) => new Vector2UShort((ushort)(a.x * d), (ushort)(a.y * d));

		public static Vector2UShort operator /(Vector2UShort a, ushort d) => new Vector2UShort((ushort)(a.x / d), (ushort)(a.y / d));

		// Vector2UShort

		public static bool operator ==(Vector2UShort a, Vector2UShort b) => a.x == b.x && a.y == b.y;

		public static bool operator !=(Vector2UShort a, Vector2UShort b) => a.x != b.x || a.y != b.y;

		public static Vector2UShort operator +(Vector2UShort a, Vector2UShort b) => new((ushort)(a.x + b.x), (ushort)(a.y + b.y));

		public static Vector2UShort operator -(Vector2UShort a, Vector2UShort b) => new((ushort)(a.x - b.x), (ushort)(a.y - b.y));

		public static Vector2UShort operator *(Vector2UShort a, Vector2UShort b) => new((ushort)(a.x * b.x), (ushort)(a.y * b.y));

		public static Vector2UShort operator /(Vector2UShort a, Vector2UShort b) => new((ushort)(a.x / b.x), (ushort)(a.y / b.y));

		// Float

		public static Vector2 operator *(Vector2UShort a, float d) => new(a.x * d, a.y * d);

		public static Vector2 operator *(float d, Vector2UShort a) => new(d * a.x, d * a.y);

		public static Vector2 operator /(Vector2UShort a, float d) => new(a.x / d, a.y / d);

		// Casts

		// Vector2
		public static explicit operator Vector2UShort(Vector2 value) => new((ushort)value.x, (ushort)value.y);
		// Vector2Int
		public static explicit operator Vector2UShort(Vector2Int value) => new((ushort)value.x, (ushort)value.y);
	}
}

