using System.IO;

namespace Dissonance.Engine.IO.Graphics.Models
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
