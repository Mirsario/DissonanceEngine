namespace Dissonance.Engine
{
	public struct Vector2UShort
	{
		public ushort X;
		public ushort Y;

		public Vector2UShort(ushort x, ushort y)
		{
			X = x;
			Y = y;
		}

		public Vector2UShort(ushort xy) : this(xy, xy) { }

		public override string ToString()
			=> $"[{X}, {Y}]";

		public override int GetHashCode()
			=> X.GetHashCode() ^ Y.GetHashCode() << 2;

		public override bool Equals(object other)
			=> other is Vector2UShort point && X == point.X && Y == point.Y;

		// Operations

		// Int
		public static Vector2UShort operator *(Vector2UShort a, int d) => new((ushort)(a.X * d), (ushort)(a.Y * d));

		public static Vector2UShort operator *(int d, Vector2UShort a) => new((ushort)(a.X * d), (ushort)(a.Y * d));

		public static Vector2UShort operator /(Vector2UShort a, int d) => new((ushort)(a.X / d), (ushort)(a.Y / d));

		// UShort

		public static Vector2UShort operator *(Vector2UShort a, ushort d) => new((ushort)(a.X * d), (ushort)(a.Y * d));

		public static Vector2UShort operator *(ushort d, Vector2UShort a) => new((ushort)(a.X * d), (ushort)(a.Y * d));

		public static Vector2UShort operator /(Vector2UShort a, ushort d) => new((ushort)(a.X / d), (ushort)(a.Y / d));

		// Vector2UShort

		public static bool operator ==(Vector2UShort a, Vector2UShort b) => a.X == b.X && a.Y == b.Y;

		public static bool operator !=(Vector2UShort a, Vector2UShort b) => a.X != b.X || a.Y != b.Y;

		public static Vector2UShort operator +(Vector2UShort a, Vector2UShort b) => new((ushort)(a.X + b.X), (ushort)(a.Y + b.Y));

		public static Vector2UShort operator -(Vector2UShort a, Vector2UShort b) => new((ushort)(a.X - b.X), (ushort)(a.Y - b.Y));

		public static Vector2UShort operator *(Vector2UShort a, Vector2UShort b) => new((ushort)(a.X * b.X), (ushort)(a.Y * b.Y));

		public static Vector2UShort operator /(Vector2UShort a, Vector2UShort b) => new((ushort)(a.X / b.X), (ushort)(a.Y / b.Y));

		// Float

		public static Vector2 operator *(Vector2UShort a, float d) => new(a.X * d, a.Y * d);

		public static Vector2 operator *(float d, Vector2UShort a) => new(d * a.X, d * a.Y);

		public static Vector2 operator /(Vector2UShort a, float d) => new(a.X / d, a.Y / d);

		// Casts

		// Vector2
		public static explicit operator Vector2UShort(Vector2 value) => new((ushort)value.X, (ushort)value.Y);
		// Vector2Int
		public static explicit operator Vector2UShort(Vector2Int value) => new((ushort)value.X, (ushort)value.Y);
	}
}

