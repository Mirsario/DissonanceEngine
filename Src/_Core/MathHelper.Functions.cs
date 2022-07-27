using System;

namespace Dissonance.Engine;

partial class MathHelper
{
	// Clamp

	public static int Clamp(int value, int min, int max)
		=> value < min ? min : value > max ? max : value;

	public static float Clamp(float value, float min, float max)
		=> value < min ? min : value > max ? max : value;

	public static double Clamp(double value, double min, double max)
		=> value < min ? min : value > max ? max : value;

	public static float Clamp01(float value)
		=> value < 0f ? 0f : value > 1f ? 1f : value;

	public static double Clamp01(double value)
		=> value < 0d ? 0d : value > 1d ? 1d : value;

	// Repeat

	public static int Repeat(int value, int length)
	{
		int r = value % length;

		return r < 0 ? r + length : r;
	}

	public static float Repeat(float value, float length)
		=> value - MathF.Floor(value / length) * length;

	public static double Repeat(double value, double length)
		=> value - Math.Floor(value / length) * length;

	// Snap

	public static float SnapToGrid(float val, float step)
		=> MathF.Ceiling(val / step) * step;

	public static double SnapToGrid(double val, double step)
		=> Math.Ceiling(val / step) * step;

	// NormalizeEuler

	public static float NormalizeEuler(float angle)
		=> angle - MathF.Floor(angle / 360f) * 360f;

	public static double NormalizeEuler(double angle)
		=> angle - Math.Floor(angle / 360d) * 360d;

	// StepTowards

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
