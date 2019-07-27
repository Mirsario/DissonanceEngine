namespace GameEngine
{
	public struct RectInt
	{
		public int x;
		public int y;
		public int width;
		public int height;

		public RectInt(Vector2Int position,Vector2Int size) : this(position.x,position.y,size.x,size.y) {}
		public RectInt(Vector2Int position,int width,int height) : this(position.x,position.y,width,height) {}
		public RectInt(int x,int y,Vector2Int size) : this(x,y,size.x,size.y) {}
		public RectInt(int x,int y,int width,int height)
		{
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
		}

		public bool Contains(Vector2 point,bool notOnBorders = false)
		{
			if(notOnBorders) {
				return point.x>x && point.x<x+width && point.y>y && point.y<y+height;
			}
			return point.x>=x && point.x<=x+width && point.y>=y && point.y<=y+height;
		}

		public static RectInt FromPoints(int x1,int y1,int x2,int y2)
		{
			RectInt rect;
			rect.x = x1;
			rect.y = y1;
			rect.width = x2-x1;
			rect.height = y2-y1;
			return rect;
		}

		public static implicit operator RectInt(RectFloat rectF) => new RectInt((int)rectF.x,(int)rectF.y,(int)rectF.width,(int)rectF.height);
	}
}