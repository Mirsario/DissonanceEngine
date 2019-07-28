using System;

namespace GameEngine
{
	public class Rand
	{
		internal static Random staticRandom;
		internal Random random;
		private static int globalSeed;
		public static int GlobalSeed {
			get => globalSeed;
			set {
				globalSeed = value;
				staticRandom = new Random(globalSeed);
			}
		}
		
		private int seed;
		public int Seed {
			get => seed;
			set {
				seed = value;
				random = new Random(seed);
			}
		}
		
		public Rand(int seed)
		{
			this.seed = seed;
			random = new Random(seed);
		}
		public float NextFloat(float minValue,float maxValue)
		{
			if(minValue>maxValue) {
				float tempVal = maxValue;
				maxValue = minValue;
				minValue = tempVal;
			}
			return minValue+(float)random.NextDouble()*(maxValue-minValue);
		}
		public int NextInt(int minValue,int maxValue)
		{
			if(minValue>maxValue) {
				int tempVal = maxValue;
				maxValue = minValue;
				minValue = tempVal;
			}
			return random.Next(minValue,maxValue);
		}
		
		public static void Init()
		{
			globalSeed = (int)DateTime.Now.Ticks;
			staticRandom = new Random(globalSeed);
		}
		public static int Next(int maxValue)
		{
			return staticRandom.Next(maxValue);
		}
		public static float Next(float maxValue)
		{
			return (float)staticRandom.NextDouble()*maxValue;
		}
		public static float Range(float minValue,float maxValue)
		{
			if(minValue>maxValue) {
				float tempVal = maxValue;
				maxValue = minValue;
				minValue = tempVal;
			}
			return minValue+(float)staticRandom.NextDouble()*(maxValue-minValue);
		}
		public static int Range(int minValue,int maxValue)
		{
			return staticRandom.Next(minValue,maxValue);
		}
	}
}

