using System.IO;

namespace Dissonance.Engine.IO
{
	partial class GltfManager
	{
		public class GltfInfo
		{
			public AssetPack assets;
			public string filePath;
			public GltfJson json;
			public Stream blobStream;
		}
	}
}
