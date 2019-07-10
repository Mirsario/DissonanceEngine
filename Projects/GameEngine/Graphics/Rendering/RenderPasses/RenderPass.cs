using System;
using System.Reflection;
using System.Collections.Generic;

namespace GameEngine.Graphics
{
	public abstract class RenderPass : IDisposable
	{
		internal static Dictionary<string,Type> fullNameToType;
		internal static Dictionary<string,RenderPassInfoAttribute> fullNameToInfo;
		
		public string name;
		public bool enabled = true;
		public Framebuffer framebuffer;
		public RenderTexture[] passedTextures;
		public Renderbuffer[] renderbuffers;
		public Shader passShader; //Remove this field?
		public Shader[] shaders;
		private Func<Camera,RectInt> viewportGetter;

		protected RenderPass(string name)
		{
			this.name = name;
		}

		public abstract void Render();
		
		protected virtual RectInt GetViewport(Camera camera) => viewportGetter?.Invoke(camera) ?? (camera?.ViewPixel ?? (framebuffer!=null ? new RectInt(0,0,framebuffer.maxTextureWidth,framebuffer.maxTextureHeight) : new RectInt(0,0,Screen.Width,Screen.Height)));

		public virtual void Dispose() {}

		public RenderPass WithFramebuffer(Framebuffer framebuffer)
		{
			this.framebuffer = framebuffer;
			return this;
		}
		public RenderPass WithViewport(Func<Camera,RectInt> getter)
		{
			viewportGetter = getter;
			return this;
		}
		public RenderPass WithPassedTextures(params RenderTexture[] passedTextures)
		{
			if(this.passedTextures==null || this.passedTextures.Length!=passedTextures.Length) {
				this.passedTextures = new RenderTexture[passedTextures.Length];
			}
			for(int i = 0;i<passedTextures.Length;i++) {
				this.passedTextures[i] = passedTextures[i];
			}
			return this;
		}
		public RenderPass WithShaders(params Shader[] shaders)
		{
			this.shaders = new Shader[shaders.Length];
			for(int i = 0;i<shaders.Length;i++) {
				this.shaders[i] = shaders[i]; // ?? throw new ArgumentNullException($"Shader cannot be null.");;
			}

			passShader = shaders.Length==0 ? null : shaders[0]; //Temp?

			return this;
		}

		internal static void Init()
		{
			fullNameToType = new Dictionary<string,Type>();
			fullNameToInfo = new Dictionary<string,RenderPassInfoAttribute>();

			foreach(var type in ReflectionCache.engineTypes) {
				if(!type.IsAbstract && typeof(RenderPass).IsAssignableFrom(type)) {
					string fullName = type.FullName;
					fullNameToType[fullName] = type;
					fullNameToInfo[fullName] = type.GetCustomAttribute<RenderPassInfoAttribute>();
				}
			}
		}
	}
}