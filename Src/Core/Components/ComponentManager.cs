using System;
using Dissonance.Engine.Utilities;

namespace Dissonance.Engine
{
	public class ComponentManager : EngineModule
	{
		private static class ComponentData<T> where T : struct, IComponent
		{
			public static T[] data = Array.Empty<T>();
			public static int[] entityRemapping = Array.Empty<int>();
		}

		internal static bool HasComponent<T>(int entityId) where T : struct, IComponent
		{
			return entityId < ComponentData<T>.entityRemapping.Length && ComponentData<T>.entityRemapping[entityId] != -1;
		}

		internal static ref T GetComponent<T>(int entityId) where T : struct, IComponent
		{
			return ref ComponentData<T>.data[ComponentData<T>.entityRemapping[entityId]];
		}

		internal static void SetComponent<T>(int entityId, T value) where T : struct, IComponent
		{
			int dataId;

			if(ComponentData<T>.entityRemapping.Length <= entityId) {
				ArrayUtils.ResizeAndFillArray(ref ComponentData<T>.entityRemapping, entityId + 1, -1);

				dataId = ComponentData<T>.data.Length;
				ComponentData<T>.entityRemapping[entityId] = dataId;

				Array.Resize(ref ComponentData<T>.data, dataId + 1);
			} else {
				dataId = ComponentData<T>.entityRemapping[entityId];
			}

			ComponentData<T>.data[dataId] = value;
		}
	}
}
