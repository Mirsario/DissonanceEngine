using System;
using System.Collections.Generic;

namespace Dissonance.Engine.IO
{
	public class AssetPack : Asset
	{
		private readonly Dictionary<Type, List<(string name, int nameHash, object value)>> assets = new Dictionary<Type, List<(string, int, object)>>();

		public T Get<T>(string name)
		{
			return Get<T>(name, out var result) ? result : default;
		}
		public bool Get<T>(string name, out T result)
		{
			if(assets.TryGetValue(typeof(T), out var list)) {
				int nameHash = name.GetHashCode();

				for(int i = 0; i < list.Count; i++) {
					var (assetName, assetNameHash, asset) = list[i];

					if(assetName != null && nameHash == assetNameHash && name == assetName) {
						result = (T)asset;

						return true;
					}
				}
			}

			result = default;

			return false;
		}
		public void Add<T>(T asset, string name = null)
		{
			if(!assets.TryGetValue(typeof(T), out var list)) {
				assets[typeof(T)] = list = new List<(string, int, object)>();
			}

			list.Add((name, name?.GetHashCode() ?? 0, asset));
		}
	}
}
