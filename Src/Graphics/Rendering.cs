using System;
using System.Collections.Generic;
using Dissonance.Engine.Graphics.RenderingPipelines;
using Dissonance.Engine.IO;
using Dissonance.Framework.Graphics;
using Dissonance.Framework.Windowing;
using Dissonance.Framework.Windowing.Input;

namespace Dissonance.Engine.Graphics
{
	//TODO: Add submeshes to Mesh.cs
	//TODO: Add some way to sort objects in a way that'd let the engine skip BoxInFrustum checks for objects which are in non-visible chunks.
	public static partial class Rendering
	{
		public static int drawCallsCount;
		public static Vector3 ambientColor = new Vector3(0.1f,0.1f,0.1f);
		public static Vector4 clearColor = Vector4.Zero;

		internal static List<Camera> cameraList;
		internal static List<Renderer> rendererList;
		internal static List<Light> lightList;
		internal static List<Light2D> light2DList;
		internal static Texture whiteTexture; //TODO: Move this
		internal static Type renderingPipelineType;
		internal static BlendingFactor currentBlendFactorSrc;
		internal static BlendingFactor currentBlendFactorDst;
		internal static uint currentStencilMask;

		private static GLVersion openGLVersion = GLVersion.GL30;

		public static RenderingPipeline RenderingPipeline { get; private set; }
		public static bool DebugFramebuffers { get; set; }
		public static GLVersion OpenGLVersion {
			get => openGLVersion;
			set {
				if(Game.preInitDone) {
					throw new InvalidOperationException("OpenGL version can only be set in 'Game.PreInit()'.");
				}

				openGLVersion = value;
			}
		}

		//TODO: To be moved
		private static Shader guiShader;
		public static Shader GUIShader => guiShader ??= Resources.Find<Shader>("GUI");

		public static void SetRenderingPipeline<T>() where T : RenderingPipeline, new()
		{
			renderingPipelineType = typeof(T);

			if(RenderingPipeline!=null) {
				InstantiateRenderingPipeline();
			}
		}

		internal static void PreInit()
		{
			renderingPipelineType = typeof(DeferredRendering);
		}
		internal static void Init()
		{
			var glVersion = GetOpenGLVersion();
			var minVersion = new Version("2.0");

			if(glVersion<minVersion) {
				throw new Exception($"Please update your graphics drivers.\r\nMinimum OpenGL version required to run this application is: {minVersion}\r\nYour OpenGL version is: {glVersion}");
			}

			CheckGLErrors("After checking GL version");

			//FontImport
			//TODO: Add AssetManager for fonts and remove this hardcode
			var tex = Resources.Import<Texture>("BuiltInAssets/GUI/Fonts/DefaultFont.png");
			GUI.font = new Font(@" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~",tex,new Vector2(12f,16f),0) { size = 16 };

			cameraList = new List<Camera>();
			rendererList = new List<Renderer>();
			lightList = new List<Light>();
			light2DList = new List<Light2D>();

			GL.CullFace(CullFaceMode.Back);
			GL.DepthFunc(DepthFunction.Lequal);

			InstantiateRenderingPipeline();

			PrimitiveMeshes.Init();

			DrawUtils.Init();
			
			whiteTexture = new Texture(1,1);
			whiteTexture.SetPixels(new[] { new Pixel(1f,1f,1f,1f) });
		}
		internal static void Render()
		{
			drawCallsCount = 0;

			//Calculate view and projection matrices, culling frustums
			for(int i = 0;i<cameraList.Count;i++) {
				var camera = cameraList[i];

				var viewSize = camera.ViewPixel;
				float aspectRatio = viewSize.width/(float)viewSize.height;

				var cameraTransform = camera.Transform;
				var cameraPosition = cameraTransform.Position;

				camera.matrix_view = Matrix4x4.LookAt(cameraPosition,cameraPosition+cameraTransform.Forward,cameraTransform.Up);

				if(camera.orthographic) {
					float max = Mathf.Max(Screen.Width,Screen.Height);
					camera.matrix_proj = Matrix4x4.CreateOrthographic(Screen.Width/max*camera.orthographicSize,Screen.Height/max*camera.orthographicSize,camera.nearClip,camera.farClip);
				}else{
					camera.matrix_proj = Matrix4x4.CreatePerspectiveFOV(camera.fov*Mathf.Deg2Rad,aspectRatio,camera.nearClip,camera.farClip);
				}

				camera.matrix_viewInverse = Matrix4x4.Invert(camera.matrix_view);
				camera.matrix_projInverse = Matrix4x4.Invert(camera.matrix_proj);

				camera.CalculateFrustum(camera.matrix_view*camera.matrix_proj);
			}

			//Clear buffers
			GL.Viewport(0,0,Screen.Width,Screen.Height);

			if(RenderingPipeline.Framebuffers!=null) {
				int length = RenderingPipeline.Framebuffers.Length;

				for(int i = 0;i<=length;i++) {
					var framebuffer = i==length ? null : RenderingPipeline.Framebuffers[i];

					framebuffer?.PrepareAttachments();

					Framebuffer.Bind(framebuffer);

					GL.ClearColor(clearColor.x,clearColor.y,clearColor.z,clearColor.w);
					//GL.StencilMask(~0);
					GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
				}
			}

			RenderingPipeline.PreRender();

			Framebuffer.Bind(null);

			CheckGLErrors("Before rendering passes");

			//Render passes
			for(int i = 0;i<RenderingPipeline.RenderPasses.Length;i++) {
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

			GLFW.SwapBuffers(Game.window);

			CheckGLErrors("After swapping buffers");
		}
		internal static void Dispose() {}

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

			for(int i = 0;i<framebuffers.Length;i++) {
				var fb = framebuffers[i];

				for(int j = 0;j<fb.renderTextures.Count;j++) {
					var tex = fb.renderTextures[j];

					if(!IsDepthTexture(tex)) {
						textureCount++;
					}
				}
			}

			if(Input.GetKey(Keys.LeftShift)) {
				textureCount = Math.Min(textureCount,1);
			}

			int size = 1;

			while(size*size<textureCount) {
				size++;
			}

			int x = 0;
			int y = 0;

			for(int i = 0;i<framebuffers.Length;i++) {
				var framebuffer = framebuffers[i];

				Framebuffer.Bind(framebuffer,FramebufferTarget.ReadFramebuffer);

				for(int j = 0;j<framebuffer.renderTextures.Count;j++) {
					var tex = framebuffer.renderTextures[j];

					if(IsDepthTexture(tex)) {
						continue;
					}

					GL.ReadBuffer(ReadBufferMode.ColorAttachment0+j);

					int wSize = Screen.width/size;
					int hSize = Screen.height/size;

					GL.BlitFramebuffer(
						0,0,tex.Width,tex.Height,
						x*wSize,(size-y-1)*hSize,(x+1)*wSize,(size-y)*hSize,
						ClearBufferMask.ColorBufferBit,
						BlitFramebufferFilter.Nearest
					);

					if(++x>=size) {
						x = 0;
						y++;
					}
				}
			}
		}
	}
}