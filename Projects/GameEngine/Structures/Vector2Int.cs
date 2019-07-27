namespace GameEngine
{
	public partial struct Vector2Int
	{
		public int x;
		public int y;
		public Vector2Int(int X,int Y)
		{
			x = X;
			y = Y;
		}

		//System.Drawing.Point
		public static implicit operator System.Drawing.Point(Vector2Int value) => new System.Drawing.Point(value.x,value.y);
		public static implicit operator Vector2Int(System.Drawing.Point value) => new Vector2Int(value.X,value.Y);

		//Vector2Int
		public static Vector2Int operator +(Vector2Int a,Vector2Int b) => new Vector2Int(a.x+b.x,a.y+b.y);
		public static Vector2Int operator -(Vector2Int a,Vector2Int b) => new Vector2Int(a.x-b.x,a.y-b.y);
		public static Vector2Int operator *(Vector2Int a,Vector2Int b) => new Vector2Int(a.x*b.x,a.y*b.y);
		public static Vector2Int operator /(Vector2Int a,Vector2Int b) => new Vector2Int(a.x/b.x,a.y/b.y);
		public static Vector2Int operator -(Vector2Int a) => new Vector2Int(-a.x,-a.y);
		public static bool operator ==(Vector2Int a,Vector2Int b) => a.x==b.x && a.y==b.y;
		public static bool operator !=(Vector2Int a,Vector2Int b) => a.x!=b.x || a.y!=b.y;
		//int
		public static Vector2Int operator*(Vector2Int a,int d) => new Vector2Int(a.x*d,a.y*d);
		public static Vector2Int operator*(int d,Vector2Int a) => new Vector2Int(a.x*d,a.y*d);
		public static Vector2Int operator/(Vector2Int a,int d) => new Vector2Int(a.x/d,a.y/d);
		//float
		public static Vector2 operator*(Vector2Int a,float d) => new Vector2(a.x*d,a.y*d);
		public static Vector2 operator*(float d,Vector2Int a) => new Vector2(d*a.x,d*a.y);
		public static Vector2 operator/(Vector2Int a,float d) => new Vector2(a.x/d,a.y/d);

		public override string ToString() => "X: "+x+",Y: "+y;
		public override int GetHashCode() => x^y<<2;
		public override bool Equals(object other)
		{
			if(!(other is Vector2Int)) {
				return false;
			}
			Vector2Int point = (Vector2Int)other;
			return x==point.x && y==point.y;
		}
	}
}

