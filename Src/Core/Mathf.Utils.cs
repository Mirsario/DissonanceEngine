using System.Runtime.CompilerServices;

namespace Dissonance.Engine.Core
{
	partial class Mathf
	{
		[MethodImpl(Inline)] public static int Repeat(int t,int length) => t>=0 ? t%length : (int.MaxValue+t+1)%length;
		[MethodImpl(Inline)] public static float Repeat(float t,float length) => t-Floor(t/length)*length;
		[MethodImpl(Inline)] public static double Repeat(double t,double length) => t-Floor(t/length)*length;

		[MethodImpl(Inline)] public static float SnapToGrid(float val,float step) => Ceil(val/step)*step;
		[MethodImpl(Inline)] public static double SnapToGrid(double val,double step) => Ceil(val/step)*step;

		[MethodImpl(Inline)] public static float NormalizeEuler(float angle) => angle-Floor(angle/360f)*360f;
		[MethodImpl(Inline)] public static double NormalizeEuler(double angle) => angle-Floor(angle/360d)*360d;

		[MethodImpl(Inline)]
		public static float StepTowards(float val,float goal,float step)
		{
			if(goal>val) {
				val += step;

				if(val>goal) {
					return goal;
				}
			} else if(goal<val) {
				val -= step;

				if(val<goal) {
					return goal;
				}
			}

			return val;
		}
		[MethodImpl(Inline)]
		public static double StepTowards(double val,double goal,double step)
		{
			if(goal>val) {
				val += step;

				if(val>goal) {
					return goal;
				}
			} else if(goal<val) {
				val -= step;

				if(val<goal) {
					return goal;
				}
			}

			return val;
		}
	}
}