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

			Framebuffer mainFramebuffer,lightingFramebuffer;

			//Framebuffers
			framebuffers = new[] {
				mainFramebuffer = new Framebuffer("mainBuffer")
					.WithRenderTexture(new RenderTexture("colorBuffer",Screen.Width,Screen.Height,useMipmaps:false,textureFormat:TextureFormat.RGBA32f),out var colorBuffer)
					.WithRenderTexture(new RenderTexture("normalBuffer",Screen.Width,Screen.Height,useMipmaps:false,textureFormat:TextureFormat.RGBA32f),out var normalBuffer)
					.WithRenderTexture(new RenderTexture("positionBuffer",Screen.Width,Screen.Height,useMipmaps:false,textureFormat:TextureFormat.RGBA32f),out var positionBuffer)
					.WithRenderTexture(new RenderTexture("emissionBuffer",Screen.Width,Screen.Height,useMipmaps:false,textureFormat:TextureFormat.RGBA32f),out var emissionBuffer)
					.WithRenderbuffer(new Renderbuffer("depthBuffer",RenderbufferStorage.DepthComponent32f),FramebufferAttachment.DepthAttachment),

				lightingFramebuffer = new Framebuffer("lightingBuffer")
					.WithRenderTexture(new RenderTexture("lightingBuffer",Screen.Width,Screen.Height,useMipmaps:false,textureFormat:TextureFormat.RGBA32f),out var lightingTexture)
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
				
				//FXAA test, to be merged with Composite
				/*new PostProcessPass("FXAA")
					.WithFramebuffer(mainFramebuffer)
					.WithPassedTextures(
						colorBuffer,
						normalBuffer,
						positionBuffer,
						emissionBuffer
					)
					.WithShaders(Resources.Find<Shader>("FXAA")),*/
				
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
		}
	}
}