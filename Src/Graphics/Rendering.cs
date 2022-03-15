using System;
using System.Linq;
using System.Reflection;
using Dissonance.Engine.IO;
using Silk.NET.OpenGL;
using static Dissonance.Engine.Graphics.OpenGLApi;
using static Dissonance.Engine.Graphics.GlfwApi;

namespace Dissonance.Engine.Graphics
{
	[Autoload(DisablingGameFlags = GameFlags.NoGraphics)]
	[ModuleDependency(typeof(Windowing), typeof(Screen), typeof(Assets), typeof(ComponentManager))]
	public sealed unsafe partial class Rendering : EngineModule
	{
		public static readonly Version MinOpenGLVersion = new(3, 2);

		internal static Texture whiteTexture; //TODO: Move this
		internal static Type renderingPipelineType;
		internal static BlendingFactor currentBlendFactorSrc;
		internal static BlendingFactor currentBlendFactorDst;
		internal static uint currentStencilMask;
		internal static Windowing windowing;

		private static Version openGLVersion = MinOpenGLVersion;
		private static DebugProc debugCallback;
		private static Asset<Shader> guiShader; //TODO: To be moved
		private Action resetRenderComponents;

		public static int DrawCallsCount { get; set; }
		public static Vector4 ClearColor { get; set; } = Vector4.Zero;
		public static Vector3 AmbientColor { get; set; } = new Vector3(0.1f, 0.1f, 0.1f);
		public static RenderingPipeline RenderingPipeline { get; set; }
		public static bool DebugFramebuffers { get; set; }

		public static Asset<Shader> GUIShader => guiShader ??= Assets.Find<Shader>("Gui/Default"); //TODO: To be moved

		public static Version OpenGLVersion {
			get => openGLVersion;
			set {
				if (Game.Instance?.preInitDone != false) {
					throw new InvalidOperationException($"OpenGL version can only be set in '{nameof(Game)}.{nameof(Game.PreInit)}()'.");
				}

				openGLVersion = value;
			}
		}

		protected override void PreInit()
		{
			Game.TryGetModule(out windowing);

			renderingPipelineType = typeof(ForwardRendering);

			if (!Game.Flags.HasFlag(GameFlags.NoWindow)) {
				InitOpenGL(GLFW);
			}

			// Prepare the delegate for resetting render components. Could be moved somewhere else...
			AssemblyRegistrationModule.OnAssemblyRegistered += (assembly, types) => {
				foreach (var type in types) {
					if (type.IsClass || type.IsInterface || !typeof(IRenderComponent).IsAssignableFrom(type)) {
						continue;
					}

					var methods = typeof(ComponentManager).GetMethods(BindingFlags.Static | BindingFlags.NonPublic);

					var hasComponentMethod = methods
						.First(m => m.Name == nameof(ComponentManager.HasComponent) && m.GetParameters().Length == 0)
						.MakeGenericMethod(type);

					var getComponentMethod = methods
						.First(m => m.Name == nameof(ComponentManager.GetComponent) && m.GetParameters().Length == 0)
						.MakeGenericMethod(type);

					var setComponentMethod = methods
						.First(m => m.Name == nameof(ComponentManager.SetComponent) && m.GetParameters().Length == 1)
						.MakeGenericMethod(type);

					resetRenderComponents += () => {
						IRenderComponent component;

						if ((bool)hasComponentMethod?.Invoke(null, null)) {
							component = (IRenderComponent)getComponentMethod.Invoke(null, null);
						} else {
							component = (IRenderComponent)Activator.CreateInstance(type);
						}

						component.Reset();

						setComponentMethod.Invoke(null, new object[] { component });
					};
				}
			};
		}

		protected override void Init()
		{
			var glVersion = GetOpenGLVersion();

			if (glVersion < openGLVersion) {
				throw new Exception($"Please update your graphics drivers.\r\nMinimum OpenGL version required to run this application is: {openGLVersion}\r\nYour OpenGL version is: {glVersion}");
			}

			CheckGLErrors("After checking GL version");

			TryEnablingDebugging();

			CheckGLErrors($"After trying to enable debugging.");

			InstantiateRenderingPipeline();

			PrimitiveMeshes.Init();
			DrawUtils.Init();
			Debug.ResetRendering();

			whiteTexture = new Texture(1, 1);
			whiteTexture.SetPixels(new[] { new Pixel(1f, 1f, 1f, 1f) });

			CheckGLErrors($"At the end '{nameof(Rendering)}.{nameof(Init)}' call.");
		}

