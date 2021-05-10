using System;
using System.Linq;
using Dissonance.Engine.IO;
using Dissonance.Framework.Graphics;

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

		internal static Texture whiteTexture; //TODO: Move this
		internal static Type renderingPipelineType;
		internal static BlendingFactor currentBlendFactorSrc;
		internal static BlendingFactor currentBlendFactorDst;
		internal static uint currentStencilMask;

		private static Version openGLVersion = MinOpenGLVersion;
		private static GL.DebugCallback debugCallback;
		private static Shader guiShader; //TODO: To be moved

		public static int DrawCallsCount { get; set; }
		public static Vector4 ClearColor { get; set; } = Vector4.One * 0.25f;
		public static Vector3 AmbientColor { get; set; } = new Vector3(0.1f, 0.1f, 0.1f);
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

		internal static Windowing windowing;

		protected override void PreInit()
		{
			Game.TryGetModule(out windowing);

			renderingPipelineType = typeof(ForwardRendering);

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
			Debug.ResetRendering();

			whiteTexture = new Texture(1, 1);
			whiteTexture.SetPixels(new[] { new Pixel(1f, 1f, 1f, 1f) });

			CheckGLErrors($"At the end '{nameof(Rendering)}.{nameof(Init)}' call.");
		}
		protected override void RenderUpdate()
		{
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

			if(renderingPipelineType != null) {
				RenderingPipeline = (RenderingPipeline)Activator.CreateInstance(renderingPipelineType);

				RenderingPipeline.Init();
			}
		}
	}
}
