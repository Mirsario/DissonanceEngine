using System;

namespace GameEngine
{
	public static class Layers
	{
		private static ulong[] indexToMask;
		private static string[] indexToName;
		private static int layerCount;

		internal static void Init()
		{
			indexToMask = new ulong[64];
			indexToName = new string[64];
			AddLayers("Default");
		}

		public static void AddLayers(params string[] layerNames)
		{
			if(Game.preInitDone) {
				throw new Exception("Cannot add layers after Game.PreInit() call has finished.");
			}

			int newLength = layerCount+layerNames.Length;
			if(newLength>=64) {
				throw new Exception("Cannot add more than 64 layers.");
			}

			int j = layerCount;
			for(int i=0;i<layerNames.Length;i++,j++) {
				indexToMask[j] = (ulong)1 << j;
				indexToName[j] = layerNames[i];
			}
			layerCount += layerNames.Length;
		}
		/*public static void AddLayers(Type enumType)
		{
			if(!enumType.IsEnum) {
				throw new Exception("This overload expects an Enum type as parameter.");
			}
			AddLayers(Enum.GetNames(enumType));
		}*/

		public static ulong GetLayerMask(int index)
		{
			if(index<0 || index>=layerCount) {
				throw new IndexOutOfRangeException();
			}
			return indexToMask[index];
		}
		public static ulong GetLayerMask(string name)
		{
			for(int i=0;i<layerCount;i++) {
				if(indexToName[i]==name) {
					return indexToMask[i];
				}
			}

			throw new Exception("Could not find any layers named ''"+name+"''");
		}

		public static string GetLayerName(int index)
		{
			if(index<0 || index>=layerCount) {
				throw new IndexOutOfRangeException();
			}
			return indexToName[index];
		}
		public static string GetLayerName(ulong flag)
		{
			for(int i=0;i<layerCount;i++) {
				if(indexToMask[i]==flag) {
					return indexToName[i];
				}
			}
			throw new Exception("Could not find any layers with flag ''"+flag+"''");
		}
		
		public static byte GetLayerIndex(string name)
		{
			for(int i=0;i<layerCount;i++) {
				if(indexToName[i]==name) {
					return (byte)i;
				}
			}
			throw new Exception("Could not find any layers named ''"+name+"''");
		}
		public static byte GetLayerIndex(ulong flag)
		{
			for(int i=0;i<layerCount;i++) {
				if(indexToMask[i]==flag) {
					return (byte)i;
				}
			}
			throw new Exception("Could not find any layers with flag ''"+flag+"''");
		}
	}
}
