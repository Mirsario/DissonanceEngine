using System;
using System.Linq;
using Dissonance.Engine.Core;
using Dissonance.Engine.Core.Attributes;
using Dissonance.Engine.Core.Components;
using Dissonance.Engine.Core.Modules;
using Dissonance.Engine.Graphics.Components;
using Dissonance.Engine.Graphics.Meshes;
using Dissonance.Engine.Graphics.RenderingPipelines;
using Dissonance.Engine.Graphics.Shaders;
using Dissonance.Engine.Graphics.Textures;
using Dissonance.Engine.Graphics.UserInterface;
using Dissonance.Engine.Input;
using Dissonance.Engine.IO;
using Dissonance.Engine.Structures;
using Dissonance.Framework.Graphics;
using Dissonance.Framework.Windowing;
using Dissonance.Framework.Windowing.Input;

namespace Dissonance.Engine.Graphics
{
	//TODO: Add submeshes to Mesh.cs
	//TODO: Add some way to sort objects in a way that'd let the engine skip BoxInFrustum checks for objects which are in non-visible chunks.
	[Autoload(DisablingGameFlags = GameFlags.NoGraphics)]
	[ModuleDependency(typeof(Windowing), typeof(Screen), typeof(Resources))]
	public sealed partial class Rendering : EngineModule
	{
		public static readonly Version MinOpenGLVersion = new Version(3, 2);
		public static readonly Version[] SupportedOpenGLVersions = GL.SupportedVersions.Where(v => v >= MinOpenGLVersion).ToArray();

		public static int drawCallsCount;
		public static Vector3 ambientColor = new Vector3(0.1f, 0.1f, 0.1f);
		public static Vector4 clearColor = Vector4.Zero;

		internal static Texture whiteTexture; //TODO: Move this
		internal static Type renderingPipelineType;
		internal static BlendingFactor currentBlendFactorSrc;
		internal static BlendingFactor currentBlendFactorDst;
		internal static uint currentStencilMask;

		private static Version openGLVersion = MinOpenGLVersion;
		private static GL.DebugCallback debugCallback;
		private static Shader guiShader; //TODO: To be moved

		public static RenderingPipeline RenderingPipeline { get; set; }
		public static bool DebugFramebuffers { get; set; }
		public static Version OpenGLVersion {
			get => openGLVersion;
			set {
				if(Game.Instance?.preInitDone != false) {
					throw new InvalidOperationException($"OpenGL version can only be set in '{nameof(Game)}.{nameof(Game.PreInit)}()'.");
				}

				if(!GL.SupportedVersions.Contains(value)) {
					throw new ArgumentException($"OpenGL version '{value}' is unknown or not supported. The following versions are supported:\r\n{string.Join("\r\n", SupportedOpenGLVersions.Select(v => $"{v};"))}.");
				}

				openGLVersion = value;
			}
		}

		public static Shader GUIShader => guiShader ??= Resources.Find<Shader>("GUI"); //TODO: To be moved

		private Windowing windowing;

