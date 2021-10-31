using System;
using System.Collections.Concurrent;

namespace Dissonance.Engine.IO
{
	internal static class ContentLookup
	{
		public static event Action OnClear;

		public static void Clear() => OnClear?.Invoke();
	}

	internal static class ContentLookup<T>
	{
		private static readonly ConcurrentDictionary<string, T> lookup = new();

		public static int Count => lookup.Count;

		static ContentLookup()
		{
			AssetLookup.OnClear += () => {
				lookup.Clear();
			};
		}

		public static void Register(string name, T content)
		{
			if (lookup.ContainsKey(name)) {
				throw new Exception($"Cannot register two {typeof(T).Name} with the same name: {name}.");
			}

			lookup[name] = content;
		}

		public static T Get(string fullName)
			=> lookup[fullName] ?? throw new ArgumentException($"Key '{fullName}' is ambiguous.");

		public static bool TryGetValue(string fullName, out T value)
			=> lookup.TryGetValue(fullName, out value) && value != null;
	}
}
