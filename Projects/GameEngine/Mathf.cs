using System;
using System.Runtime.CompilerServices;

namespace GameEngine
{
	public static class Mathf
	{
		//Very aggressive class, be careful
		public const float NegativeInfinity = float.NegativeInfinity;
		public const float Infinity = float.PositiveInfinity;
		public const float Epsilon = 1.401298E-45f;

		public const float FourPI = 12.56637061435917295385f;
		public const float TwoPI = 6.283185307179586476925f;
		public const float PI = 3.14159265358979323846264f;
		public const float HalfPI = 1.570796326794896619231f;
		public const float QuarterPI = 0.7853981633974483096157f;

		public const float Deg2Rad = PI/180f;
		public const float Rad2Deg = 180f/PI;

		public static float StepTowards(float val,float goal,float step)
		{
			if(goal>val) {
				val += step;

				if(val>goal) {
					return goal;
				}
			}else if(goal<val) {
				val -= step;

				if(val<goal) {
					return goal;
				}
			}

			return val;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static float SnapToGrid(float val,float step) => Ceil(val/step)*step;
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static int Repeat(int t,int length) => t>=0 ? t%length : (int.MaxValue+t+1)%length;
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Repeat(float t,float length) => t-Floor(t/length)*length;
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static float NormalizeEuler(float angle)
		{
			if(angle>360f) {
				do { angle -= 360f; } while(angle>360f);
				return angle;
			}
			while(angle<0f) {
				angle += 360f;
			}
			return angle;
		}

		//Trigonometry
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Sin(float f) => (float)Math.Sin(f);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Cos(float f) => (float)Math.Cos(f);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Tan(float f) => (float)Math.Tan(f);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Asin(float f) => (float)Math.Asin(f);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Acos(float f) => (float)Math.Acos(f);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Atan(float f) => (float)Math.Atan(f);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Atan2(float y,float x) => (float)Math.Atan2(y,x);
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Sqrt(float f) => (float)Math.Sqrt(f);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static float SqrtReciprocal(float f) => 1f/Sqrt(f); //TODO: This could be sped up?
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Abs(float f) => Math.Abs(f);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static int Abs(int value) => Math.Abs(value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Pow(float f,float p) => (float)Math.Pow(f,p);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Exp(float power) => (float)Math.Exp(power);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Log(float f,float p) => (float)Math.Log(f,p);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Log(float f) => (float)Math.Log(f);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Log10(float f) => (float)Math.Log10(f);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Dot(float[] a,float[] b) => a[0]*b[0]+a[1]*b[1];
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Sign(float f) => f<0f ? -1f : 1f;
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static int Sign(int i) => i<0 ? -1 : 1;
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static int SignWithZero(int i) => i==0 ? 0 : (i<0 ? -1 : 1);

		//Rounding
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Ceil(float f) => (float)Math.Ceiling(f);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Floor(float f) => (float)Math.Floor(f);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Round(float f) => (float)Math.Round(f);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static int CeilToInt(float f) => (int)Math.Ceiling(f);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static int FloorToInt(float f) => (int)Math.Floor(f);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static int RoundToInt(float f) => (int)Math.Round(f);

		//Clamp
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static int Clamp(int value,int min,int max) => value<min ? min : (value>max ? max : value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Clamp(float value,float min,float max) => value<min ? min : (value>max ? max : value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static double Clamp(double value,double min,double max) => value<min ? min : (value>max ? max : value);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Clamp01(float value) => value<0f ? 0f : (value>1f ? 1f : value);

		//Min/Max
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Min(float a,float b) => a>=b ? b : a;
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Min(params float[] values)
		{
			if(values.Length==0) {
				return 0f;
			}
			float num = values[0];
			for(int i=1;i<values.Length;i++) {
				if(values[i]<num) {
					num = values[i];
				}
			}
			return num;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static int Min(int a,int b) => a>=b ? b : a;
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static int Min(params int[] values)
		{
			if(values.Length==0) {
				return 0;
			}
			int num = values[0];
			for(int i=1;i<values.Length;i++) {
				if(values[i]<num) {
					num = values[i];
				}
			}
			return num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Max(float a,float b) => a<=b ? b : a;
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Max(params float[] values)
		{
			if(values.Length==0) {
				return 0f;
			}
			float num = values[0];
			for(int i=1;i<values.Length;i++) {
				if(values[i]>num) {
					num = values[i];
				}
			}
			return num;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static int Max(int a,int b) => a<=b ? b : a;
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static int Max(params int[] values)
		{
			if(values.Length==0) {
				return 0;
			}
			int num = values[0];
			for(int i=1;i<values.Length;i++) {
				if(values[i]>num) {
					num = values[i];
				}
			}
			return num;
		}

		//Interpolation
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Lerp(float a,float b,float time) => a+(b-a)*Clamp01(time);
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static float LerpAngle(float a,float b,float t)
		{
			float num = Repeat(b-a,360f);
			return a+(num>180f ? num-360f : num)*(t<0f ? 0f : (t>1f ? 1f : t));
		}
		public static float BiLerp(float valueTopLeft,float valueTopRight,float valueBottomLeft,float valueBottomRight,Vector2 topLeft,Vector2 bottomRight,Vector2 point)
		{
			float x2x1 = bottomRight.x-topLeft.x;
			float y2y1 = bottomRight.y-topLeft.y;
			float x2x = bottomRight.x-point.x;
			float y2y = bottomRight.y-point.y;
			float yy1 = point.y-topLeft.y;
			float xx1 = point.x-topLeft.x;
			return 1f/(x2x1*y2y1)*(valueBottomLeft*x2x*y2y+valueBottomRight*xx1*y2y+valueTopLeft*x2x*yy1+valueTopRight*xx1*yy1);
		}
	}
}