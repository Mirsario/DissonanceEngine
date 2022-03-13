using System;

namespace Dissonance.Engine
{
	public sealed class Rand : EngineModule
	{
		private const int BufferSize = sizeof(ulong);

		[ThreadStatic] private static Random threadRandom;

		[ThreadStatic] private static byte[] threadBuffer;

		private static Random ThreadRandom => threadRandom ??= new Random((int)DateTime.Now.Ticks);
		private static byte[] ThreadBuffer => threadBuffer ??= new byte[BufferSize];

		internal Random random;

		private int seed;

		public int Seed {
			get => seed;
			set {
				seed = value;
				random = new Random(seed);
			}
		}

		// Next - 1 byte

		public static sbyte Next(sbyte maxValue)
		{
			byte[] buffer = ThreadBuffer;

			ThreadRandom.NextBytes(new Span<byte>(buffer, 0, sizeof(sbyte)));

			return (sbyte)(buffer[0] % maxValue);
		}

		public static byte Next(byte maxValue)
		{
			byte[] buffer = ThreadBuffer;

			ThreadRandom.NextBytes(new Span<byte>(buffer, 0, sizeof(byte)));

			return (byte)(buffer[0] % maxValue);
		}

		// Next - 2 bytes

		public static short Next(short maxValue)
		{
			byte[] buffer = ThreadBuffer;

			ThreadRandom.NextBytes(new Span<byte>(buffer, 0, sizeof(short)));

			return (short)(BitConverter.ToInt32(buffer, 0) % maxValue);
		}

		public static ushort Next(ushort maxValue)
		{
			byte[] buffer = ThreadBuffer;

			ThreadRandom.NextBytes(new Span<byte>(buffer, 0, sizeof(ushort)));

			return (ushort)(BitConverter.ToUInt32(buffer, 0) % maxValue);
		}

		// Next - 4 bytes

		public static int Next(int maxValue)
			=> ThreadRandom.Next(maxValue);

		public static uint Next(uint maxValue)
		{
			byte[] buffer = ThreadBuffer;

			ThreadRandom.NextBytes(new Span<byte>(buffer, 0, sizeof(uint)));

			return BitConverter.ToUInt32(buffer, 0) % maxValue;
		}

		public static float Next(float maxValue)
			=> (float)ThreadRandom.NextDouble() * maxValue;

		// Next - 8 bytes

		public static long Next(long maxValue)
		{
			byte[] buffer = ThreadBuffer;

			ThreadRandom.NextBytes(new Span<byte>(buffer, 0, sizeof(long)));

			return BitConverter.ToInt64(buffer, 0) % maxValue;
		}

		public static ulong Next(ulong maxValue)
		{
			byte[] buffer = ThreadBuffer;

			ThreadRandom.NextBytes(new Span<byte>(buffer, 0, sizeof(ulong)));

			return BitConverter.ToUInt64(buffer, 0) % maxValue;
		}

		public static double Next(double maxValue)
			=> ThreadRandom.NextDouble() * maxValue;

		// NextBytes

		public static void NextBytes(byte[] buffer)
			=> ThreadRandom.NextBytes(buffer);

		// Range
		public static int Range(int minValue, int maxValue)
			=> ThreadRandom.Next(minValue, maxValue);

		public static float Range(float minValue, float maxValue)
		{
			if (minValue > maxValue) {
				float tempVal = maxValue;

				maxValue = minValue;
				minValue = tempVal;
			}

			return minValue + (float)ThreadRandom.NextDouble() * (maxValue - minValue);
		}
	}
}

