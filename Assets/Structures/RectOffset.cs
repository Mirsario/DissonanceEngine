namespace GameEngine
{
	public struct RectOffset
	{
		public float left;
		public float right;
		public float top;
		public float bottom;
		
		public RectOffset(float Left,float Top,float Right,float Bottom)
		{
			left = Left;
			top = Top;
			right = Right;
			bottom = Bottom;
		}
		
		public static bool operator==(RectOffset a,RectOffset b)
		{
			return a.Equals(b);
		}
		public static bool operator!=(RectOffset a,RectOffset b)
		{
			return !a.Equals(b);
		}
		public override int GetHashCode()
		{
			return left.GetHashCode()^right.GetHashCode()<<2^top.GetHashCode()>>2^bottom.GetHashCode()>>1;
		}
		public override bool Equals(object other)
		{
			if(!(other is RectOffset)) {
				return false;
			}
			var otherNew = (RectOffset)other;
			return left.Equals(otherNew.left) && right.Equals(otherNew.right) && top.Equals(otherNew.top) && bottom.Equals(otherNew.bottom);
		}
	}
}