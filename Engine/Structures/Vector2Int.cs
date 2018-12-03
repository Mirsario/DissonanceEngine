using System;
using System.Collections.Generic;

namespace GameEngine
{
	public struct Vector2Int
	{
		public int x;
		public int y;
		public Vector2Int(int X,int Y)
		{
			x = X;
			y = Y;
		}
		
		public static implicit operator System.Drawing.Point(Vector2Int value) => new System.Drawing.Point(value.x,value.y);
		public static implicit operator Vector2Int(System.Drawing.Point value) => new Vector2Int(value.X,value.Y);

		public static explicit operator Vector2(Vector2Int value) => new Vector2(value.x,value.y);
		public static explicit operator Vector2Int(Vector2 value) => new Vector2Int((int)value.x,(int)value.y);

		#region Operators
		#region Vector2Int
		public static Vector2Int operator+(Vector2Int a,Vector2Int b) => new Vector2Int(a.x+b.x,a.y+b.y);
		public static Vector2Int operator-(Vector2Int a,Vector2Int b) => new Vector2Int(a.x-b.x,a.y-b.y);
		public static Vector2Int operator*(Vector2Int a,Vector2Int b) => new Vector2Int(a.x*b.x,a.y*b.y);
		public static Vector2Int operator/(Vector2Int a,Vector2Int b) => new Vector2Int(a.x/b.x,a.y/b.y);
		public static Vector2Int operator-(Vector2Int a) => new Vector2Int(-a.x,-a.y);

		public static bool operator==(Vector2Int a,Vector2Int b) => a.x==b.x && a.y==b.y;
		public static bool operator!=(Vector2Int a,Vector2Int b) => a.x!=b.x || a.y!=b.y;
		#endregion
		#region Vector2
		public static Vector2 operator+(Vector2Int a,Vector2 b) => new Vector2(a.x+b.x,a.y+b.y);
		public static Vector2 operator+(Vector2 a,Vector2Int b) => new Vector2(a.x+b.x,a.y+b.y);
		public static Vector2 operator-(Vector2Int a,Vector2 b) => new Vector2(a.x-b.x,a.y-b.y);
		public static Vector2 operator-(Vector2 a,Vector2Int b) => new Vector2(a.x-b.x,a.y-b.y);
		public static Vector2 operator*(Vector2Int a,Vector2 b) => new Vector2(a.x*b.x,a.y*b.y);
		public static Vector2 operator*(Vector2 a,Vector2Int b) => new Vector2(a.x*b.x,a.y*b.y);
		public static Vector2 operator/(Vector2Int a,Vector2 b) => new Vector2(a.x/b.x,a.y/b.y);
		public static Vector2 operator/(Vector2 a,Vector2Int b) => new Vector2(a.x/b.x,a.y/b.y);

		public static bool operator==(Vector2Int a,Vector2 b) => a.x==b.x && a.y==b.y;
		public static bool operator==(Vector2 a,Vector2Int b) => a.x==b.x && a.y==b.y;
		public static bool operator!=(Vector2Int a,Vector2 b) => a.x!=b.x || a.y!=b.y;
		public static bool operator!=(Vector2 a,Vector2Int b) => a.x!=b.x || a.y!=b.y;
		#endregion
		#region Int
		public static Vector2Int operator*(Vector2Int a,int d) => new Vector2Int(a.x*d,a.y*d);
		public static Vector2Int operator*(int d,Vector2Int a) => new Vector2Int(a.x*d,a.y*d);
		public static Vector2Int operator/(Vector2Int a,int d) => new Vector2Int(a.x/d,a.y/d);
		#endregion
		#region Float
		public static Vector2 operator*(Vector2Int a,float d) => new Vector2(a.x*d,a.y*d);
		public static Vector2 operator*(float d,Vector2Int a) => new Vector2(d*a.x,d*a.y);
		public static Vector2 operator/(Vector2Int a,float d) => new Vector2(a.x/d,a.y/d);
		#endregion
		#endregion


		public override string ToString()
		{
			return "X: "+x+",Y: "+y;
		}
		public override int GetHashCode()
		{
			return x.GetHashCode()^y.GetHashCode()<<2;
		}
		public override bool Equals(object other)
		{
			if(!(other is Vector2Int)) {
				return false;
			}
			var point = (Vector2Int)other;
			return x==point.x && y==point.y;
		}
	}
}

