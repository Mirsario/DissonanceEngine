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
			//var resLighting = Screen.Size/4;

			static Vector2Int LightingSize() => new Vector2Int((int)(Screen.Width/(Main.camera?.zoomGoal ?? 1f)),(int)(Screen.Height/(Main.camera?.zoomGoal ?? 1f)));

			//Framebuffers
			framebuffers = new[] {
				mainFramebuffer = new Framebuffer("mainBuffer")
					.WithRenderTexture(new RenderTexture("colorBuffer",resNormal.x,resNormal.y),out var colorBuffer)
					.WithRenderTexture(new RenderTexture("emissionBuffer",resNormal.x,resNormal.y),out var emissionBuffer)
					.WithRenderbuffer(new Renderbuffer("depthBuffer",RenderbufferStorage.DepthComponent32f),FramebufferAttachment.DepthAttachment),

				lightingFramebuffer = new Framebuffer("lightingBuffer")
					.WithRenderTexture(new RenderTexture("lightingBuffer",LightingSize),out var lightingBuffer)
			};

			//RenderPasses
			renderPasses = new[] {
				//Geometry
				new GeometryPass("Geometry")
					.WithFramebuffer(mainFramebuffer),
				
				//Lighting
				new Light2DPass("Lighting")
					.WithFramebuffer(lightingFramebuffer)
					.WithViewport(c => {
						if(c!=null) {
							var view = c.ViewPixel;
							var cam = Main.camera;
							float zoom = cam.zoom;
							return new RectInt(
								(int)(Math.Floor(view.x/zoom)*zoom),(int)(Math.Floor(view.y/zoom)*zoom),
								(int)(view.width/zoom),				(int)(view.height/zoom)
							);
						}
						return Screen.Rectangle;
					})
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
	}
}
