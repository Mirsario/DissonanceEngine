using System;
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
			static Vector2Int LightingSize()
			{
				float zoom = Main.camera?.zoomGoal ?? 1f;
				return new Vector2Int((int)Math.Ceiling(Screen.Width/zoom),(int)Math.Ceiling(Screen.Height/zoom));
			}

			var colorBuffer = new RenderTexture("colorBuffer",ScreenSize);
			var emissionBuffer = new RenderTexture("emissionBuffer",ScreenSize,FilterMode.Bilinear,TextureWrapMode.Clamp,false,TextureFormat.RGBA32f);
			var depthBuffer = new Renderbuffer("depthBuffer",RenderbufferStorage.DepthComponent32f);
			var lightingBuffer = new RenderTexture("lightingBuffer",LightingSize,textureFormat:TextureFormat.RGB8);

			//Framebuffers
			framebuffers = new[] {
				mainFramebuffer = Framebuffer.Create("mainBuffer",fb => {
					fb.AttachRenderTextures(
						colorBuffer,
						emissionBuffer
					);
					fb.AttachRenderbuffer(depthBuffer,FramebufferAttachment.DepthAttachment);
				}),

				lightingFramebuffer = Framebuffer.Create("lightingBuffer",fb => {
					fb.AttachRenderTexture(lightingBuffer);
				})
			};

			postLightingShader = Resources.Find<Shader>("Game/PostLighting");
			compositeShader = Resources.Find<Shader>("Game/Composite");

			//RenderPasses
			renderPasses = new RenderPass[] {
				//Geometry
				RenderPass.Create<GeometryPass>("Geometry",p => {
					p.Framebuffer = mainFramebuffer;
				}),
				
				//Lights
				RenderPass.Create<Light2DPass>("Lighting",p => {
					p.Framebuffer = lightingFramebuffer;
					p.ViewportFunc = c => new RectInt(default,LightingSize());
					p.Shader = Resources.Find<Shader>("Game/Light");
				}),

				//Post Lighting
				RenderPass.Create<PostProcessPass>("PostLighting",p => {
					p.Framebuffer = lightingFramebuffer;
					p.ViewportFunc = c => new RectInt(default,LightingSize());
					p.Shader = postLightingShader;
					p.PassedTextures = new[] {
						lightingBuffer,
						emissionBuffer
					};
				}),
				
				//Composite
				RenderPass.Create<PostProcessPass>("Composite",p => {
					p.Framebuffer = null;
					p.PassedTextures = new[] {
						colorBuffer,
						emissionBuffer,
						lightingBuffer
					};
					p.Shader = compositeShader;
				}),

				//GUI
				RenderPass.Create<GUIPass>("GUI")
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
