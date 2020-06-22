using System;
using System.Runtime.CompilerServices;

namespace Dissonance.Engine.Core
{
	public static partial class Mathf
	{
		[MethodImpl(Inline)] public static int Abs(int value) => Math.Abs(value);
		[MethodImpl(Inline)] public static float Abs(float f) => Math.Abs(f);
		[MethodImpl(Inline)] public static double Abs(double d) => Math.Abs(d);

		[MethodImpl(Inline)] public static int Sign(int i) => Math.Sign(i);
		[MethodImpl(Inline)] public static float Sign(float f) => Math.Sign(f);
		[MethodImpl(Inline)] public static double Sign(double d) => Math.Sign(d);

		[MethodImpl(Inline)] public static float Sin(float f) => (float)Math.Sin(f);
		[MethodImpl(Inline)] public static double Sin(double d) => Math.Sin(d);

		[MethodImpl(Inline)] public static float Cos(float f) => (float)Math.Cos(f);
		[MethodImpl(Inline)] public static double Cos(double d) => Math.Cos(d);

		[MethodImpl(Inline)] public static float Tan(float f) => (float)Math.Tan(f);
		[MethodImpl(Inline)] public static double Tan(double d) => Math.Tan(d);

		[MethodImpl(Inline)] public static float Asin(float f) => (float)Math.Asin(f);
		[MethodImpl(Inline)] public static double Asin(double d) => Math.Asin(d);

		[MethodImpl(Inline)] public static float Acos(float f) => (float)Math.Acos(f);
		[MethodImpl(Inline)] public static double Acos(double d) => Math.Acos(d);

		[MethodImpl(Inline)] public static float Atan(float v) => (float)Math.Atan(v);
		[MethodImpl(Inline)] public static double Atan(double d) => Math.Atan(d);

		[MethodImpl(Inline)] public static float Atan2(float y,float x) => (float)Math.Atan2(y,x);
		[MethodImpl(Inline)] public static double Atan2(double y,double x) => Math.Atan2(y,x);

		[MethodImpl(Inline)] public static float Sqrt(float f) => (float)Math.Sqrt(f);
		[MethodImpl(Inline)] public static double Sqrt(double d) => Math.Sqrt(d);

		[MethodImpl(Inline)] public static float SqrtReciprocal(float f) => 1f/Sqrt(f);
		[MethodImpl(Inline)] public static double SqrtReciprocal(double d) => 1d/Sqrt(d);

		[MethodImpl(Inline)] public static float Pow(float f,float p) => (float)Math.Pow(f,p);
		[MethodImpl(Inline)] public static double Pow(double d,double p) => Math.Pow(d,p);

		[MethodImpl(Inline)] public static float Exp(float power) => (float)Math.Exp(power);
		[MethodImpl(Inline)] public static double Exp(double power) => Math.Exp(power);

		[MethodImpl(Inline)] public static float Log(float f) => (float)Math.Log(f);
		[MethodImpl(Inline)] public static double Log(double d) => Math.Log(d);

		[MethodImpl(Inline)] public static float Log(float f,float p) => (float)Math.Log(f,p);
		[MethodImpl(Inline)] public static double Log(double d,double p) => Math.Log(d,p);

		[MethodImpl(Inline)] public static float Log10(float f) => (float)Math.Log10(f);
		[MethodImpl(Inline)] public static double Log10(double d) => Math.Log10(d);

		[MethodImpl(Inline)] public static float Dot(float aX,float aY,float bX,float bY) => aX*bX+aY*bY;
		[MethodImpl(Inline)] public static double Dot(double aX,double aY,double bX,double bY) => aX*bX+aY*bY;
	}
}