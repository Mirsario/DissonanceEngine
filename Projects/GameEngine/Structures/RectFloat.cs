namespace GameEngine
{
	public struct RectFloat
	{
		public float x;
		public float y;
		public float width;
		public float height;

		public float Right {
			get => x+width;
			set => x = value-width;
		}
		public float Bottom {
			get => y+height;
			set => y = value-height;
		}

		public RectFloat(float x,float y,float width,float height)
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

		public static RectFloat FromPoints(float x1,float y1,float x2,float y2)
		{
			RectFloat rect;
			rect.x = x1;
			rect.y = y1;
			rect.width = x2-x1;
			rect.height = y2-y1;
			return rect;
		}

		public static implicit operator RectFloat(RectInt rectI) => new RectFloat(rectI.x,rectI.y,rectI.width,rectI.height);
	}
}