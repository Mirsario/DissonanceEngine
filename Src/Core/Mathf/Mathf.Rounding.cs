using System;
using System.Runtime.CompilerServices;

namespace Dissonance.Engine
{
	partial class Mathf
	{
		[MethodImpl(Inline)] public static float Ceil(float f) => (float)Math.Ceiling(f);
		[MethodImpl(Inline)] public static double Ceil(double d) => Math.Ceiling(d);

		[MethodImpl(Inline)] public static float Floor(float f) => (float)Math.Floor(f);
		[MethodImpl(Inline)] public static double Floor(double d) => Math.Floor(d);

		[MethodImpl(Inline)] public static float Round(float f) => (float)Math.Round(f);
		[MethodImpl(Inline)] public static double Round(double d) => Math.Round(d);

		[MethodImpl(Inline)] public static int CeilToInt(float f) => (int)Math.Ceiling(f);
		[MethodImpl(Inline)] public static int CeilToInt(double d) => (int)Math.Ceiling(d);

		[MethodImpl(Inline)] public static int FloorToInt(float f) => (int)Math.Floor(f);
		[MethodImpl(Inline)] public static int FloorToInt(double d) => (int)Math.Floor(d);

		[MethodImpl(Inline)] public static int RoundToInt(float f) => (int)Math.Round(f);
		[MethodImpl(Inline)] public static int RoundToInt(double d) => (int)Math.Round(d);
	}
}