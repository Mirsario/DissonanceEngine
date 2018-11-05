using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Matrix3 = OpenTK.Matrix3;

namespace GameEngine
{
	public struct Vector3
	{
        public const float kEpsilon = 0.00001F;
		public const float kEpsilonNormalSqrt = 1e-15F;

		public static readonly int sizeInBytes = Marshal.SizeOf(typeof(Vector3));
		public static readonly Vector3 zero = default;
		public static readonly Vector3 one = new Vector3( 1f,1f,1f);
		public static readonly Vector3 unitX = new Vector3( 1f,0f,0f);
		public static readonly Vector3 unitY = new Vector3( 0f,1f,0f);
		public static readonly Vector3 unitZ = new Vector3( 0f,0f,1f);
		public static readonly Vector3 up = new Vector3( 0f,1f,0f);
		public static readonly Vector3 down = new Vector3( 0f,-1f,0f);
		public static readonly Vector3 left = new Vector3(-1f,0f,0f);
		public static readonly Vector3 right = new Vector3( 1f,0f,0f);
		public static readonly Vector3 forward = new Vector3( 0f,0f,1f);
		public static readonly Vector3 backward = new Vector3( 0f,0f,-1f);

		public float Magnitude => Mathf.Sqrt(x*x+y*y+z*z);
		public float SqrMagnitude => x*x+y*y+z*z;
		public float x;
		public float y;
		public float z;

		public Vector2 XY {
			get => new Vector2(x,y);
			set { x = value.x; y = value.y; }
		}
		public Vector2 XZ {
			get => new Vector2(x,z);
			set { x = value.x; z = value.y; }
		}

		public Vector3 Normalized {
			get {
				var vec = new Vector3(x,y,z);
				float mag = Magnitude;
				if(mag!=0f) {
					vec *= 1f/mag;
				}
				return vec;
			}
		}
		
		public float this[int index] {
			get {
				switch(index) {
					case 0: return x;
					case 1: return y;
					case 2: return z;
					default: throw new IndexOutOfRangeException("Indices for Vector3 run from 0 to 2,inclusive.");
				}
			}
			set {
				switch(index) {
					case 0: x = value; return;
					case 1: y = value; return;
					case 2: z = value; return;
					default: throw new IndexOutOfRangeException("Indices for Vector3 run from 0 to 2,inclusive.");
				}
			}
		}
		public Vector3(float X,float Y,float Z)
		{
			x = X;
			y = Y;
			z = Z;
		}
		public Vector3(float XYZ)
		{
			x = XYZ;
			y = XYZ;
			z = XYZ;
		}
		
		public float[] ToArray() => new[] { x,y,z };
		public override string ToString() => string.Format(" {0:F5}\t {1:F5}\t {2:F5}",new object[]{x,y,z}).Replace("-","-");

		public void Normalize()
		{
			float mag = Magnitude;
			if(mag!=0f) {
				float num = 1f/mag;
				x *= num;
				y *= num;
				z *= num;
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
			}
		}
		public void NormalizeEuler()
		{
			while(x>=360f) {
				x -= 360f;
			}
			while(x<0f) {
				x += 360f;
			}
			while(y>=360f) {
				y -= 360f;
			}
			while(y<0f) {
				y += 360f;
			}
			while(z>=360f) {
				z -= 360f;
			}
			while(z<0f) {
				z += 360f;
			}
		}
		public Vector3 Rotate(Vector3 rot)
		{
			return Matrix4x4.CreateRotation(rot)*this;
		}
		public Vector3 RotatedBy(float x,float y,float z)
		{
			//TODO: Test if this should have all axes reversed or just Y
			return Matrix4x4.CreateRotation(-x,-y,-z)*this;
		}
		
