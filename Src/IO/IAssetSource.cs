using System.Collections.Generic;
using System.IO;

namespace Dissonance.Engine.IO;

public interface IAssetSource
{
	IEnumerable<string> EnumerateAssets();

	bool HasAsset(string path);

	Stream OpenStream(string path);
}
