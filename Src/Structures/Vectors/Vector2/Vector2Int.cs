using System.Runtime.InteropServices;

namespace Dissonance.Engine
{
	public struct Vector2Int
	{
		public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Vector2Int));
		public static readonly Vector2Int Zero = default;
		public static readonly Vector2Int One = new Vector2Int(1,1);
		public static readonly Vector2Int UnitX = new Vector2Int(1,0);
		public static readonly Vector2Int UnitY = new Vector2Int(0,1);
		public static readonly Vector2Int Up = new Vector2Int(0,1);
		public static readonly Vector2Int Down = new Vector2Int(0,-1);
		public static readonly Vector2Int Left = new Vector2Int(-1,0);
		public static readonly Vector2Int Right = new Vector2Int(1,0);

		public int x;
		public int y;

		public Vector2Int(int X,int Y)
		{
			x = X;
			y = Y;
		}

		public override int GetHashCode() => x^y<<2;
		public override bool Equals(object other) => other is Vector2Int point && x==point.x && y==point.y;
		public override string ToString() => "X: "+x+",Y: "+y;

		//Operations

		//int
		public static Vector2Int operator *(Vector2Int a,int d) => new Vector2Int(a.x*d,a.y*d);
		public static Vector2Int operator *(int d,Vector2Int a) => new Vector2Int(a.x*d,a.y*d);
		public static Vector2Int operator /(Vector2Int a,int d) => new Vector2Int(a.x/d,a.y/d);
		//float
		public static Vector2 operator *(Vector2Int a,float d) => new Vector2(a.x*d,a.y*d);
		public static Vector2 operator *(float d,Vector2Int a) => new Vector2(d*a.x,d*a.y);
		public static Vector2 operator /(Vector2Int a,float d) => new Vector2(a.x/d,a.y/d);
		//Vector2Int
		public static Vector2Int operator +(Vector2Int a,Vector2Int b) => new Vector2Int(a.x+b.x,a.y+b.y);
		public static Vector2Int operator -(Vector2Int a,Vector2Int b) => new Vector2Int(a.x-b.x,a.y-b.y);
		public static Vector2Int operator *(Vector2Int a,Vector2Int b) => new Vector2Int(a.x*b.x,a.y*b.y);
		public static Vector2Int operator /(Vector2Int a,Vector2Int b) => new Vector2Int(a.x/b.x,a.y/b.y);
		public static Vector2Int operator -(Vector2Int a) => new Vector2Int(-a.x,-a.y);
		public static bool operator ==(Vector2Int a,Vector2Int b) => a.x==b.x && a.y==b.y;
		public static bool operator !=(Vector2Int a,Vector2Int b) => a.x!=b.x || a.y!=b.y;
		//Vector2
		public static bool operator ==(Vector2Int a,Vector2 b) => a.x==b.x && a.y==b.y;
		public static bool operator ==(Vector2 a,Vector2Int b) => a.x==b.x && a.y==b.y;
		public static bool operator !=(Vector2Int a,Vector2 b) => a.x!=b.x || a.y!=b.y;
		public static bool operator !=(Vector2 a,Vector2Int b) => a.x!=b.x || a.y!=b.y;
		//Vector2UShort
		public static Vector2Int operator +(Vector2Int a,Vector2UShort b) => new Vector2Int(a.x+b.x,a.y+b.y);
		public static Vector2Int operator +(Vector2UShort a,Vector2Int b) => new Vector2Int(a.x+b.x,a.y+b.y);
		public static Vector2Int operator -(Vector2Int a,Vector2UShort b) => new Vector2Int(a.x-b.x,a.y-b.y);
		public static Vector2Int operator -(Vector2UShort a,Vector2Int b) => new Vector2Int(a.x-b.x,a.y-b.y);
		public static Vector2Int operator *(Vector2Int a,Vector2UShort b) => new Vector2Int(a.x*b.x,a.y*b.y);
		public static Vector2Int operator *(Vector2UShort a,Vector2Int b) => new Vector2Int(a.x*b.x,a.y*b.y);
		public static Vector2Int operator /(Vector2Int a,Vector2UShort b) => new Vector2Int(a.x/b.x,a.y/b.y);
		public static Vector2Int operator /(Vector2UShort a,Vector2Int b) => new Vector2Int(a.x/b.x,a.y/b.y);

		//Casts

		//int*
		public static unsafe implicit operator int*(Vector2Int vec) => (int*)&vec;
		//System.Drawing.Point
		public static implicit operator System.Drawing.Point(Vector2Int value) => new System.Drawing.Point(value.x,value.y);
		public static implicit operator Vector2Int(System.Drawing.Point value) => new Vector2Int(value.X,value.Y);
		//Vector2Int
		public static explicit operator Vector2Int(Vector2 value) => new Vector2Int((int)value.x,(int)value.y);
		//Vector2UShort
		public static explicit operator Vector2Int(Vector2UShort value) => new Vector2Int(value.x,value.y);
	}
}

