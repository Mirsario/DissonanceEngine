using System;
using System.Collections.Generic;

namespace Dissonance.Engine.IO
{
	internal static class AssetLookup
	{
		public static event Action OnClear;

		public static void Clear() => OnClear?.Invoke();
	}

	public static class AssetLookup<T>
	{
		private static readonly Dictionary<string, Asset<T>> lookup = new();

		public static int Count => lookup.Count;

		static AssetLookup()
		{
			AssetLookup.OnClear += () => {
				lookup.Clear();
			};
		}

		public static void Register(string name, Asset<T> asset)
		{
			if (lookup.ContainsKey(name)) {
				throw new Exception($"Cannot register two {typeof(T).Name} with the same name: {name}.");
			}

			lookup[name] = asset;
		}

		internal static Asset<T> Get(string fullName)
			=> lookup[fullName];

		internal static bool TryGetValue(string fullName, out Asset<T> value)
			=> lookup.TryGetValue(fullName, out value);
	}
}
