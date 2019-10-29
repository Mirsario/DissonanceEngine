namespace AbyssCrusaders
{
	public struct CollisionInfo
	{
		public static readonly CollisionInfo Full = new CollisionInfo(true,true,true,true);
		public static readonly CollisionInfo None = default;
		
		public bool up;
		public bool down;
		public bool left;
		public bool right;

		public CollisionInfo(bool up,bool down,bool left,bool right)
		{
			this.up = up;
			this.down = down;
			this.left = left;
			this.right = right;
		}
	}
}
