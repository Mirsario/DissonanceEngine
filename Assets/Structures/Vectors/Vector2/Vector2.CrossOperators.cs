namespace GameEngine
{
	public partial struct Vector2
	{
		//OpenTK.Vector2
		public static implicit operator OpenTK.Vector2(Vector2 value) => new OpenTK.Vector2(value.x,value.y);
		public static implicit operator Vector2(OpenTK.Vector2 value) => new Vector2(value.X,value.Y);

		//Vector2Int
		public static explicit operator Vector2(Vector2Int value) => new Vector2(value.x,value.y);
		public static Vector2 operator +(Vector2Int a,Vector2 b) => new Vector2(a.x+b.x,a.y+b.y);
		public static Vector2 operator +(Vector2 a,Vector2Int b) => new Vector2(a.x+b.x,a.y+b.y);
		public static Vector2 operator -(Vector2Int a,Vector2 b) => new Vector2(a.x-b.x,a.y-b.y);
		public static Vector2 operator -(Vector2 a,Vector2Int b) => new Vector2(a.x-b.x,a.y-b.y);
		public static Vector2 operator *(Vector2Int a,Vector2 b) => new Vector2(a.x*b.x,a.y*b.y);
		public static Vector2 operator *(Vector2 a,Vector2Int b) => new Vector2(a.x*b.x,a.y*b.y);
		public static Vector2 operator /(Vector2Int a,Vector2 b) => new Vector2(a.x/b.x,a.y/b.y);
		public static Vector2 operator /(Vector2 a,Vector2Int b) => new Vector2(a.x/b.x,a.y/b.y);

		//Vector2UShort
		public static explicit operator Vector2(Vector2UShort value) => new Vector2(value.x,value.y);
		public static Vector2 operator +(Vector2UShort a,Vector2 b) => new Vector2(a.x+b.x,a.y+b.y);
		public static Vector2 operator +(Vector2 a,Vector2UShort b) => new Vector2(a.x+b.x,a.y+b.y);
		public static Vector2 operator -(Vector2UShort a,Vector2 b) => new Vector2(a.x-b.x,a.y-b.y);
		public static Vector2 operator -(Vector2 a,Vector2UShort b) => new Vector2(a.x-b.x,a.y-b.y);
		public static Vector2 operator *(Vector2UShort a,Vector2 b) => new Vector2(a.x*b.x,a.y*b.y);
		public static Vector2 operator *(Vector2 a,Vector2UShort b) => new Vector2(a.x*b.x,a.y*b.y);
		public static Vector2 operator /(Vector2UShort a,Vector2 b) => new Vector2(a.x/b.x,a.y/b.y);
		public static Vector2 operator /(Vector2 a,Vector2UShort b) => new Vector2(a.x/b.x,a.y/b.y);
	}

	public partial struct Vector2Int
	{
		//Vector2
		public static explicit operator Vector2Int(Vector2 value) => new Vector2Int((int)value.x,(int)value.y);
		public static bool operator ==(Vector2Int a,Vector2 b) => a.x==b.x && a.y==b.y;
		public static bool operator ==(Vector2 a,Vector2Int b) => a.x==b.x && a.y==b.y;
		public static bool operator !=(Vector2Int a,Vector2 b) => a.x!=b.x || a.y!=b.y;
		public static bool operator !=(Vector2 a,Vector2Int b) => a.x!=b.x || a.y!=b.y;

		//Vector2UShort
		public static explicit operator Vector2Int(Vector2UShort value) => new Vector2Int(value.x,value.y);
		public static Vector2Int operator +(Vector2Int a,Vector2UShort b) => new Vector2Int(a.x+b.x,a.y+b.y);
		public static Vector2Int operator +(Vector2UShort a,Vector2Int b) => new Vector2Int(a.x+b.x,a.y+b.y);
		public static Vector2Int operator -(Vector2Int a,Vector2UShort b) => new Vector2Int(a.x-b.x,a.y-b.y);
		public static Vector2Int operator -(Vector2UShort a,Vector2Int b) => new Vector2Int(a.x-b.x,a.y-b.y);
		public static Vector2Int operator *(Vector2Int a,Vector2UShort b) => new Vector2Int(a.x*b.x,a.y*b.y);
		public static Vector2Int operator *(Vector2UShort a,Vector2Int b) => new Vector2Int(a.x*b.x,a.y*b.y);
		public static Vector2Int operator /(Vector2Int a,Vector2UShort b) => new Vector2Int(a.x/b.x,a.y/b.y);
		public static Vector2Int operator /(Vector2UShort a,Vector2Int b) => new Vector2Int(a.x/b.x,a.y/b.y);
	}

	public partial struct Vector2UShort
	{
		//Vector2
		public static explicit operator Vector2UShort(Vector2 value) => new Vector2UShort((ushort)value.x,(ushort)value.y);

		//Vector2Int
		public static explicit operator Vector2UShort(Vector2Int value) => new Vector2UShort((ushort)value.x,(ushort)value.y);
	}
}
