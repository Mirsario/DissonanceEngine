using System;

namespace Dissonance.Engine;

public class Gradient<T>
{
	public struct GradientKey
	{
		public float Time;
		public T Value;

		public GradientKey(float time, T value)
		{
			Time = time;
			Value = value;
		}
	}

	public static Func<T, T, float, T> LerpFunc { protected get; set; }

	public GradientKey[] keys;

	static Gradient()
	{
		Gradient<float>.LerpFunc = MathHelper.Lerp;
		Gradient<double>.LerpFunc = (left, right, t) => MathHelper.Lerp(left, right, t);

		Gradient<sbyte>.LerpFunc = (left, right, t) => (sbyte)Math.Round(MathHelper.Lerp(left, right, t));
		Gradient<byte>.LerpFunc = (left, right, t) => (byte)Math.Round(MathHelper.Lerp(left, right, t));
		Gradient<short>.LerpFunc = (left, right, t) => (short)Math.Round(MathHelper.Lerp(left, right, t));
		Gradient<ushort>.LerpFunc = (left, right, t) => (ushort)Math.Round(MathHelper.Lerp(left, right, t));
		Gradient<int>.LerpFunc = (left, right, t) => (int)Math.Round(MathHelper.Lerp(left, right, t));
		Gradient<uint>.LerpFunc = (left, right, t) => (uint)Math.Round(MathHelper.Lerp(left, right, t));
		Gradient<long>.LerpFunc = (left, right, t) => (long)Math.Round(MathHelper.Lerp(left, right, t));
		Gradient<ulong>.LerpFunc = (left, right, t) => (ulong)Math.Round(MathHelper.Lerp(left, right, t));

		Gradient<Vector2>.LerpFunc = Vector2.Lerp;
		Gradient<Vector3>.LerpFunc = Vector3.Lerp;
		Gradient<Vector4>.LerpFunc = Vector4.Lerp;
	}

	public Gradient(float[] positions, T[] values)
	{
		if (LerpFunc == null) {
			throw new NotSupportedException($"Gradient<{typeof(T).Name}>.lerpFunc is not defined.");
		}

		if (positions.Length != values.Length || positions.Length == 0) {
			throw new ArgumentException("Array lengths must be equal and not be zero.");
		}

		keys = new GradientKey[positions.Length];

		for (int i = 0; i < keys.Length; i++) {
			keys[i] = new GradientKey(positions[i], values[i]);
		}
	}

	public T GetValue(float time)
	{
		GradientKey left = default;
		GradientKey right = default;
		bool leftHasValue = false;
		bool rightHasValue = false;

		for (int i = 0; i < keys.Length; i++) {
			if (!leftHasValue || keys[i].Time > left.Time && keys[i].Time <= time) {
				left = keys[i];
				leftHasValue = true;
			}
		}

		for (int i = keys.Length - 1; i >= 0; i--) {
			if (!rightHasValue || keys[i].Time < right.Time && keys[i].Time >= time) {
				right = keys[i];
				rightHasValue = true;
			}
		}

		return left.Time == right.Time
			? left.Value
			: LerpFunc(left.Value, right.Value, (time - left.Time) / (right.Time - left.Time));
	}
}
