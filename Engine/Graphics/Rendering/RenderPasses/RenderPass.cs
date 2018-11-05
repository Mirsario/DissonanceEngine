using System;
using System.Reflection;
using System.Collections.Generic;

namespace GameEngine
{
	internal abstract class RenderPass : IDisposable
	{
		internal static Dictionary<string,Type> idToType;
		internal static Dictionary<string,RenderPass> idToInstance;
		
		public string name;
		public Framebuffer framebuffer;
		public RenderTexture[] textures;
		public RenderBuffer[] renderBuffers;
		public Shader passShader;
		public Shader[] shaders;

		public abstract string Id { get; }
		public virtual bool RequiresShader => false;
		public virtual string[] AcceptedShaderNames => null;

		public virtual void Render() {}
		public virtual void Dispose() {}

		internal static void Init()
		{
			idToType = new Dictionary<string,Type>();
			idToInstance = new Dictionary<string,RenderPass>();

			foreach(var type in ReflectionCache.engineTypes) {
				if(!type.IsAbstract && typeof(RenderPass).IsAssignableFrom(type)) {
					var instance = (RenderPass)Activator.CreateInstance(type);
					string id = instance.Id;
					idToType[id] = type;
					idToInstance[id] = instance;
				}
			}
		}
	}
}