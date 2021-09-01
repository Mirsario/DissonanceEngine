using System.Runtime.CompilerServices;

namespace Dissonance.Engine
{
	partial class Mathf
	{
		[MethodImpl(Inline)]
		public static int Repeat(int value, int length)
		{
			int r = value % length;

			return r < 0 ? r + length : r;
		}

		[MethodImpl(Inline)] public static float Repeat(float value, float length) => value - Floor(value / length) * length;

		[MethodImpl(Inline)] public static double Repeat(double value, double length) => value - Floor(value / length) * length;

		[MethodImpl(Inline)] public static float SnapToGrid(float val, float step) => Ceil(val / step) * step;

		[MethodImpl(Inline)] public static double SnapToGrid(double val, double step) => Ceil(val / step) * step;

		[MethodImpl(Inline)] public static float NormalizeEuler(float angle) => angle - Floor(angle / 360f) * 360f;

		[MethodImpl(Inline)] public static double NormalizeEuler(double angle) => angle - Floor(angle / 360d) * 360d;

		[MethodImpl(Inline)]
		public static float StepTowards(float val, float goal, float step)
		{
			if (goal > val) {
				val += step;

				if (val > goal) {
					return goal;
				}
			} else if (goal < val) {
				val -= step;

				if (val < goal) {
					return goal;
				}
			}

			return val;
		}

		[MethodImpl(Inline)]
		public static double StepTowards(double val, double goal, double step)
		{
			if (goal > val) {
				val += step;

				if (val > goal) {
					return goal;
				}
			} else if (goal < val) {
				val -= step;

				if (val < goal) {
					return goal;
				}
			}

			return val;
		}
	}
}
