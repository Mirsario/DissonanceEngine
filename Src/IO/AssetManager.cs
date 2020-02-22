using System;
using System.IO;

namespace Dissonance.Engine
{
	//TODO: It's currently quite impossible to make format return, for example, an array of meshes. Have to study unity's implementation of all this and come up with something better.
	public abstract class AssetManager
	{
		public virtual string[] Extensions => throw new NotImplementedException();

		public virtual void Init() { }
		public virtual bool Autoload(string file) => false;
	}

	public abstract class AssetManager<T> : AssetManager where T : class
	{
		public virtual T Import(Stream stream,string fileName) => throw new NotImplementedException();
		public virtual void Export(T asset,Stream stream) => throw new NotImplementedException();
	}
}