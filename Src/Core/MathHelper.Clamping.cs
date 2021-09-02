namespace Dissonance.Engine
{
	public static partial class MathHelper
	{
		public static sbyte Clamp(sbyte value, sbyte min, sbyte max)
			=> value < min ? min : value > max ? max : value;

		public static byte Clamp(byte value, byte min, byte max)
			=> value < min ? min : value > max ? max : value;

		public static short Clamp(short value, short min, short max)
			=> value < min ? min : value > max ? max : value;

		public static ushort Clamp(ushort value, ushort min, ushort max)
			=> value < min ? min : value > max ? max : value;

		public static int Clamp(int value, int min, int max)
			=> value < min ? min : value > max ? max : value;

		public static uint Clamp(uint value, uint min, uint max)
			=> value < min ? min : value > max ? max : value;

		public static float Clamp(float value, float min, float max)
			=> value < min ? min : value > max ? max : value;

		public static double Clamp(double value, double min, double max)
			=> value < min ? min : value > max ? max : value;

		public static float Clamp01(float value)
			=> value < 0f ? 0f : value > 1f ? 1f : value;

		public static double Clamp01(double value)
			=> value < 0d ? 0d : value > 1d ? 1d : value;
	}
}
