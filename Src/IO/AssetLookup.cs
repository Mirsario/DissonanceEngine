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
		private static readonly ConcurrentDictionary<string, (string assetPath, Asset<T> asset)> lookup = new();

		public static int Count => lookup.Count;

		static AssetLookup()
		{
			AssetLookup.OnClear += () => {
				lookup.Clear();
			};
		}

		public static void Register(string name, string path, Asset<T> asset)
		{
			if (lookup.TryGetValue(name, out var existingTuple) && existingTuple.assetPath != path) {
				lookup[name] = (null, null); // This marks the registry as ambiguous.
			} else {
				lookup[name] = (path, asset);
			}
		}

		public static Asset<T> Get(string fullName, AssetRequestMode mode = AssetRequestMode.DoNotLoad)
		{
			var tuple = lookup[fullName];

			if (tuple.assetPath == null && tuple.asset == null) {
				throw new ArgumentException($"Key '{fullName}' is ambiguous.");
			}

			if (tuple.asset == null) {
				tuple.asset = Assets.Get<T>(tuple.assetPath, mode);
				lookup[fullName] = tuple;
			}

			return tuple.asset;
		}

		public static bool TryGet(string fullName, out Asset<T> result, AssetRequestMode mode = AssetRequestMode.DoNotLoad)
		{
			var tuple = lookup[fullName];

			if (tuple.assetPath == null && tuple.asset == null) {
				result = null;

				return false;
			}

			if (tuple.asset == null) {
				tuple.asset = Assets.Get<T>(tuple.assetPath, mode);
				lookup[fullName] = tuple;
			}

			result = tuple.asset;

			return true;
		}
	}
}
