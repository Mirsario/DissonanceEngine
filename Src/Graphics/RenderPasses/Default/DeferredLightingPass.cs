﻿using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics
{
	[RenderPassInfo(AcceptedShaderNames = new[] { "point", "directional", "spot" })]
	public class DeferredLightingPass : RenderPass
	{
		//public override string[] AcceptedShaderNames => Enum.GetNames(typeof(LightType)).Select(q => q.ToLower()).ToArray();

		//public Shader pointShader;
		//public Shader directionalShader;
		//public Shader spotShader;

		/*public DeferredLightingPass(string name,Framebuffer framebuffer,RenderTexture[] renderTextures,Shader pointShader,Shader directionalShader,Shader spotShader) : base(name,framebuffer,renderTextures)
		{
			this.pointShader = pointShader;
			this.directionalShader = directionalShader;
			this.spotShader = spotShader;
		}*/

		public override void Render()
		{
			Framebuffer.BindWithDrawBuffers(framebuffer);

			GL.Enable(EnableCap.Blend);
			GL.Enable(EnableCap.CullFace);
			GL.DepthMask(false);

			//test if it equals 1
			//GL.StencilFunc(StencilFunction.Notequal,0x01,0x01);
			//GL.StencilMask(0);

			Matrix4x4 worldInverse = default,
			worldView = default, worldViewInverse = default,
			worldViewProj = default, worldViewProjInverse = default;

			foreach(var camera in ComponentManager.EnumerateComponents<Camera>()) {
				var viewRect = camera.ViewPixel;
				GL.Viewport(viewRect.x, viewRect.y, viewRect.width, viewRect.height);

				var cameraPos = camera.Transform.Position;

				for(int i = 0; i < shaders.Length; i++) {
					var activeShader = shaders[i];

					if(activeShader == null) {
						continue;
					}

					Shader.SetShader(activeShader);

					activeShader.SetupCommonUniforms();
					activeShader.SetupCameraUniforms(camera, cameraPos);

					var lightType = (Light.Type)i;

					//TODO: Update & optimize this
					for(int j = 0; j < passedTextures.Length; j++) {
						var tex = passedTextures[j];
						GL.ActiveTexture((TextureUnit)((int)TextureUnit.Texture0 + j));
						GL.BindTexture(TextureTarget.Texture2D, tex.Id);

						if(activeShader.TryGetUniformLocation(tex.name, out int location)) {
							GL.Uniform1(location, j);
						}
					}

					//TODO: Update & optimize this
					activeShader.TryGetUniformLocation("lightRange", out int uniformLightRange);
					activeShader.TryGetUniformLocation("lightPosition", out int uniformLightPosition);
					activeShader.TryGetUniformLocation("lightDirection", out int uniformLightDirection);
					activeShader.TryGetUniformLocation("lightIntensity", out int uniformLightIntensity);
					activeShader.TryGetUniformLocation("lightColor", out int uniformLightColor);

					foreach(var light in ComponentManager.EnumerateComponents<Light>()) {
						if(light.type != lightType) {
							continue;
						}

						var lightTransform = light.Transform;
						var lightPosition = lightTransform.Position;

						var world = Matrix4x4.CreateScale(light.range) * Matrix4x4.CreateTranslation(lightPosition);

						activeShader.SetupMatrixUniforms(
							lightTransform,
							ref world, ref worldInverse,
							ref worldView, ref worldViewInverse,
							ref worldViewProj, ref worldViewProjInverse,
							ref camera.matrix_view, ref camera.matrix_viewInverse,
							ref camera.matrix_proj, ref camera.matrix_projInverse,
							true
						);

						if(uniformLightRange != -1) {
							GL.Uniform1(uniformLightRange, light.range);
						}

						if(uniformLightPosition != -1) {
							GL.Uniform3(uniformLightPosition, lightPosition.x, lightPosition.y, lightPosition.z);
						}

						if(uniformLightDirection != -1) {
							var forward = lightTransform.Forward;

							GL.Uniform3(uniformLightDirection, forward.x, forward.y, forward.z);
						}

						if(uniformLightIntensity != -1) {
							GL.Uniform1(uniformLightIntensity, light.intensity);
						}

						if(uniformLightColor != -1) {
							GL.Uniform3(uniformLightColor, light.color.x, light.color.y, light.color.z);
						}

						switch(lightType) {
							case Light.Type.Point:
								PrimitiveMeshes.IcoSphere.Render();
								break;
							case Light.Type.Directional:
								PrimitiveMeshes.ScreenQuad.Render();
								break;
						}
					}
				}
			}

			GL.DepthMask(true);
			GL.Disable(EnableCap.Blend);
			GL.Disable(EnableCap.CullFace);

			Shader.SetShader(null);

			GL.BindTexture(TextureTarget.Texture2D, 0);
		}
	}
}
