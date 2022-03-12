namespace Dissonance.Engine
{
	public struct RectOffset
	{
		public float Left;
		public float Right;
		public float Top;
		public float Bottom;

		public RectOffset(float left, float top, float right, float bottom)
		{
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
		}

		public override int GetHashCode()
			=> Left.GetHashCode() ^ Right.GetHashCode() << 2 ^ Top.GetHashCode() >> 2 ^ Bottom.GetHashCode() >> 1;
		
		public override bool Equals(object other)
			=> other is RectOffset otherRect && Left == otherRect.Left && Right == otherRect.Right && Top == otherRect.Top && Bottom == otherRect.Bottom;

		public static bool operator ==(RectOffset a, RectOffset b) => a.Equals(b);

		public static bool operator !=(RectOffset a, RectOffset b) => !a.Equals(b);
	}
}
