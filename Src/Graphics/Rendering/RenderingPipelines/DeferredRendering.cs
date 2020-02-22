using Dissonance.Engine.IO;
using Dissonance.Framework.Graphics;
using System.Collections.Generic;

namespace Dissonance.Engine.Graphics.RenderingPipelines
{
	public class DeferredRendering : RenderingPipeline
	{
		public override void Setup(List<Framebuffer> framebuffers,List<RenderPass> renderPasses)
		{
			Debug.Log($"Setting up '{GetType().Name}' rendering pipeline.");

			Framebuffer mainFramebuffer,lightingFramebuffer;

			static Vector2Int ScreenSize() => Screen.Size;

			var colorBuffer = new RenderTexture("colorBuffer",ScreenSize,useMipmaps:false,textureFormat:TextureFormat.RGBA32f);
			var normalBuffer = new RenderTexture("normalBuffer",ScreenSize,useMipmaps:false,textureFormat:TextureFormat.RGBA32f);
			var positionBuffer = new RenderTexture("positionBuffer",ScreenSize,useMipmaps:false,textureFormat:TextureFormat.RGBA32f);
			var emissionBuffer = new RenderTexture("emissionBuffer",ScreenSize,useMipmaps:false,textureFormat:TextureFormat.RGBA32f);
			var specularBuffer = new RenderTexture("specularBuffer",ScreenSize,useMipmaps:false,textureFormat:TextureFormat.R32f);

			var depthBuffer = new RenderTexture("depthBuffer",ScreenSize,useMipmaps:false,textureFormat:TextureFormat.Depth32);

			var lightingBuffer = new RenderTexture("lightingBuffer",ScreenSize,useMipmaps:false,textureFormat:TextureFormat.RGBA32f);

			//Framebuffers
			framebuffers.AddRange(new[] {
				mainFramebuffer = Framebuffer.Create("mainBuffer",fb => {
					fb.AttachRenderTextures(
						colorBuffer,
						normalBuffer,
						positionBuffer,
						emissionBuffer,
						specularBuffer
					);

					fb.AttachRenderTexture(depthBuffer,FramebufferAttachment.DepthAttachment);
				}),

				lightingFramebuffer = Framebuffer.Create("lightingBuffer",fb => {
					fb.AttachRenderTexture(lightingBuffer);
				})
			});

			//RenderPasses
			renderPasses.AddRange(new RenderPass[] {
				//Geometry
				RenderPass.Create<GeometryPass>("Geometry",p => {
					p.Framebuffer = mainFramebuffer;
				}),
				
				//Lighting
				RenderPass.Create<DeferredLightingPass>("Lighting",p => {
					p.Framebuffer = lightingFramebuffer;
					p.Shaders = new[] {
						Resources.Find<Shader>("LightingPoint"),
						Resources.Find<Shader>("LightingDirectional"),
						null
					};
					p.PassedTextures = new[] {
						normalBuffer,
						positionBuffer,
						emissionBuffer,
						specularBuffer
					};
				}),
				
				//Composite
				RenderPass.Create<PostProcessPass>("Composite",p => {
					p.Shader = Resources.Find<Shader>("Composite");
					p.PassedTextures = new[] {
						colorBuffer,
						normalBuffer,
						emissionBuffer,
						lightingBuffer
					};
				}),

				//GUI
				RenderPass.Create<GUIPass>("GUI")
			});

			Framebuffer.Bind(null);
		}
	}
}