using System;
using System.Runtime.CompilerServices;

namespace Dissonance.Engine.Core
{
	partial class Mathf
	{
		[MethodImpl(Inline)] public static int Min(int a,int b) => Math.Min(a,b);
		[MethodImpl(Inline)] public static float Min(float a,float b) => Math.Min(a,b);
		[MethodImpl(Inline)] public static double Min(double a,double b) => Math.Min(a,b);

		[MethodImpl(Inline)] public static int Max(int a,int b) => Math.Max(a,b);
		[MethodImpl(Inline)] public static float Max(float a,float b) => Math.Max(a,b);
		[MethodImpl(Inline)] public static double Max(double a,double b) => Math.Max(a,b);

		public static int Min(params int[] values)
		{
			if(values.Length==0) {
				return 0;
			}

			int num = values[0];

			for(int i = 1;i<values.Length;i++) {
				if(values[i]<num) {
					num = values[i];
				}
			}

			return num;
		}
		public static float Min(params float[] values)
		{
			if(values.Length==0) {
				return 0f;
			}

			float num = values[0];

			for(int i = 1;i<values.Length;i++) {
				if(values[i]<num) {
					num = values[i];
				}
			}

			return num;
		}
		public static double Min(params double[] values)
		{
			if(values.Length==0) {
				return 0d;
			}

			double num = values[0];

			for(int i = 1;i<values.Length;i++) {
				if(values[i]<num) {
					num = values[i];
				}
			}

			return num;
		}

		public static int Max(params int[] values)
		{
			if(values.Length==0) {
				return 0;
			}

			int num = values[0];

			for(int i = 1;i<values.Length;i++) {
				if(values[i]>num) {
					num = values[i];
				}
			}

			return num;
		}
		public static float Max(params float[] values)
		{
			if(values.Length==0) {
				return 0f;
			}

			float num = values[0];

			for(int i = 1;i<values.Length;i++) {
				if(values[i]>num) {
					num = values[i];
				}
			}

			return num;
		}
		public static double Max(params double[] values)
		{
			if(values.Length==0) {
				return 0d;
			}

			double num = values[0];

			for(int i = 1;i<values.Length;i++) {
				if(values[i]>num) {
					num = values[i];
				}
			}

			return num;
		}
	}
}