		protected override void PreInit()
		{
			Game.TryGetModule(out windowing);

			renderingPipelineType = typeof(DeferredRendering);

			if(!Game.Flags.HasFlag(GameFlags.NoWindow)) {
				PrepareOpenGL();
			}
		}
		protected override void Init()
		{
			var glVersion = GetOpenGLVersion();

			if(glVersion < openGLVersion) {
				throw new Exception($"Please update your graphics drivers.\r\nMinimum OpenGL version required to run this application is: {openGLVersion}\r\nYour OpenGL version is: {glVersion}");
			}

			CheckGLErrors("After checking GL version");

			TryEnablingDebugging();

			CheckGLErrors($"After trying to enable debugging.");

			//FontImport
			//TODO: Add AssetManager for fonts and remove this hardcode
			var tex = Resources.Import<Texture>("BuiltInAssets/GUI/Fonts/DefaultFont.png");
			GUI.font = new Font(@" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~", tex, new Vector2(12f, 16f), 0) { size = 16 };

			CheckGLErrors($"After initializing a default font.");

			GL.CullFace(CullFaceMode.Back);
			GL.DepthFunc(DepthFunction.Lequal);

			InstantiateRenderingPipeline();

			PrimitiveMeshes.Init();

			DrawUtils.Init();

			whiteTexture = new Texture(1, 1);
			whiteTexture.SetPixels(new[] { new Pixel(1f, 1f, 1f, 1f) });

			CheckGLErrors($"At the end '{nameof(Rendering)}.{nameof(Init)}' call.");
		}
		protected override void RenderUpdate()
		{
			drawCallsCount = 0;

			//Calculate view and projection matrices, culling frustums
			foreach(var camera in ComponentManager.EnumerateComponents<Camera>()) {
				var viewSize = camera.ViewPixel;
				float aspectRatio = viewSize.width / (float)viewSize.height;

				var cameraTransform = camera.Transform;
				var cameraPosition = cameraTransform.Position;

				camera.matrix_view = Matrix4x4.LookAt(cameraPosition, cameraPosition + cameraTransform.Forward, cameraTransform.Up);

				if(camera.orthographic) {
					float max = Mathf.Max(Screen.Width, Screen.Height);

					camera.matrix_proj = Matrix4x4.CreateOrthographic(Screen.Width / max * camera.orthographicSize, Screen.Height / max * camera.orthographicSize, camera.nearClip, camera.farClip);
				} else {
					camera.matrix_proj = Matrix4x4.CreatePerspectiveFOV(camera.fov * Mathf.Deg2Rad, aspectRatio, camera.nearClip, camera.farClip);
				}

				camera.matrix_viewInverse = Matrix4x4.Invert(camera.matrix_view);
				camera.matrix_projInverse = Matrix4x4.Invert(camera.matrix_proj);

				camera.CalculateFrustum(camera.matrix_view * camera.matrix_proj);
			}

			//Clear buffers
			GL.Viewport(0, 0, Screen.Width, Screen.Height);

			if(RenderingPipeline.Framebuffers != null) {
				int length = RenderingPipeline.Framebuffers.Length;

				for(int i = 0; i <= length; i++) {
					var framebuffer = i == length ? null : RenderingPipeline.Framebuffers[i];

					framebuffer?.PrepareAttachments();

					Framebuffer.BindWithDrawBuffers(framebuffer);

					GL.ClearColor(clearColor.x, clearColor.y, clearColor.z, clearColor.w);
					//GL.StencilMask(~0);
					GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
				}
			}

			RenderingPipeline.PreRender();

			Framebuffer.Bind(null);

			CheckGLErrors("Before rendering passes");

			//Render passes
			for(int i = 0; i < RenderingPipeline.RenderPasses.Length; i++) {
				var pass = RenderingPipeline.RenderPasses[i];

				if(pass.enabled) {
					pass.Render();

					CheckGLErrors($"Rendering pass {pass.name} ({pass.GetType().Name})");
				}
			}

			Framebuffer.Bind(null);

			RenderingPipeline.PostRender();

			if(DebugFramebuffers) {
				BlitFramebuffers();
			}

			windowing.SwapBuffers();

			CheckGLErrors("After swapping buffers");
		}
		protected override void OnDispose()
		{
			windowing = null;

			//TODO:
		}

		public static void SetRenderingPipeline<T>() where T : RenderingPipeline, new()
		{
			renderingPipelineType = typeof(T);

			if(RenderingPipeline != null && Game.Instance?.NoGraphics == false) {
				InstantiateRenderingPipeline();
			}
		}

		private static void PrepareOpenGL()
		{
			Debug.Log("Preparing OpenGL...");

			GL.Load(OpenGLVersion);

			CheckGLErrors("Post GL.Load()");

			Debug.Log($"Initialized OpenGL {GL.GetString(StringName.Version)}");
		}
		private static void InstantiateRenderingPipeline()
		{
			RenderingPipeline?.Dispose();

			RenderingPipeline = (RenderingPipeline)Activator.CreateInstance(renderingPipelineType);

			RenderingPipeline.Init();
		}
		private static void BlitFramebuffers()
		{
			//TODO: This is temporary
			static bool IsDepthTexture(RenderTexture tex) => tex.name.Contains("depth");

			int textureCount = 0;
			var framebuffers = RenderingPipeline.framebuffers;

			for(int i = 0; i < framebuffers.Length; i++) {
				var fb = framebuffers[i];

				for(int j = 0; j < fb.renderTextures.Count; j++) {
					var tex = fb.renderTextures[j];

					if(!IsDepthTexture(tex)) {
						textureCount++;
					}
				}
			}

			if(InputEngine.GetKey(Keys.LeftShift)) {
				textureCount = Math.Min(textureCount, 1);
			}

			int size = 1;

			while(size * size < textureCount) {
				size++;
			}

			int x = 0;
			int y = 0;

			for(int i = 0; i < framebuffers.Length; i++) {
				var framebuffer = framebuffers[i];

				Framebuffer.Bind(framebuffer, FramebufferTarget.ReadFramebuffer);

				for(int j = 0; j < framebuffer.renderTextures.Count; j++) {
					var tex = framebuffer.renderTextures[j];

					if(IsDepthTexture(tex)) {
						continue;
					}

					GL.ReadBuffer(ReadBufferMode.ColorAttachment0 + j);

					int wSize = Screen.Width / size;
					int hSize = Screen.Height / size;

					GL.BlitFramebuffer(
						0, 0, tex.Width, tex.Height,
						x * wSize, (size - y - 1) * hSize, (x + 1) * wSize, (size - y) * hSize,
						ClearBufferMask.ColorBufferBit,
						BlitFramebufferFilter.Nearest
					);

					if(++x >= size) {
						x = 0;
						y++;
					}
				}
			}
		}
	}
}
