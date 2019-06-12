using System;
using System.Linq;
using GameEngine;
using GameEngine.Graphics;

namespace AbyssCrusaders
{
	public class CustomRenderPipeline : RenderingPipeline
	{
		public override void Setup(out Framebuffer[] framebuffers,out RenderPass[] renderPasses)
		{
			//Vector2Int ScreenSize() => new Vector2Int(Screen.Width,Screen.Height);

			Framebuffer mainFramebuffer,lightingFramebuffer;

			var resNormal = Screen.Size;
			var resLighting = Screen.Size/4;

			//Framebuffers
			framebuffers = new[] {
				mainFramebuffer = new Framebuffer("mainBuffer")
					.WithRenderTexture(new RenderTexture("colorBuffer",resNormal.x,resNormal.y),out var colorBuffer)
					.WithRenderTexture(new RenderTexture("emissionBuffer",resLighting.x,resLighting.y),out var emissionBuffer)
					.WithRenderbuffer(new Renderbuffer("depthBuffer",RenderbufferStorage.DepthComponent32f),FramebufferAttachment.DepthAttachment),

				lightingFramebuffer = new Framebuffer("lightingBuffer")
					.WithRenderTexture(new RenderTexture("lightingBuffer",resLighting.x,resLighting.y),out var lightingBuffer)
			};

			//RenderPasses
			renderPasses = new RenderPass[] {
				//Geometry
				new GeometryPass("Geometry")
					.WithFramebuffer(mainFramebuffer),
				
				//Lighting
				new Light2DPass("Lighting")
					.WithFramebuffer(lightingFramebuffer)
					.WithShaders(Resources.Find<Shader>("Game/Light")),

				//Lighting
				new PostProcessPass("PostLighting")
					.WithFramebuffer(lightingFramebuffer)
					.WithPassedTextures(
						lightingBuffer,
						emissionBuffer
					)
					.WithShaders(Resources.Find<Shader>("Game/PostLighting")),
				
				//Composite
				new PostProcessPass("Composite")
					.WithFramebuffer(null)
					.WithPassedTextures(
						colorBuffer,
						lightingBuffer
					)
					.WithShaders(Resources.Find<Shader>("Game/Composite")),
			};
		}
		public override void Resize()
		{
			
		}
	}
}
