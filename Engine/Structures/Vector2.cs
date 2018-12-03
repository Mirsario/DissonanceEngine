using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Matrix3 = OpenTK.Matrix3;

namespace GameEngine
{
	public struct Vector2
	{
		public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Vector2));
		public static readonly Vector2 zero = default;
		public static readonly Vector2 one = new Vector2( 1f,1f);
		public static readonly Vector2 unitX = new Vector2( 1f,0f);
		public static readonly Vector2 unitY = new Vector2( 0f,1f);
		public static readonly Vector2 up = new Vector2( 0f,1f);
		public static readonly Vector2 down = new Vector2( 0f,-1f);
		public static readonly Vector2 left = new Vector2(-1f,0f);
		public static readonly Vector2 right = new Vector2( 1f,0f);

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

		public static Vector2 Lerp(Vector2 from,Vector2 to,float t)
		{
			t = Mathf.Clamp01(t);
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
		public static float Distance(Vector2 a,Vector2 b) => (a-b).Magnitude;
		public static float SqrDistance(Vector2 a,Vector2 b) => (a-b).SqrMagnitude;

		public static Vector2 operator +(Vector2 a,Vector2 b) => new Vector2(a.x+b.x,a.y+b.y);
		public static Vector2 operator -(Vector2 a,Vector2 b) => new Vector2(a.x-b.x,a.y-b.y);
		public static Vector2 operator *(Vector2 a,Vector2 b) => new Vector2(a.x*b.x,a.y*b.y);
		public static Vector2 operator -(Vector2 a) => new Vector2(-a.x,-a.y);
		public static Vector2 operator *(Vector2 a,float d) => new Vector2(a.x*d,a.y*d);
		public static Vector2 operator *(float d,Vector2 a) => new Vector2(a.x*d,a.y*d);
		public static Vector2 operator /(Vector2 a,float d) => new Vector2(a.x/d,a.y/d);
		public static bool operator ==(Vector2 a,Vector2 b) => (a-b).SqrMagnitude<9.99999944E-11f;
		public static bool operator !=(Vector2 a,Vector2 b) => (a-b).SqrMagnitude>=9.99999944E-11f;

		public static implicit operator OpenTK.Vector2(Vector2 value) => new OpenTK.Vector2(value.x,value.y);
		public static implicit operator Vector2(OpenTK.Vector2 value) => new Vector2(value.X,value.Y);
	}
}

