using System;
using System.IO;

namespace GameEngine
{
	//TODO: It's currently quite impossible to make format return, for example, an array of meshes. Have to study unity's implementation of all this and come up with something better.
	public abstract class AssetManager
	{
		public virtual string[] Extensions => throw new NotImplementedException();
		public virtual bool Autoload(string file) => false;
	}
	public abstract class AssetManager<T> : AssetManager where T : class
	{
		public virtual T Import(Stream stream,string fileName) => throw new NotImplementedException();
		public virtual void Export(T asset,Stream stream) => throw new NotImplementedException();
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class AutoloadRequirement : Attribute
	{
		public Type[] requirements;

		public AutoloadRequirement(params Type[] types)
		{
			if(types==null || types.Length==0) {
				throw new ArgumentException("'types' array cannot be null or empty");
			}
			requirements = types;
		}
	}
}