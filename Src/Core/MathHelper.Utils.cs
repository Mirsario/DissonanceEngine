using System;

namespace Dissonance.Engine
{
	public static partial class MathHelper
	{
		public static int Repeat(int value, int length)
		{
			int r = value % length;

			return r < 0 ? r + length : r;
		}

		public static float Repeat(float value, float length)
			=> value - MathF.Floor(value / length) * length;

		public static double Repeat(double value, double length)
			=> value - Math.Floor(value / length) * length;

		public static float SnapToGrid(float val, float step)
			=> MathF.Ceiling(val / step) * step;

		public static double SnapToGrid(double val, double step)
			=> Math.Ceiling(val / step) * step;

		public static float NormalizeEuler(float angle)
			=> angle - MathF.Floor(angle / 360f) * 360f;

		public static double NormalizeEuler(double angle)
			=> angle - Math.Floor(angle / 360d) * 360d;

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
