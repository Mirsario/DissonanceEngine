using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Matrix3 = OpenTK.Matrix3;

namespace GameEngine
{
	public struct Quaternion
	{
		public const float kEpsilon = 1E-06f;
		public float x;
		public float y;
		public float z;
		public float w;
		public float this[int index] {
			get {
				switch(index) {
					case 0:
						return x;
					case 1:
						return y;
					case 2:
						return z;
					case 3:
						return w;
					default:
						throw new IndexOutOfRangeException("Quaternion has values ranging from 0 to 3");
				}
			}
			set
			{
				switch(index) {
					case 0:
						x = value;
						return;
					case 1:
						y = value;
						return;
					case 2:
						z = value;
						return;
					case 3:
						w = value;
						return;
					default:
						throw new IndexOutOfRangeException("Quaternion has values ranging from 0 to 3");
				}
			}
		}
		public static Quaternion Identity	=> new Quaternion(0f,0f,0f,1f);
		public float Magnitude				=> Mathf.Sqrt(w*w+x*x+y*y+z*z);
		public float SqrMagnitude			=> w*w+x*x+y*y+z*z;
		public Quaternion Normalized {
			get {
				var quaternion = this;
				quaternion.Normalize();
				return quaternion;
			}
		}
		public Quaternion Inverted {
			get {
				var quaternion = this;
				quaternion.w = -quaternion.w;
				return quaternion;
			}
		}

		#region Constructors
		public Quaternion(float X,float Y,float Z,float W)
		{
			x = X;
			y = Y;
			z = Z;
			w = W;
		}
		
		public static Quaternion FromDirection(Vector3 direction,Vector3 up)
		{
			return Matrix4x4.LookAt(Vector3.zero,-direction,up).ExtractQuaternion();
			/*direction.Normalize();
			
			Vector3 vector2 = Vector3.Cross(up,direction).normalized;
			Vector3 vector3 = Vector3.Cross(direction,vector2);
 
			float num8 = (vector2.x+vector3.y)+direction.z;
			var quaternion = new Quaternion();
			if(num8>0.0) {
				var num = Mathf.Sqrt(num8+1f);
				quaternion.w = num*0.5f;
				num = 0.5f/num;
				quaternion.x = (vector3.z-direction.y)*num;
				quaternion.y = (direction.x-vector2.z)*num;
				quaternion.z = (vector2.y-vector3.x)*num;
				return quaternion;
			}
			if((vector2.x>=vector3.y)&&(vector2.x>=direction.z)) {
				var num7 = Mathf.Sqrt(((1f+vector2.x)-vector3.y)-direction.z);
				var num4 = 0.5f/num7;
				quaternion.x = 0.5f*num7;
				quaternion.y = (vector2.y+vector3.x)*num4;
				quaternion.z = (vector2.z+direction.x)*num4;
				quaternion.w = (vector3.z-direction.y)*num4;
				return quaternion;
			}
			if(vector3.y>direction.z) {
				var num6 = Mathf.Sqrt(((1f+vector3.y)-vector2.x)-direction.z);
				var num3 = 0.5f/num6;
				quaternion.x = (vector3.x+vector2.y)*num3;
				quaternion.y = 0.5f*num6;
				quaternion.z = (direction.y+vector3.z)*num3;
				quaternion.w = (direction.x-vector2.z)*num3;
				return quaternion;
			}
			var num5 = Mathf.Sqrt(((1f+direction.z)-vector2.x)-vector3.y);
			var num2 = 0.5f/num5;
			quaternion.x = (direction.x+vector2.z)*num2;
			quaternion.y = (direction.y+vector3.z)*num2;
			quaternion.z = 0.5f*num5;
			quaternion.w = (vector2.y-vector3.x)*num2;
			return quaternion;*/
		}
		public static Quaternion FromEuler(Vector3 euler)
		{
			euler *= 0.5f;

			float cX = (float)Math.Cos(euler.x);
			float cY = (float)Math.Cos(euler.y);
			float cZ = (float)Math.Cos(euler.z);
			float sX = (float)Math.Sin(euler.x);
			float sY = (float)Math.Sin(euler.y);
			float sZ = (float)Math.Sin(euler.z);

			return new Quaternion(
				sX*sY*cZ+cX*cY*sZ,
				sX*cY*cZ+cX*sY*sZ,
				cX*sY*cZ-sX*cY*sZ,	
				cX*cY*cZ-sX*sY*sZ
			);
		}
		#endregion