		protected override void PreRenderUpdate()
		{
			resetRenderComponents?.Invoke();
		}

		[HookPosition(10000)]
		protected override void RenderUpdate()
		{
			// Clear the screen
			ClearScreen();

			// Execute rendering systems

			foreach (var world in WorldManager.ReadWorlds()) {
				world.ExecuteCallbacks<RootRenderingCallback>();
			}

			// RenderPasses
			var pipeline = RenderingPipeline;

			for (int i = 0; i < pipeline.RenderPasses.Length; i++) {
				var pass = pipeline.RenderPasses[i];

				if (pass.Enabled) {
					pass.Render();

					CheckGLErrors($"Rendering pass {pass.Name} ({pass.GetType().Name})");
				}
			}

			Framebuffer.Bind(null);

			// Blit buffers
			if (DebugFramebuffers) {
				BlitFramebuffers();
			}

			// Swap buffers
			windowing.SwapBuffers();
			CheckGLErrors("After swapping buffers");

			DrawCallsCount = 0;

			Debug.ResetRendering();
		}

		protected override void OnDispose()
		{
			windowing = null;

			//TODO:
		}

		public static void SetRenderingPipeline<T>() where T : RenderingPipeline, new()
		{
			renderingPipelineType = typeof(T);

			if (RenderingPipeline != null && Game.Instance?.NoGraphics == false) {
				InstantiateRenderingPipeline();
			}
		}

		private static void InstantiateRenderingPipeline()
		{
			RenderingPipeline?.Dispose();

			if (renderingPipelineType != null) {
				RenderingPipeline = (RenderingPipeline)Activator.CreateInstance(renderingPipelineType);

				RenderingPipeline.Init();
			}
		}
		
		private static void ClearScreen()
		{
			OpenGL.Viewport(0, 0, (uint)Screen.Width, (uint)Screen.Height);

			var pipeline = RenderingPipeline;
			var clearColor = ClearColor;

			if (pipeline.Framebuffers != null) {
				int length = pipeline.Framebuffers.Length;

				for (int i = 0; i <= length; i++) {
					var framebuffer = i == length ? null : pipeline.Framebuffers[i];

					framebuffer?.PrepareAttachments();

					Framebuffer.BindWithDrawBuffers(framebuffer);

					OpenGL.ClearColor(clearColor.X, clearColor.Y, clearColor.Z, clearColor.W);
					// OpenGL.Api.StencilMask(~0);
					OpenGL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
				}
			}

			Framebuffer.Bind(null);
		}

		private static void BlitFramebuffers()
		{
			static bool IsDepthTexture(RenderTexture tex)
				=> tex.PixelFormat == PixelFormat.DepthComponent;

			var renderingPipeline = RenderingPipeline;
			var framebuffers = renderingPipeline.framebuffers;
			int textureCount = 0;

			for (int i = 0; i < framebuffers.Length; i++) {
				var fb = framebuffers[i];

				for (int j = 0; j < fb.renderTextures.Count; j++) {
					var tex = fb.renderTextures[j];

					if (!IsDepthTexture(tex)) {
						textureCount++;
					}
				}
			}

			int size = 1;

			while (size * size < textureCount) {
				size++;
			}

			int x = 0;
			int y = 0;

			for (int i = 0; i < framebuffers.Length; i++) {
				var framebuffer = framebuffers[i];

				Framebuffer.Bind(framebuffer, FramebufferTarget.ReadFramebuffer);

				for (int j = 0; j < framebuffer.renderTextures.Count; j++) {
					var tex = framebuffer.renderTextures[j];

					if (IsDepthTexture(tex)) {
						continue;
					}

					OpenGL.ReadBuffer(ReadBufferMode.ColorAttachment0 + j);

					int wSize = Screen.Width / size;
					int hSize = Screen.Height / size;

					OpenGL.BlitFramebuffer(
						0, 0, tex.Width, tex.Height,
						x * wSize, (size - y - 1) * hSize, (x + 1) * wSize, (size - y) * hSize,
						ClearBufferMask.ColorBufferBit,
						BlitFramebufferFilter.Nearest
					);

					if (++x >= size) {
						x = 0;
						y++;
					}
				}
			}
		}
	}
}
