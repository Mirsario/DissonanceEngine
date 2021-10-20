using System.IO;

namespace Dissonance.Engine.IO
{
	partial class GltfReader
	{
		protected class GltfInfo
		{
			public readonly string FilePath;
			public readonly PackedScene Scene;

			public GltfJson json;
			public Stream blobStream;

			public GltfInfo(string filePath)
			{
				FilePath = filePath;
				Scene = new PackedScene();
			}
		}
	}
}
