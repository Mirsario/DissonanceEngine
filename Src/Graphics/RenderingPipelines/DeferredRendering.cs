﻿using Dissonance.Engine.IO;
using Dissonance.Framework.Graphics;
using System.Collections.Generic;

namespace Dissonance.Engine.Graphics
{
	public class DeferredRendering : RenderingPipeline
	{
		public override void Setup(List<Framebuffer> framebuffers, List<RenderPass> renderPasses)
		{
			Debug.Log($"Setting up '{GetType().Name}' rendering pipeline.");

			Framebuffer gFramebuffer, lightingFramebuffer;

			static Vector2Int ScreenSize() => Screen.Size;

			var colorBuffer = new RenderTexture("colorBuffer", ScreenSize, useMipmaps: false, textureFormat: TextureFormat.RGBA32f);
			var normalBuffer = new RenderTexture("normalBuffer", ScreenSize, useMipmaps: false, textureFormat: TextureFormat.RGB32f);
			var positionBuffer = new RenderTexture("positionBuffer", ScreenSize, useMipmaps: false, textureFormat: TextureFormat.RGB32f);
			var emissionBuffer = new RenderTexture("emissionBuffer", ScreenSize, useMipmaps: false, textureFormat: TextureFormat.RGB32f);
			var specularBuffer = new RenderTexture("specularBuffer", ScreenSize, useMipmaps: false, textureFormat: TextureFormat.R32f);

			var depthBuffer = new RenderTexture("depthBuffer", ScreenSize, useMipmaps: false, textureFormat: TextureFormat.Depth32);

			var lightingDiffuseBuffer = new RenderTexture("lightingDiffuseBuffer", ScreenSize, useMipmaps: false, textureFormat: TextureFormat.RGB32f);
			var lightingSpecularBuffer = new RenderTexture("lightingSpecularBuffer", ScreenSize, useMipmaps: false, textureFormat: TextureFormat.RGB32f);

			//Framebuffers
			framebuffers.AddRange(new[] {
				gFramebuffer = Framebuffer.Create("G-Buffer", fb => {
					fb.AttachRenderTextures(
						colorBuffer,
						normalBuffer,
						positionBuffer,
						emissionBuffer,
						specularBuffer
					);

					fb.AttachRenderTexture(depthBuffer,FramebufferAttachment.DepthAttachment);
				}),

				lightingFramebuffer = Framebuffer.Create("Lighting", fb => fb.AttachRenderTextures(
					lightingDiffuseBuffer,
					lightingSpecularBuffer
				))
			});

			//RenderPasses
			renderPasses.AddRange(new RenderPass[] {
				//Geometry
				RenderPass.Create<GeometryPass>("Geometry", p => p.Framebuffer = gFramebuffer),
				
				//Lighting
				RenderPass.Create<DeferredLightingPass>("Lighting", p => {
					p.Framebuffer = lightingFramebuffer;
					p.Shaders = new[] {
						Resources.Find<Shader>("LightingPoint"),
						Resources.Find<Shader>("LightingDirectional"),
						null
					};
					p.PassedTextures = new[] {
						positionBuffer,
						normalBuffer,
						specularBuffer
					};
				}),
				
				//Composite
				RenderPass.Create<PostProcessPass>("Composite", p => {
					p.Shader = Resources.Find<Shader>("Composite");
					p.PassedTextures = new[] {
						colorBuffer,
						normalBuffer, //temp.
						emissionBuffer,
						lightingDiffuseBuffer,
						lightingSpecularBuffer
					};
				}),

				//GUI
				RenderPass.Create<GUIPass>("GUI")
			});

			Framebuffer.Bind(null);
		}
	}
}
