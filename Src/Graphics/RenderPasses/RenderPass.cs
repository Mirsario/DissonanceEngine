using System;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Dissonance.Engine.Structures;
using Dissonance.Engine.Graphics.Components;
using Dissonance.Engine.Graphics.Shaders;
using Dissonance.Engine.Graphics.Textures;
using Dissonance.Engine.Core;

#pragma warning disable IDE0051 // Remove unused private members

namespace Dissonance.Engine.Graphics.RenderPasses
{
	public abstract class RenderPass : IDisposable
	{
		internal static Dictionary<string,Type> fullNameToType;
		internal static Dictionary<string,RenderPassInfoAttribute> fullNameToInfo;

		public string name;
		public bool enabled = true;
		public Renderbuffer[] renderbuffers;
		public Shader passShader; //Remove this field?

		protected Func<Camera,RectInt> viewportFunc;
		protected Framebuffer framebuffer;
		protected Shader[] shaders;
		protected RenderTexture[] passedTextures;

		public Func<Camera,RectInt> ViewportFunc {
			get => viewportFunc;
			set => viewportFunc = value;
		}
		public Framebuffer Framebuffer {
			get => framebuffer;
			set => framebuffer = value;
		}
		public Shader[] Shaders {
			get => shaders;
			set {
				if(value==null || value.Length==0) {
					shaders = null;
					passShader = null;
					return;
				}

				int length = value.Length;
				shaders = new Shader[length];

				Array.Copy(value,shaders,length);

				passShader = length==0 ? null : value[0]; //Temp?
			}
		}
		public Shader Shader {
			get => shaders?[0];
			set => Shaders = value!=null ? new[] { value } : null;
		}
		public RenderTexture[] PassedTextures {
			get => passedTextures;
			set {
				if(value==null || value.Length==0) {
					passedTextures = null;
					return;
				}

				int length = value.Length;
				if(passedTextures==null || passedTextures.Length!=length) {
					passedTextures = new RenderTexture[length];
				}

				Array.Copy(value,passedTextures,length);
			}
		}

		protected RenderPass() { }

		public abstract void Render();

		public virtual void OnInit() { }
		public virtual void Dispose() { }

		protected virtual RectInt GetViewport(Camera camera)
			=> viewportFunc?.Invoke(camera) ?? camera?.ViewPixel ?? (framebuffer!=null ? new RectInt(0,0,framebuffer.maxTextureWidth,framebuffer.maxTextureHeight) : new RectInt(0,0,Screen.Width,Screen.Height));

		public static T Create<T>(string name,Action<T> initializer = null) where T : RenderPass, new()
		{
			var pass = new T {
				name = name
			};

			initializer?.Invoke(pass);

			pass.OnInit();

			return pass;
		}
		public static RenderPass Create(Type type,string name,Action<RenderPass> initializer = null)
		{
			//TODO: This is not ideal, because it's skipping setting of default values.
			var pass = (RenderPass)FormatterServices.GetUninitializedObject(type);

			pass.name = name;
			pass.enabled = true;

			initializer?.Invoke(pass);

			return pass;
		}

		internal static void Init()
		{
			fullNameToType = new Dictionary<string,Type>();
			fullNameToInfo = new Dictionary<string,RenderPassInfoAttribute>();

			foreach(var type in AssemblyCache.EngineTypes) {
				if(!type.IsAbstract && typeof(RenderPass).IsAssignableFrom(type)) {
					string fullName = type.FullName;
					fullNameToType[fullName] = type;
					fullNameToInfo[fullName] = type.GetCustomAttribute<RenderPassInfoAttribute>();
				}
			}
		}
	}
}