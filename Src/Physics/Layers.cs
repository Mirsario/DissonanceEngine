using System;

namespace Dissonance.Engine.Physics
{
	public sealed class Layers : EngineModule
	{
		public const int MaxLayers = sizeof(ulong) * 8;

		private static ulong[] indexToMask;
		private static string[] indexToName;
		private static int layerCount;

		protected override void PreInit()
		{
			indexToMask = new ulong[MaxLayers];
			indexToName = new string[MaxLayers];

			AddLayers("Default");
		}

		public static void AddLayers<T>() where T : Enum => AddLayers(Enum.GetNames(typeof(T)));
		public static void AddLayers(params string[] layerNames)
		{
			if(Game.Instance?.preInitDone != false) {
				throw new Exception("Cannot add layers after Game.PreInit() call has finished.");
			}

			int newLength = layerCount + layerNames.Length;

			if(newLength >= 64) {
				throw new Exception("Cannot add more than 64 layers.");
			}

			for(int i = 0, j = layerCount; i < layerNames.Length; i++, j++) {
				indexToMask[j] = (ulong)1 << j;
				indexToName[j] = layerNames[i];
			}

			layerCount += layerNames.Length;
		}

		public static ulong GetLayerMask(int index)
		{
			if(index < 0 || index >= layerCount) {
				throw new IndexOutOfRangeException();
			}

			return indexToMask[index];
		}
		public static ulong GetLayerMask(string name)
		{
			for(int i = 0; i < layerCount; i++) {
				if(indexToName[i] == name) {
					return indexToMask[i];
				}
			}

			throw new Exception($"Could not find any layers named '{name}'.");
		}

		public static string GetLayerName(int index)
		{
			if(index < 0 || index >= layerCount) {
				throw new IndexOutOfRangeException();
			}

			return indexToName[index];
		}
		public static string GetLayerName(ulong flag)
		{
			for(int i = 0; i < layerCount; i++) {
				if(indexToMask[i] == flag) {
					return indexToName[i];
				}
			}

			throw new Exception($"Could not find any layers with flag '{flag}'.");
		}

		public static byte GetLayerIndex(string name)
		{
			for(int i = 0; i < layerCount; i++) {
				if(indexToName[i] == name) {
					return (byte)i;
				}
			}

			throw new Exception($"Could not find any layers named '{name}'.");
		}
		public static byte GetLayerIndex(ulong flag)
		{
			for(int i = 0; i < layerCount; i++) {
				if(indexToMask[i] == flag) {
					return (byte)i;
				}
			}

			throw new Exception($"Could not find any layers with flag '{flag}'.");
		}
	}
}
