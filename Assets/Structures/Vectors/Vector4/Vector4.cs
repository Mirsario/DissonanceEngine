using System;
using System.Runtime.InteropServices;

namespace Dissonance.Engine
{
	public struct Vector4
	{
		public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Vector4));
		public static readonly Vector4 Zero = default;
		public static readonly Vector4 One = new Vector4(1f,1f,1f,1f);
		public static readonly Vector4 UnitX = new Vector4(1f,0f,0f,0f);
		public static readonly Vector4 UnitY = new Vector4(0f,1f,0f,0f);
		public static readonly Vector4 UnitZ = new Vector4(0f,0f,1f,0f);
		public static readonly Vector4 UnitW = new Vector4(0f,0f,0f,1f);

		public float x;
		public float y;
		public float z;
		public float w;

		public float Magnitude => Mathf.Sqrt(x*x+y*y+z*z+w*w);
		public float SqrMagnitude => x*x+y*y+z*z+w*w;
		public bool HasNaNs => float.IsNaN(x) || float.IsNaN(y) || float.IsNaN(z);
		public Vector2 XY => new Vector2(x,y);
		public Vector3 XYZ => new Vector3(x,y,z);
		public Vector4 Normalized {
			get {
				float mag = Magnitude;

				if(mag!=0f) {
					return this*(1f/mag);
				}

				return this;
			}
		}

		public float this[int index] {
			get => index switch {
				0 => x,
				1 => y,
				2 => z,
				3 => w,
				_ => throw new IndexOutOfRangeException("Indices for Vector4 run from 0 to 3,inclusive."),
			};
			set {
				switch(index) {
					case 0: x = value; return;
					case 1: y = value; return;
					case 2: z = value; return;
					case 3: w = value; return;
					default:
						throw new IndexOutOfRangeException("Indices for Vector4 run from 0 to 3,inclusive.");
				}
			}
		}
		public Vector4(float X,float Y,float Z,float W)
		{
			x = X;
			y = Y;
			z = Z;
			w = W;
		}
		public Vector4(float X,float Y,float Z)
		{
			x = X;
			y = Y;
			z = Z;
			w = 0f;
		}
		public Vector4(float X,float Y)
		{
			x = X;
			y = Y;
			z = 0f;
			w = 0f;
		}
		public Vector4(Vector3 vec3,float W)
		{
			x = vec3.x;
			y = vec3.y;
			z = vec3.z;
			w = W;
		}
		
		public float[] ToArray()
		{
			return new[] {
				x,y,z,w
			};
		}
		public override string ToString()
		{
			return "X: "+x+",Y: "+y+",Z: "+z+",W: "+w;
		}
		public void Normalize()
		{
			float mag = Magnitude;
			if(mag!=0f) {
				float num = 1f/mag;
				x *= num;
				y *= num;
				z *= num;
				w *= num;
			}
		}
		public void Normalize(out float magnitude)
		{
			magnitude = Magnitude;
			if(magnitude!=0f) {
				float num = 1f/magnitude;
				x *= num;
				y *= num;
				z *= num;
				w *= num;
			}
		}

		public static Vector4 Lerp(Vector4 from,Vector4 to,float t)
		{
			t = Mathf.Clamp01(t);
			return new Vector4(from.x+(to.x-from.x)*t,from.y+(to.y-from.y)*t,from.z+(to.z-from.z)*t,from.w+(to.w-from.w)*t);
		}
		public static Vector4 BiLerp(Vector4 valueTopLeft,Vector4 valueTopRight,Vector4 valueBottomLeft,Vector4 valueBottomRight,Vector2 topLeft,Vector2 bottomRight,Vector2 point)
		{
			float x2x1,y2y1,x2x,y2y,yy1,xx1;
			x2x1 = bottomRight.x-topLeft.x;
			y2y1 = bottomRight.y-topLeft.y;
			x2x = bottomRight.x-point.x;
			y2y = bottomRight.y-point.y;
			yy1 = point.y-topLeft.y;
			xx1 = point.x-topLeft.x;
			float mul = 1f/(x2x1*y2y1);
			float mulTopLeft = x2x*yy1;
			float mulTopRight = xx1*yy1;
			float mulBottomLeft = x2x*y2y;
			float mulBottomRight = xx1*y2y;
			return new Vector4(
				mul*(valueTopLeft.x*mulTopLeft+valueTopRight.x*mulTopRight+valueBottomLeft.x*mulBottomLeft+valueBottomRight.x*mulBottomRight),
				mul*(valueTopLeft.y*mulTopLeft+valueTopRight.y*mulTopRight+valueBottomLeft.y*mulBottomLeft+valueBottomRight.y*mulBottomRight),
				mul*(valueTopLeft.z*mulTopLeft+valueTopRight.z*mulTopRight+valueBottomLeft.z*mulBottomLeft+valueBottomRight.z*mulBottomRight),
				mul*(valueTopLeft.w*mulTopLeft+valueTopRight.w*mulTopRight+valueBottomLeft.w*mulBottomLeft+valueBottomRight.w*mulBottomRight)
			);
		}
		public static float Distance(Vector4 a,Vector4 b)
		{
			return (a-b).Magnitude;
		}
		public static float SqrDistance(Vector4 a,Vector4 b)
		{
			return (a-b).SqrMagnitude;
		}
		public static Vector4 Floor(Vector4 vec)
		{
			vec.x = Mathf.Floor(vec.x);
			vec.y = Mathf.Floor(vec.y);
			vec.z = Mathf.Floor(vec.z);
			vec.w = Mathf.Floor(vec.w);
			return vec;
		}
		public static Vector4 Ceil(Vector4 vec)
		{
			vec.x = Mathf.Ceil(vec.x);
			vec.y = Mathf.Ceil(vec.y);
			vec.z = Mathf.Ceil(vec.z);
			vec.w = Mathf.Ceil(vec.w);
			return vec;
		}
		public static Vector4 Round(Vector4 vec)
		{
			vec.x = Mathf.Round(vec.x);
			vec.y = Mathf.Round(vec.y);
			vec.z = Mathf.Round(vec.z);
			vec.w = Mathf.Round(vec.w);
			return vec;
		}

		public override int GetHashCode() => x.GetHashCode()^y.GetHashCode()<<2^z.GetHashCode()>>2^w.GetHashCode()>>1;
		public override bool Equals(object other) => other is Vector4 vector && x==vector.x && y==vector.y && z==vector.z && w==vector.w;

		public static explicit operator Vector4(Vector3 value) => new Vector4(value.x,value.y,value.z,0f);
		public static explicit operator Vector3(Vector4 value) => new Vector3(value.x,value.y,value.z);
		public static explicit operator Vector2(Vector4 value) => new Vector2(value.x,value.y);
		//Vector4
		public static Vector4 operator +(Vector4 a,Vector4 b) => new Vector4(a.x+b.x,a.y+b.y,a.z+b.z,a.w+b.w);
		public static Vector4 operator -(Vector4 a,Vector4 b) => new Vector4(a.x-b.x,a.y-b.y,a.z-b.z,a.w-b.w);
		public static Vector4 operator -(Vector4 a) => new Vector4(-a.x,-a.y,-a.z,-a.w);
		public static Vector4 operator *(Vector4 a,float d) => new Vector4(a.x*d,a.y*d,a.z*d,a.w*d);
		public static Vector4 operator *(float d,Vector4 a) => new Vector4(a.x*d,a.y*d,a.z*d,a.w*d);
		public static Vector4 operator /(Vector4 a,float d) => new Vector4(a.x/d,a.y/d,a.z/d,a.w/d);
		public static bool operator ==(Vector4 a,Vector4 b) => (a-b).SqrMagnitude<9.99999944E-11f;
		public static bool operator !=(Vector4 a,Vector4 b) => (a-b).SqrMagnitude>=9.99999944E-11f;

		public static unsafe implicit operator float*(Vector4 vec) => (float*)&vec;
	}
}

