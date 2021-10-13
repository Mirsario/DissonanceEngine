using System.Collections.Generic;
using System.IO;

namespace Dissonance.Engine.IO
{
	public abstract class AssetSource
	{
		public abstract IEnumerable<string> EnumerateAssets();

		public abstract bool HasAsset(string path);

		public abstract Stream OpenStream(string path);
	}
}
