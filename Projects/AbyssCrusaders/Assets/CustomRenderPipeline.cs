using System;
using System.Collections.Generic;
using GameEngine;
using GameEngine.Graphics;
using GameEngine.Physics;

namespace AbyssCrusaders
{
	public class CustomRenderPipeline : RenderingPipeline
	{
		private Shader postLightingShader;
		private Shader compositeShader;
		
		public override void Setup(List<Framebuffer> framebuffers,List<RenderPass> renderPasses)
		{
			//Vector2Int ScreenSize() => new Vector2Int(Screen.Width,Screen.Height);

			Framebuffer mainFramebuffer,lightingOcclusionFramebuffer,lightingFramebuffer;

			var colorBuffer = new RenderTexture("colorBuffer",GetScreenSize,FilterMode.Bilinear);
			var emissionBuffer = new RenderTexture("emissionBuffer",GetScreenSize,FilterMode.Bilinear,TextureWrapMode.Clamp,false,TextureFormat.RGBA32f);
			var depthBuffer = new Renderbuffer("depthBuffer",RenderbufferStorage.DepthComponent32f);

			var lightingOcclusionBuffer = new RenderTexture("lightingOcclusionBuffer",GetLightingResolution,textureFormat:TextureFormat.R8);

			var lightingBuffer = new RenderTexture("lightingBuffer",GetLightingResolution,textureFormat:TextureFormat.RGB8);

			//Framebuffers
			framebuffers.AddRange(new[] {
				mainFramebuffer = Framebuffer.Create("mainBuffer",fb => {
					fb.AttachRenderTextures(
						colorBuffer,
						emissionBuffer
					);
					fb.AttachRenderbuffer(depthBuffer,FramebufferAttachment.DepthAttachment);
				}),

				lightingOcclusionFramebuffer = Framebuffer.Create("lightingOcclusionFramebuffer",fb => {
					fb.AttachRenderTexture(lightingOcclusionBuffer);
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
					p.layerMask = ulong.MaxValue^Layers.GetLayerMask("TerrainLightingOcclusion");
				}),

				//Terrain Lighting Occlusion
				/*RenderPass.Create<GeometryPass>("TerrainLightingOcclusion",p => {
					p.Framebuffer = lightingOcclusionFramebuffer;
					p.ViewportFunc = c => new RectInt(default,GetLightingResolution());
					p.layerMask = Layers.GetLayerMask("TerrainLightingOcclusion");
				}),*/
				
				//Lights
				RenderPass.Create<Light2DPass>("Lighting",p => {
					p.Framebuffer = lightingFramebuffer;
					p.ViewportFunc = c => new RectInt(default,GetLightingResolution());
					p.Shader = Resources.Find<Shader>("Game/Light");
				}),

				//Post Lighting
				/*RenderPass.Create<PostProcessPass>("PostLighting",p => {
					p.Framebuffer = lightingFramebuffer;
					p.ViewportFunc = c => new RectInt(default,GetLightingResolution());
					p.Shader = postLightingShader = Resources.Find<Shader>("Game/PostLighting");
					p.PassedTextures = new[] {
						lightingBuffer,
						lightingOcclusionBuffer
					};
				}),*/
				
				//Composite
				RenderPass.Create<PostProcessPass>("Composite",p => {
					p.Shader = compositeShader = Resources.Find<Shader>("Game/Composite");
					p.PassedTextures = new[] {
						colorBuffer,
						emissionBuffer,
						lightingBuffer
					};
				}),

				//GUI
				RenderPass.Create<GUIPass>("GUI")
			});
		}

		public override void PreRender()
		{
			//postLightingShader.SetFloat("zoom",Main.camera.zoomGoal);
			//postLightingShader.SetVector2("cameraPos",Main.camera.Position);

			float zoomGoal = Main.camera.zoomGoal;
			compositeShader.SetFloat("zoom",zoomGoal);

			var lightingRes = (Vector2)GetLightingResolution();
			compositeShader.SetVector2("lightingResolution",lightingRes);
			Main.debugStrings[nameof(lightingRes)] = lightingRes.ToString();

			var lightingResUpscaled = lightingRes*zoomGoal;
			Main.debugStrings[nameof(lightingResUpscaled)] = lightingResUpscaled.ToString();
			Main.debugStrings["screenResolution"] = Screen.Size.ToString();

			var cameraPos = Main.camera.rect.Position;
			compositeShader.SetVector2("cameraPos",cameraPos);
			Main.debugStrings[nameof(cameraPos)] = cameraPos.ToString();

			var cameraPixelPos = cameraPos*Main.UnitSizeInPixels;
			var cameraPixelPosSnapped = Vector2.Floor(cameraPixelPos);
			//var resDiffScale = (lightingRes*zoomGoal)/(Vector2)Screen.Size;
			Main.debugStrings[nameof(cameraPixelPos)] = cameraPixelPos.ToString();
			Main.debugStrings[nameof(cameraPixelPosSnapped)] = cameraPixelPosSnapped.ToString();

			//var offset = (cameraPixelPos-cameraPixelPosSnapped)*zoomGoal; //()/lightingRes*resDiffScale
			var offset = new Vector2(
				cameraPixelPosSnapped.x%zoomGoal,
				cameraPixelPosSnapped.y%zoomGoal
			);
			offset = new Vector2(zoomGoal,zoomGoal)-offset;
			//offset = Vector2.Floor(offset);
			Main.debugStrings[nameof(offset)] = offset.ToString();
			offset /= lightingResUpscaled;
			compositeShader.SetVector2("lightingOffset",Input.GetKey(Keys.Z) ? Vector2.Zero : -offset);
		}

		private static Vector2Int GetScreenSize() => Screen.Size;
		private static Vector2Int GetLightingResolution()
		{
			float zoom = Main.camera?.zoomGoal ?? 1f;
			return new Vector2Int((int)Math.Ceiling(Screen.Width/zoom),(int)Math.Ceiling(Screen.Height/zoom));
		}
	}
}
