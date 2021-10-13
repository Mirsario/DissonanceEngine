using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Dissonance.Engine.IO
{
	internal static class AssetLookup
	{
		public static event Action OnClear;

		public static void Clear() => OnClear?.Invoke();
	}

	internal static class AssetLookup<T>
	{
		private static readonly ConcurrentDictionary<string, Asset<T>> lookup = new();

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
				//throw new Exception($"Cannot register two {typeof(T).Name} with the same name: {name}.");

				asset = null; // This marks the registry as ambiguous.
			}

			lookup[name] = asset;
		}

		public static Asset<T> Get(string fullName)
			=> lookup[fullName] ?? throw new ArgumentException($"Key '{fullName}' is ambiguous.");

		public static bool TryGetValue(string fullName, out Asset<T> value)
			=> lookup.TryGetValue(fullName, out value) && value != null;
	}
}
