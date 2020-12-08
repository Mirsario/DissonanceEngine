namespace Dissonance.Engine
{
	public struct RectOffset
	{
		public float left;
		public float right;
		public float top;
		public float bottom;

		public RectOffset(float Left, float Top, float Right, float Bottom)
		{
			left = Left;
			top = Top;
			right = Right;
			bottom = Bottom;
		}

		public override int GetHashCode() => left.GetHashCode() ^ right.GetHashCode() << 2 ^ top.GetHashCode() >> 2 ^ bottom.GetHashCode() >> 1;
		public override bool Equals(object other) => other is RectOffset otherRect && left == otherRect.left && right == otherRect.right && top == otherRect.top && bottom == otherRect.bottom;

		public static bool operator ==(RectOffset a, RectOffset b) => a.Equals(b);
		public static bool operator !=(RectOffset a, RectOffset b) => !a.Equals(b);
	}
}
