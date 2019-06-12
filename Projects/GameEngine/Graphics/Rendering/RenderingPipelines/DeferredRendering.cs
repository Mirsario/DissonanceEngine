using System;
using System.Linq;
using OpenTK.Graphics.OpenGL;

namespace GameEngine.Graphics.RenderingPipelines
{
	public class DeferredRendering : RenderingPipeline
	{
		public override void Setup(out Framebuffer[] framebuffers,out RenderPass[] renderPasses)
		{
			//Vector2Int ScreenSize() => new Vector2Int(Screen.Width,Screen.Height);

			Debug.Log($"Setting up '{GetType().Name}' rendering pipeline.");

			Framebuffer mainFramebuffer,lightingFramebuffer;

			//Framebuffers
			framebuffers = new[] {
				mainFramebuffer = new Framebuffer("mainBuffer")
					.WithRenderTexture(new RenderTexture("colorBuffer",() => Screen.Size,useMipmaps:false,textureFormat:TextureFormat.RGBA32f),out var colorBuffer)
					.WithRenderTexture(new RenderTexture("normalBuffer",() => Screen.Size,useMipmaps:false,textureFormat:TextureFormat.RGBA32f),out var normalBuffer)
					.WithRenderTexture(new RenderTexture("positionBuffer",() => Screen.Size,useMipmaps:false,textureFormat:TextureFormat.RGBA32f),out var positionBuffer)
					.WithRenderTexture(new RenderTexture("emissionBuffer",() => Screen.Size,useMipmaps:false,textureFormat:TextureFormat.RGBA32f),out var emissionBuffer)
					.WithRenderbuffer(new Renderbuffer("depthBuffer",RenderbufferStorage.DepthComponent32f),FramebufferAttachment.DepthAttachment),

				lightingFramebuffer = new Framebuffer("lightingBuffer")
					.WithRenderTexture(new RenderTexture("lightingBuffer",() => Screen.Size,useMipmaps:false,textureFormat:TextureFormat.RGBA32f),out var lightingTexture)
			};

			//RenderPasses
			renderPasses = new RenderPass[] {
				//Geometry
				new GeometryPass("Geometry")
					.WithFramebuffer(mainFramebuffer),
				
				//Lighting
				new DeferredLightingPass("Lighting")
					.WithFramebuffer(lightingFramebuffer)
					.WithPassedTextures(
						normalBuffer,
						positionBuffer,
						emissionBuffer
					)
					.WithShaders(
						Resources.Find<Shader>("LightingPoint"),
						Resources.Find<Shader>("LightingDirectional"),
						null
					),
				
				//Composite
				new PostProcessPass("Composite")
					.WithFramebuffer(null)
					.WithPassedTextures(
						colorBuffer,
						emissionBuffer,
						lightingTexture
					)
					.WithShaders(Resources.Find<Shader>("Composite")),
			};

			Framebuffer.Bind(null);
		}
		public override void Resize()
		{
			/*Vector2Int screenSize = Screen.size;
			
			for(int i = 0;i<framebuffers.Length;i++) {
				var fb = framebuffers[i];
				for(int j = 0;j<fb.renderTextures.Length;j++) {
					fb.renderTextures[j].Resize(screenSize.x,screenSize.y);
				}
			}*/

			for(int i = 0;i<framebuffers.Length;i++) {
				var fb = framebuffers[i];
				for(int j = 0;j<fb.renderTextures.Length;j++) {
					var rt = fb.renderTextures[j];
					if(rt.UpdateSize()) {
						rt.Resize(rt.Width,rt.Height);
					}
				}
			}
		}
	}
}