		public void Normalize()
		{
			float n = x*x+y*y+z*z+w*w;
			if(n==1f) {
				return;
			}
			float sqrtR = Mathf.SqrtReciprocal(n);
			x *= sqrtR;
			y *= sqrtR;
			z *= sqrtR;
			w *= sqrtR;
		}
		public void Invert()
		{
			w = -w;
		}
		private bool EqualTest(float a,float b,float tolerance)
		{
			return a+tolerance>=b && a-tolerance<=b;
		}
		public Vector3 ToEuler()
		{
			float sqX = x*x;
			float sqY = y*y;
			float sqZ = z*z;
			float sqW = w*w;
			float test = 2f*(y*w-x*z);
			
			Vector3 result;
			if(EqualTest(test,1f,0.000001f)) {
			//if(test==1f) {
				result.z = -2f*Mathf.Atan2(x,w);//heading = rotation about z-axis
				result.x = 0f;//bank = rotation about x-axis
				result.y = Mathf.PI/2f;//attitude = rotation about y-axis
			}else if(EqualTest(test,-1f,0.000001f)) {
			//}else if(test==-1f) {
				result.z = 2f*Mathf.Atan2(x,w);//heading = rotation about z-axis
				result.x = 0f;//bank = rotation about x-axis
				result.y = Mathf.PI/-2f;//attitude = rotation about y-axis
			}else{
				result.z = Mathf.Atan2(2f*(x*y+z*w),sqX-sqY-sqZ+sqW);//heading = rotation about z-axis
				result.x = Mathf.Atan2(2f*(y*z+x*w),-sqX-sqY+sqZ+sqW);//bank = rotation about x-axis
				result.y = Mathf.Asin(Mathf.Clamp(test,-1f,1f));//attitude = rotation about y-axis
			}
			result *= Mathf.Rad2Deg;
			result.NormalizeEuler();
			//result.NormalizeEuler();
			return result;
		}
		/*public Vector3 ToEuler()
		{
			Quaternion q = this;
			Vector3 result;
			
			float ysqr = q.y*q.y;
			float t0 = -2f*(ysqr+q.z*q.z)+1f;
			float t1 = 2f*(q.x*q.y-q.w*q.z);
			float t2 = -2f*(q.x*q.z+q.w*q.y);
			float t3 = 2f*(q.y*q.z-q.w*q.x);
			float t4 = -2f*(q.x*q.x+ysqr)+1f;
			
			t2 = t2 > 1f ? 1f : t2;
			t2 = t2 <-1f ?-1f : t2;
			
			result.x = Mathf.Asin(t2);
			result.z = Mathf.Atan2(t3,t4);
			result.y = Mathf.Atan2(t1,t0);
			
			return (result*Mathf.Rad2Deg);//*0.9999998f;
		}*/
		public void ToAxisAngle(out Vector3 axis,out float angle)
		{
			var result = ToAxisAngle();
			axis = (Vector3)result;
			angle = result.w;
		}
		public Vector4 ToAxisAngle()
		{
			if(Mathf.Abs(w)>1f) {
				Normalize();
			}
			var result = new Vector4 {
				w = 2f*Mathf.Acos(w)
			};
			float den = Mathf.Sqrt(1f-w*w);
			if(den>0.0001f) {
				result.x = x/den;
				result.y = y/den;
				result.z = z/den;
			}else{
				result.x = 1f;
				result.y = 0f;
				result.z = 0f;
			}
			return result;
		}
		
		public static Quaternion Normalize(Quaternion quaternion)
		{
			quaternion.Normalize();
			return quaternion;
		}
		public static Quaternion Invert(Quaternion quaternion)
		{
			quaternion.w = -quaternion.w;
			return quaternion;
		}
		public override string ToString()
		{
			return "["+x.ToString()+","+y.ToString()+","+z.ToString()+","+w.ToString()+"]";
		}
		
		public static Vector3 operator*(Quaternion rotation,Vector3 point)
		{
			float num1 = rotation.x*2f;
			float num2 = rotation.y*2f;
			float num3 = rotation.z*2f;
			float num4 = rotation.x*num1;
			float num5 = rotation.y*num2;
			float num6 = rotation.z*num3;
			float num7 = rotation.x*num2;
			float num8 = rotation.x*num3;
			float num9 = rotation.y*num3;
			float num10 = rotation.w*num1;
			float num11 = rotation.w*num2;
			float num12 = rotation.w*num3;
			Vector3 result;
			result.x = (1f-(num5+num6))*point.x+(num7-num12)*point.y+(num8+num11)*point.z;
			result.y = (num7+num12)*point.x+(1f-(num4+num6))*point.y+(num9-num10)*point.z;
			result.z = (num8-num11)*point.x+(num9+num10)*point.y+(1f-(num4+num5))*point.z;
			return result;
		}
		public static Quaternion operator*(Quaternion q,Quaternion other)
		{
			Quaternion result;
			result.x = other.w*q.x+other.x*q.w+other.y*q.z-other.z*q.y;
			result.y = other.w*q.y+other.y*q.w+other.z*q.x-other.x*q.z;
			result.z = other.w*q.z+other.z*q.w+other.x*q.y-other.y*q.x;
			result.w = other.w*q.w-other.x*q.x-other.y*q.y-other.z*q.z;
			return result;
		}
		public static Quaternion operator*(Quaternion q,float s)
		{
			return new Quaternion(s*q.x,s*q.y,s*q.z,s*q.w);
		}
		public static Quaternion operator*(float s,Quaternion q)
		{
			return new Quaternion(s*q.x,s*q.y,s*q.z,s*q.w);
		}
		public static bool operator==(Quaternion a,Quaternion b)
		{
			return a.x==b.x && a.y==b.y && a.z==b.z && a.w==b.w;
		}
		public static bool operator!=(Quaternion a,Quaternion b)
		{
			return a.x!=b.x || a.y!=b.y || a.z!=b.z || a.w!=b.w;
		}
		public override int GetHashCode()
		{
			return x.GetHashCode()^y.GetHashCode()<<2^z.GetHashCode()>>2^w.GetHashCode()>>1;
		}
		public override bool Equals(object other)
		{
			if(!(other is Quaternion)) {
				return false;
			}
			var q = (Quaternion)other;
			return x.Equals(q.x) && y.Equals(q.y) && z.Equals(q.z) && w.Equals(q.w);
		}

		public static implicit operator OpenTK.Quaternion(Quaternion value)
		{
			return new OpenTK.Quaternion(value.x,value.y,value.z,value.w);
		}
		public static implicit operator Quaternion(OpenTK.Quaternion value)
		{
			return new Quaternion(value.X,value.Y,value.Z,value.W);
		}
	}
}

