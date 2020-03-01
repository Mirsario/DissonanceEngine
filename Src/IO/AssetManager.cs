using System;
using System.IO;

namespace Dissonance.Engine.IO
{
	//TODO: It's currently quite impossible to make format return, for example, an array of meshes.
	public abstract class AssetManager
	{
		public virtual string[] Extensions => throw new NotImplementedException();

		public virtual void Init() { }
		public virtual bool Autoload(string file) => false;
	}

	public abstract class AssetManager<T> : AssetManager where T : class
	{
		public virtual T Import(Stream stream,string filePath) => throw new NotImplementedException();
		public virtual void Export(T asset,Stream stream) => throw new NotImplementedException();
	}
}