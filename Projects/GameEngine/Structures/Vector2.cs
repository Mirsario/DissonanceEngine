using System;
using System.Runtime.InteropServices;

namespace GameEngine
{
	public partial struct Vector2
	{
		public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Vector2));
		public static readonly Vector2 Zero = default;
		public static readonly Vector2 One = new Vector2( 1f,1f);
		public static readonly Vector2 UnitX = new Vector2( 1f,0f);
		public static readonly Vector2 UnitY = new Vector2( 0f,1f);
		public static readonly Vector2 Up = new Vector2( 0f,1f);
		public static readonly Vector2 Down = new Vector2( 0f,-1f);
		public static readonly Vector2 Left = new Vector2(-1f,0f);
		public static readonly Vector2 Right = new Vector2( 1f,0f);

		public float x;
		public float y;

		public Vector2 Normalized => Normalize(this);
		public float Magnitude => Mathf.Sqrt(x*x+y*y);
		public float SqrMagnitude => x*x+y*y;

		public float this[int index]
		{
			get {
				switch(index) {
					case 0: return x;
					case 1: return y;
					default:
						throw new IndexOutOfRangeException("Indices for Vector2 run from 0 to 1,inclusive.");
				}
			}
			set {
				switch(index) {
					case 0: x = value; return;
					case 1: y = value; return;
					default:
						throw new IndexOutOfRangeException("Indices for Vector2 run from 0 to 1,inclusive.");
				}
			}
		}

		public Vector2(float X,float Y)
		{
			x = X;
			y = Y;
		}

		public void Normalize()
		{
			float mag = Magnitude;
			if(mag!=0f) {
				float num = 1f/mag;
				x *= num;
				y *= num;
			}
		}
		public void Normalize(out float magnitude)
		{
			magnitude = Magnitude;
			if(magnitude!=0f) {
				float num = 1f/magnitude;
				x *= num;
				y *= num;
			}
		}

		public float[] ToArray() => new[] { x,y };
		public override string ToString() => "X: "+x+",Y: "+y;
		public override int GetHashCode() => x.GetHashCode()^y.GetHashCode()<<2;
		public override bool Equals(object other)
		{
			if(!(other is Vector2 vector)) {
				return false;
			}
			return x.Equals(vector.x) && y.Equals(vector.y);
		}

		public static Vector2 StepTowards(Vector2 val,Vector2 goal,float step) => new Vector2(
			Mathf.StepTowards(val.x,goal.x,step),
			Mathf.StepTowards(val.y,goal.y,step)
		);
		public static Vector2 SnapToGrid(Vector2 val,float step) => new Vector2(
			Mathf.SnapToGrid(val.x,step),
			Mathf.SnapToGrid(val.y,step)
		);
		public static Vector2 Lerp(Vector2 from,Vector2 to,float t)
		{
			if(t<0f) {
				t = 0f;
			}else if(t>1f) {
				t = 1f;
			}
			return new Vector2(from.x+(to.x-from.x)*t,from.y+(to.y-from.y)*t);
		}
		public static Vector2 Normalize(Vector2 vec)
		{
			float mag = vec.Magnitude;
			if(mag!=0f) {
				float num = 1f/mag;
				vec.x *= num;
				vec.y *= num;
			}
			return vec;
		}
		public static Vector2 Rotate(Vector2 vec,float angle)
		{
			float sin = Mathf.Sin(angle*Mathf.Deg2Rad);
			float cos = Mathf.Cos(angle*Mathf.Deg2Rad);
			return new Vector2((cos*vec.x)-(sin*vec.y),(sin*vec.x)+(cos*vec.y));
		}
		public static float Dot(Vector2 a,Vector2 b) => a.x*b.x+a.y*b.y;
		public static float Distance(Vector2 a,Vector2 b) => (a-b).Magnitude;
		public static float SqrDistance(Vector2 a,Vector2 b) => (a-b).SqrMagnitude;

		//Operators
		public static Vector2 operator -(Vector2 a) => new Vector2(-a.x,-a.y);
		public static Vector2 operator +(Vector2 a,Vector2 b) => new Vector2(a.x+b.x,a.y+b.y);
		public static Vector2 operator -(Vector2 a,Vector2 b) => new Vector2(a.x-b.x,a.y-b.y);
		public static Vector2 operator *(Vector2 a,Vector2 b) => new Vector2(a.x*b.x,a.y*b.y);
		public static Vector2 operator /(Vector2 a,Vector2 b) => new Vector2(a.x/b.x,a.y/b.y);
		public static bool operator ==(Vector2 a,Vector2 b) => (a-b).SqrMagnitude<9.99999944E-11f;
		public static bool operator !=(Vector2 a,Vector2 b) => (a-b).SqrMagnitude>=9.99999944E-11f;
		//Float
		public static Vector2 operator *(Vector2 a,int d) => new Vector2(a.x*d,a.y*d);
		public static Vector2 operator *(int d,Vector2 a) => new Vector2(a.x*d,a.y*d);
		public static Vector2 operator /(Vector2 a,int d) => new Vector2(a.x/d,a.y/d);
		//Float
		public static Vector2 operator *(Vector2 a,float d) => new Vector2(a.x*d,a.y*d);
		public static Vector2 operator *(float d,Vector2 a) => new Vector2(a.x*d,a.y*d);
		public static Vector2 operator /(Vector2 a,float d) => new Vector2(a.x/d,a.y/d);
	}
}

