using System;
using Dissonance.Engine.Core;
using Dissonance.Engine.Core.Modules;

namespace Dissonance.Engine.Physics
{
	public sealed class Layers : EngineModule
	{
		public const int MaxLayers = sizeof(ulong)*8;

		internal static Layers Instance => Game.Instance.GetModule<Layers>(true);

		private ulong[] indexToMask;
		private string[] indexToName;
		private int layerCount;

		protected override void PreInit()
		{
			indexToMask = new ulong[MaxLayers];
			indexToName = new string[MaxLayers];

			AddLayers("Default");
		}

		public static void AddLayers<T>() where T : Enum => AddLayers(Enum.GetNames(typeof(T)));
		public static void AddLayers(params string[] layerNames)
		{
			if(Game.Instance?.preInitDone!=false) {
				throw new Exception("Cannot add layers after Game.PreInit() call has finished.");
			}

			var instance = Instance;
			int newLength = instance.layerCount+layerNames.Length;

			if(newLength>=64) {
				throw new Exception("Cannot add more than 64 layers.");
			}

			for(int i = 0,j = instance.layerCount;i<layerNames.Length;i++,j++) {
				instance.indexToMask[j] = (ulong)1 << j;
				instance.indexToName[j] = layerNames[i];
			}

			instance.layerCount += layerNames.Length;
		}

		public static ulong GetLayerMask(int index)
		{
			var instance = Instance;

			if(index<0 || index>=instance.layerCount) {
				throw new IndexOutOfRangeException();
			}

			return instance.indexToMask[index];
		}
		public static ulong GetLayerMask(string name)
		{
			var instance = Instance;

			for(int i = 0;i<instance.layerCount;i++) {
				if(instance.indexToName[i]==name) {
					return instance.indexToMask[i];
				}
			}

			throw new Exception($"Could not find any layers named '{name}'.");
		}

		public static string GetLayerName(int index)
		{
			var instance = Instance;

			if(index<0 || index>=instance.layerCount) {
				throw new IndexOutOfRangeException();
			}

			return instance.indexToName[index];
		}
		public static string GetLayerName(ulong flag)
		{
			var instance = Instance;

			for(int i = 0;i<instance.layerCount;i++) {
				if(instance.indexToMask[i]==flag) {
					return instance.indexToName[i];
				}
			}

			throw new Exception($"Could not find any layers with flag '{flag}'.");
		}

		public static byte GetLayerIndex(string name)
		{
			var instance = Instance;

			for(int i = 0;i<instance.layerCount;i++) {
				if(instance.indexToName[i]==name) {
					return (byte)i;
				}
			}

			throw new Exception($"Could not find any layers named '{name}'.");
		}
		public static byte GetLayerIndex(ulong flag)
		{
			var instance = Instance;

			for(int i = 0;i<instance.layerCount;i++) {
				if(instance.indexToMask[i]==flag) {
					return (byte)i;
				}
			}

			throw new Exception($"Could not find any layers with flag '{flag}'.");
		}
	}
}
