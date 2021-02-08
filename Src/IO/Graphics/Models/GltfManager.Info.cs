using System.IO;

namespace Dissonance.Engine.IO
{
	partial class GltfManager
	{
		protected class GltfInfo
		{
			public readonly string FilePath;
			public readonly GameObject RootObject;

			public GltfJson json;
			public Stream blobStream;

			public GltfInfo(string filePath)
			{
				FilePath = filePath;
				RootObject = GameObject.Instantiate<GameObject>(g => g.Name = Path.GetFileName(filePath), enable: false);
			}
		}
	}
}
