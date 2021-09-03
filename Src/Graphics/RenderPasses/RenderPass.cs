using System;
using System.Collections.Generic;

namespace Dissonance.Engine.Graphics
{
	public abstract class RenderPass : IDisposable
	{
		internal static Dictionary<string, Type> fullNameToType;

		private RenderTexture[] passedTextures;

		public string Name { get; set; }
		public bool Enabled { get; set; } = true;
		public Framebuffer Framebuffer { get; set; }
		public Renderbuffer[] Renderbuffers { get; set; }
		public Func<Camera, RectInt> ViewportFunc { get; set; }

		public RenderTexture[] PassedTextures {
			get => passedTextures;
			set {
				if (value == null || value.Length == 0) {
					passedTextures = null;
					return;
				}

				int length = value.Length;

				if (passedTextures == null || passedTextures.Length != length) {
					passedTextures = new RenderTexture[length];
				}

				Array.Copy(value, passedTextures, length);
			}
		}

		protected RenderPass() { }

		public abstract void Render();

		protected virtual void OnInit() { }

		protected virtual void OnDispose() { }

		protected virtual RectInt GetViewport(Camera? camera)
		{
			if (camera.HasValue) {
				if (ViewportFunc != null) {
					return ViewportFunc(camera.Value);
				}

				return camera.Value.ViewPixel;
			}

			if (Framebuffer != null) {
				return new RectInt(0, 0, Framebuffer.maxTextureWidth, Framebuffer.maxTextureHeight);
			}

			return new RectInt(0, 0, Screen.Width, Screen.Height);
		}

		public void Dispose()
		{
			OnDispose();
			GC.SuppressFinalize(this);
		}

		public ref T GlobalGet<T>() where T : struct
			=> ref ComponentManager.GetComponent<T>();

		public void GlobalSet<T>(T value) where T : struct
			=> ComponentManager.SetComponent(value);

		public static T Create<T>(string name, Action<T> initializer = null) where T : RenderPass, new()
		{
			var pass = new T {
				Name = name
			};

			initializer?.Invoke(pass);

			pass.OnInit();

			return pass;
		}

		public static RenderPass Create(Type type, string name, Action<RenderPass> initializer = null)
		{
			var pass = (RenderPass)Activator.CreateInstance(type, true);

			pass.Name = name;
			pass.Enabled = true;

			initializer?.Invoke(pass);

			return pass;
		}

		internal static void Init()
		{
			fullNameToType = new Dictionary<string, Type>();

			foreach (var type in AssemblyCache.EngineTypes) {
				if (!type.IsAbstract && typeof(RenderPass).IsAssignableFrom(type)) {
					fullNameToType[type.FullName] = type;
				}
			}
		}
	}
}
