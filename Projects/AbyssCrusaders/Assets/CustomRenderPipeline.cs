using System;
using System.Linq;
using GameEngine;
using GameEngine.Graphics;

namespace AbyssCrusaders
{
	public class CustomRenderPipeline : RenderingPipeline
	{
		private Shader postLightingShader;
		private Shader compositeShader;
		
		public override void Setup(out Framebuffer[] framebuffers,out RenderPass[] renderPasses)
		{
			//Vector2Int ScreenSize() => new Vector2Int(Screen.Width,Screen.Height);

			Framebuffer mainFramebuffer,lightingFramebuffer;

			static Vector2Int ScreenSize() => Screen.Size;
			static Vector2Int LightingSize() => new Vector2Int((int)Math.Ceiling(Screen.Width/(Main.camera?.zoomGoal ?? 1f)),(int)Math.Ceiling(Screen.Height/(Main.camera?.zoomGoal ?? 1f)));

			//Framebuffers
			framebuffers = new[] {
				mainFramebuffer = new Framebuffer("mainBuffer")
					.WithRenderTexture(new RenderTexture("colorBuffer",ScreenSize),out var colorBuffer)
					.WithRenderTexture(new RenderTexture("emissionBuffer",ScreenSize,textureFormat:TextureFormat.RGBA32f),out var emissionBuffer)
					.WithRenderbuffer(new Renderbuffer("depthBuffer",RenderbufferStorage.DepthComponent32f),FramebufferAttachment.DepthAttachment),

				lightingFramebuffer = new Framebuffer("lightingBuffer")
					.WithRenderTexture(new RenderTexture("lightingBuffer",LightingSize,textureFormat:TextureFormat.RGB8),out var lightingBuffer)
			};

			postLightingShader = Resources.Find<Shader>("Game/PostLighting");
			compositeShader = Resources.Find<Shader>("Game/Composite");

			//RenderPasses
			renderPasses = new[] {
				//Geometry
				new GeometryPass("Geometry")
					.WithFramebuffer(mainFramebuffer),
				
				//Lights
				new Light2DPass("Lighting")
					.WithFramebuffer(lightingFramebuffer)
					.WithViewport(c => new RectInt(default,LightingSize()))
					.WithShaders(Resources.Find<Shader>("Game/Light")),

				//Post Lighting
				new PostProcessPass("PostLighting")
					.WithFramebuffer(lightingFramebuffer)
					.WithViewport(c => new RectInt(default,LightingSize()))
					.WithPassedTextures(
						lightingBuffer,
						emissionBuffer
					)
					.WithShaders(postLightingShader),
				
				//Composite
				new PostProcessPass("Composite")
					.WithFramebuffer(null)
					.WithPassedTextures(
						colorBuffer,
						emissionBuffer,
						lightingBuffer
					)
					.WithShaders(compositeShader),
			};
		}

		public override void PreRender()
		{
			//postLightingShader.SetFloat("zoom",Main.camera.zoomGoal);
			compositeShader.SetVector2("cameraPos",Main.camera.Position);
			compositeShader.SetFloat("zoom",Main.camera.zoomGoal);
		}
	}
}
