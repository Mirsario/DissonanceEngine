using System;
using System.IO;

namespace Dissonance.Engine.IO
{
	public interface IAssetReader<T>
	{
		string[] Extensions { get; }

		T ReadFromStream(Stream stream, string assetPath);
	}
}
