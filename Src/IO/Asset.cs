using System;
using System.Collections.Generic;

namespace Dissonance.Engine.IO
{
	public abstract class Asset : IDisposable
	{
		public virtual string AssetName => null;

		public virtual void Dispose() { }

		public void RegisterAsset()
		{
			string name = AssetName;

			if(name != null) {
				var type = GetType();

				var resources = Resources.Instance;

				if(!resources.cacheByName.TryGetValue(type, out var dict)) {
					resources.cacheByName[type] = dict = new Dictionary<string, object>();
				}

				dict[name] = this;
			}
		}
	}
}
