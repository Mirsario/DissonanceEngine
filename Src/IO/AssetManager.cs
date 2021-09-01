using System;
using System.IO;

namespace Dissonance.Engine.IO
{
	public abstract class AssetManager
	{
		public virtual string[] Extensions => throw new NotImplementedException();

		public virtual void Init() { }
		public virtual bool Autoload(string file) => false;
	}

	public abstract class AssetManager<T> : AssetManager
	{
		public virtual T Import(Stream stream, string filePath) => throw new NotImplementedException();
		public virtual void Export(T asset, Stream stream) => throw new NotImplementedException();
	}
}