		public static Vector3 StepTowards(Vector3 val,Vector3 goal,float step)
		{
			return new Vector3(
				Mathf.StepTowards(val.x,goal.x,step),
				Mathf.StepTowards(val.y,goal.y,step),
				Mathf.StepTowards(val.z,goal.z,step)
			);
		}
		public static Vector3 EulerToDirection(Vector3 euler)
		{
			euler *= Mathf.Deg2Rad;
			float cX = Mathf.Cos(euler.x);
			float sX = Mathf.Sin(euler.x);
			float cY = Mathf.Cos(euler.y);
			float sY = Mathf.Sin(euler.y);
			//float cZ = Mathf.Cos(euler.z);
			//float sZ = Mathf.Sin(euler.z);
			return-(new Vector3(-cX*sY,sX,cX*cY)*Mathf.Rad2Deg);
			//return (new Vector3(cY*cX,sY*cX,sX)*Mathf.Rad2Deg);
		}
		public static Vector3 DirectionToEuler(Vector3 direction)
		{
			//direction = direction.Normalized;

			//TODO: Redo this fucking garbage
			//Matrix4x4 matrix = Matrix4x4.LookAt(zero,direction,up);
			
			return Matrix4x4.LookAt(zero,direction.Normalized,up).ExtractEuler();
			/*float cX = Mathf.Cos(direction.x);
			float sX = Mathf.Sin(direction.x);
			float cY = Mathf.Cos(direction.y);
			float sY = Mathf.Sin(direction.y);
			float cZ = Mathf.Cos(direction.z);
			float sZ = Mathf.Sin(direction.z);
			return new Vector3(cY*cZ-sX*sY*sZ,cZ*sX*sY+cY*sZ,-cX*sY);*/
		}
		public static Vector3 Repeat(Vector3 vec,float length) => new Vector3(
			vec.x-Mathf.Floor(vec.x/length)*length,
			vec.y-Mathf.Floor(vec.y/length)*length,
			vec.z-Mathf.Floor(vec.z/length)*length
		);
		public static Vector3 Rotate(Vector3 vec,Vector3 rot) => Matrix4x4.CreateRotation(rot)*vec;
		public static Vector3 Normalize(Vector3 vec)
		{
			float mag = vec.Magnitude;
			if(mag!=0f) {
				vec *= 1f/mag;
			}
			return vec;
		}
		public static Vector3 Lerp(Vector3 from,Vector3 to,float t)
		{
			t = Mathf.Clamp01(t);
			return new Vector3(from.x+(to.x-from.x)*t,from.y+(to.y-from.y)*t,from.z+(to.z-from.z)*t);
		}
		public static void Cross(ref Vector3 left,ref Vector3 right,out Vector3 result)
		{
			result = new Vector3(
				left.y*right.z-left.z*right.y,
				left.z*right.x-left.x*right.z,
				left.x*right.y-left.y*right.x
			);
		}
		public static Vector3 Cross(Vector3 left,Vector3 right)
		{
			Cross(ref left,ref right,out var result);
			return result;
		}
		public static float Dot(Vector3 left,Vector3 right)
		{
			return left.x*right.x+left.y*right.y+left.z*right.z;
		}
		public static float Angle(Vector3 from,Vector3 to)
		{
			float denominator = Mathf.Sqrt(from.SqrMagnitude*to.SqrMagnitude);
			if(denominator<kEpsilonNormalSqrt) {
				return 0f;
			}
			float dot = Mathf.Clamp(Dot(from,to)/denominator,-1F,1F);
			return Mathf.Acos(dot)*Mathf.Rad2Deg;
		}
		public static float Distance(Vector3 a,Vector3 b)
		{
			return (a-b).Magnitude;
		}
		public static float SqrDistance(Vector3 a,Vector3 b)
		{
			return (a-b).SqrMagnitude;
		}
		
		public static Vector3 operator*(Vector3 a,Vector3 b) => new Vector3(a.x*b.x,a.y*b.y,a.z*b.z);	//Vector3
		public static Vector3 operator/(Vector3 a,Vector3 b) => new Vector3(a.x/b.x,a.y/b.y,a.z/b.z);
		public static Vector3 operator+(Vector3 a,Vector3 b) => new Vector3(a.x+b.x,a.y+b.y,a.z+b.z);
		public static Vector3 operator-(Vector3 a,Vector3 b) => new Vector3(a.x-b.x,a.y-b.y,a.z-b.z);
		public static Vector3 operator-(Vector3 a) => new Vector3(-a.x,-a.y,-a.z);
		public static bool operator==(Vector3 a,Vector3 b) => (a-b).SqrMagnitude<9.99999944E-11f;
		public static bool operator!=(Vector3 a,Vector3 b) => (a-b).SqrMagnitude>=9.99999944E-11f;
		public static Vector3 operator*(Vector3 a,float d) => new Vector3(a.x*d,a.y*d,a.z*d);			//Float
		public static Vector3 operator*(float d,Vector3 a) => new Vector3(a.x*d,a.y*d,a.z*d);
		public static Vector3 operator/(Vector3 a,float d) => new Vector3(a.x/d,a.y/d,a.z/d);
		
		public static implicit operator OpenTK.Vector3(Vector3 value) => new OpenTK.Vector3(value.x,value.y,value.z);
		public static implicit operator Vector3(OpenTK.Vector3 value) => new Vector3(value.X,value.Y,value.Z);
		public static implicit operator BulletSharp.Vector3(Vector3 value) => new BulletSharp.Vector3(value.x,value.y,value.z);
		public static implicit operator Vector3(BulletSharp.Vector3 value) => new Vector3(value.X,value.Y,value.Z);

		public override int GetHashCode()
		{
			return x.GetHashCode()^y.GetHashCode()<<2^z.GetHashCode()>>2;
		}
		public override bool Equals(object other)
		{
			if(!(other is Vector3)) {
				return false;
			}
			var vector = (Vector3)other;
			return x.Equals(vector.x) && y.Equals(vector.y) && z.Equals(vector.z);
		}
		/*public static implicit operator BulletSharp.Vector3(Vector3 value)
		{
			return new BulletSharp.Vector3(value.x,value.y,value.z);
		}
		public static implicit operator Vector3(BulletSharp.Vector3 value)
		{
			return new Vector3(value.X,value.Y,value.Z);
		}*/
	}
}

