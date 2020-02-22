using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dissonance.Engine
{
	public abstract class Asset : IDisposable
	{
		public virtual string AssetName => null;

		public virtual void Dispose() { }

		public void RegisterAsset()
		{
			string name = AssetName;

			if(name!=null) {
				var type = GetType();

				if(!Resources.cacheByName.TryGetValue(type,out var dict)) {
					Resources.cacheByName[type] = dict = new Dictionary<string,object>();
				}

				dict[name] = this;
			}
		}
	}
}
