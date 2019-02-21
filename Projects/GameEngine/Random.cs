using System;

namespace GameEngine
{
	public class Rand
	{
		internal static Random staticRandom;
		internal Random random;
		private static int _globalSeed;
		public static int GlobalSeed {
			get => _globalSeed;
			set {
				_globalSeed = value;
				staticRandom = new Random(_globalSeed);
			}
		}
		
		private int _seed;
		public int Seed {
			get => _seed;
			set {
				_seed = value;
				random = new Random(_seed);
			}
		}
		
		public Rand(int seed)
		{
			_seed = seed;
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
			_globalSeed = (int)DateTime.Now.Ticks;
			staticRandom = new Random(_globalSeed);
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

