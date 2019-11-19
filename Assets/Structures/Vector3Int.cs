using System.Runtime.InteropServices;

namespace GameEngine
{
	public struct Vector3Int
	{
		public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Vector3Int));
		public static readonly Vector3Int Zero = default;
		public static readonly Vector3Int One = new Vector3Int(1,1,1);
		public static readonly Vector3Int UnitX = new Vector3Int(1,0,0);
		public static readonly Vector3Int UnitY = new Vector3Int(0,1,0);
		public static readonly Vector3Int UnitZ = new Vector3Int(0,0,1);
		public static readonly Vector3Int Up = new Vector3Int(0,1,0);
		public static readonly Vector3Int Down = new Vector3Int(0,-1,0);
		public static readonly Vector3Int Left = new Vector3Int(-1,0,0);
		public static readonly Vector3Int Right = new Vector3Int(1,0,0);
		public static readonly Vector3Int Forward = new Vector3Int(0,0,1);
		public static readonly Vector3Int Backward = new Vector3Int(0,0,-1);

		public int x;
		public int y;
		public int z;

		public Vector3Int(int X,int Y,int Z)
		{
			x = X;
			y = Y;
			z = Z;
		}
		public override string ToString()
		{
			return "X: "+x+",Y: "+y+",Z: "+z;
		}

		public static explicit operator Vector3(Vector3Int value) => new Vector3(value.x,value.y,value.z);
		public static explicit operator Vector3Int(Vector3 value) => new Vector3Int((int)value.x,(int)value.y,(int)value.z);

		public static Vector3Int operator +(Vector3Int a,Vector3Int b) => new Vector3Int(a.x+b.x,a.y+b.y,a.z+b.z);
		public static Vector3Int operator -(Vector3Int a,Vector3Int b) => new Vector3Int(a.x-b.x,a.y-b.y,a.z+b.z);
		public static Vector3Int operator -(Vector3Int a) => new Vector3Int(-a.x,-a.y,-a.z);
		public static Vector3Int operator *(Vector3Int a,int d) => new Vector3Int(a.x*d,a.y*d,a.z*d);
		public static Vector3Int operator *(int d,Vector3Int a) => new Vector3Int(a.x*d,a.y*d,a.z*d);
		public static Vector3Int operator /(Vector3Int a,int d) => new Vector3Int(a.x/d,a.y/d,a.z/d);
		public static Vector3 operator *(Vector3Int a,float d) => new Vector3(a.x*d,a.y*d,a.z*d);
		public static Vector3 operator /(Vector3Int a,float d) => new Vector3(a.x/d,a.y/d,a.z/d);
		public static bool operator ==(Vector3Int a,Vector3Int b) => a.x==b.x && a.y==b.y && a.z==b.z;
		public static bool operator !=(Vector3Int a,Vector3Int b) => a.x!=b.x || a.y!=b.y || a.z!=b.z;

		public override int GetHashCode()
		{
			return x.GetHashCode()^y.GetHashCode()<<2^z.GetHashCode()>>2;
		}
		public override bool Equals(object other)
		{
			if(!(other is Vector3Int)) {
				return false;
			}
			var otherPoint = (Vector3Int)other;
			return x==otherPoint.x && y==otherPoint.y && z==otherPoint.z;
		}
	}
}

