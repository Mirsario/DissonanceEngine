using System;
using System.Collections.Generic;
using GameEngine;
using GameEngine.Graphics;
using GameEngine.Physics;

namespace AbyssCrusaders
{
	public class CustomRenderingPipeline : RenderingPipeline
	{
		private Shader compositeShader;
		
		public override void Setup(List<Framebuffer> framebuffers,List<RenderPass> renderPasses)
		{
			static Vector2Int ScreenSize() => Screen.Size;

			Framebuffer mainFramebuffer,lightingOcclusionFramebuffer,lightingFramebuffer;

			//RenderBuffers
			var depthBuffer = new RenderTexture("depthBuffer",ScreenSize,useMipmaps:false,textureFormat: TextureFormat.Depth32);

			//Screen Buffers
			var colorBuffer = new RenderTexture("colorBuffer",ScreenSize,FilterMode.Point);
			var emissionBuffer = new RenderTexture("emissionBuffer",ScreenSize,FilterMode.Point,TextureWrapMode.Clamp,false,TextureFormat.RGBA32f);

			//Terrain
			var terrainLightingDataBuffer = new RenderTexture("terrainLightingDataBuffer",ScreenSize,FilterMode.Bilinear,textureFormat:TextureFormat.RG8);

			//Lighting Buffers
			var lightingBuffer = new RenderTexture("lightingBuffer",ScreenSize,FilterMode.Point,textureFormat:TextureFormat.RGB8);


			//Framebuffers
			framebuffers.AddRange(new[] {
				mainFramebuffer = Framebuffer.Create("mainBuffer",fb => {
					fb.AttachRenderTextures(
						colorBuffer,
						emissionBuffer
					);
					fb.AttachRenderTexture(depthBuffer,FramebufferAttachment.DepthAttachment);
				}),

				lightingOcclusionFramebuffer = Framebuffer.Create("lightingOcclusionFramebuffer",fb => {
					fb.AttachRenderTexture(terrainLightingDataBuffer);
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
					p.layerMask = ulong.MaxValue^Layers.GetLayerMask("TerrainLighting");
				}),

				//Terrain Lighting Occlusion
				RenderPass.Create<GeometryPass>("TerrainLighting",p => {
					p.Framebuffer = lightingOcclusionFramebuffer;
					p.layerMask = Layers.GetLayerMask("TerrainLighting");
					//p.ViewportFunc = c => new RectInt(default,GetLightingResolution());
				}),
				
				//Lights
				RenderPass.Create<Light2DPass>("Lighting",p => {
					p.Framebuffer = lightingFramebuffer;
					p.Shader = Resources.Find<Shader>("Game/Light");
					//p.ViewportFunc = c => new RectInt(default,GetLightingResolution(true));
				}),

				//Post Lighting
				RenderPass.Create<PostProcessPass>("PostLighting",p => {
					p.Framebuffer = lightingFramebuffer;
					p.Shader = Resources.Find<Shader>("Game/PostLighting");
					p.PassedTextures = new[] {
						lightingBuffer,
						terrainLightingDataBuffer
					};
					//p.ViewportFunc = c => new RectInt(default,GetLightingResolution());
				}),
				
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

			/*float zoomGoal = Main.camera.zoomGoal;
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

			var offset = new Vector2(
				cameraPixelPosSnapped.x%zoomGoal,
				cameraPixelPosSnapped.y%zoomGoal
			);
			offset = new Vector2(zoomGoal,zoomGoal)-offset;
			//offset = Vector2.Floor(offset);
			Main.debugStrings[nameof(offset)] = offset.ToString();
			offset /= lightingResUpscaled;
			compositeShader.SetVector2("lightingOffset",Input.GetKey(Keys.Z) ? Vector2.Zero : -offset);*/
		}
		/*private static Vector2Int GetLightingResolution() => GetLightingResolution(false);
		private static Vector2Int GetLightingResolution(bool floor)
		{
			float zoom = Main.camera?.zoomGoal ?? 1f;
			float width = Screen.Width/zoom;
			float height = Screen.Height/zoom;
			return new Vector2Int(
				(int)(floor ? Math.Floor(width) : Math.Ceiling(width)),
				(int)(floor ? Math.Floor(height) : Math.Ceiling(height))
			);
		}*/
	}
}
