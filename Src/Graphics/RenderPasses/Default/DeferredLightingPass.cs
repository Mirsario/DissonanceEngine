using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics
{
	public class DeferredLightingPass : RenderPass
	{
		public readonly Shader[] ShadersByLightType = new Shader[2];

		public override void Render()
		{
			Framebuffer.BindWithDrawBuffers(Framebuffer);

			GL.Enable(EnableCap.Blend);
			GL.Enable(EnableCap.CullFace);
			GL.CullFace(CullFaceMode.Back);
			GL.DepthMask(false);

			// test if it equals 1
			// GL.StencilFunc(StencilFunction.Notequal, 0x01, 0x01);
			// GL.StencilMask(0);

			Matrix4x4 worldMatrix, inverseWorldMatrix = default,
			worldViewMatrix = default, inverseWorldViewMatrix = default,
			worldViewProjMatrix = default, inverseWorldViewProjMatrix = default;

			var renderViewData = GlobalGet<RenderViewData>();
			var lightingData = GlobalGet<LightingPassData>();

			foreach (var renderView in renderViewData.RenderViews) {
				var viewport = renderView.Viewport;

				GL.Viewport(viewport.x, viewport.y, viewport.width, viewport.height);

				var cameraTransform = renderView.Transform;
				var cameraPosition = cameraTransform.Position;

				for (int i = 0; i < ShadersByLightType.Length; i++) {
					var activeShader = ShadersByLightType[i];

					if (activeShader == null) {
						continue;
					}

					Shader.SetShader(activeShader);

					//activeShader.SetupDefaultUniforms();
					//activeShader.SetupCameraUniforms(renderView.NearClip, renderView.FarClip, cameraPosition);

					var lightType = (Light.LightType)i;

					//TODO: Update & optimize this
					for (int j = 0; j < PassedTextures.Length; j++) {
						var tex = PassedTextures[j];

						GL.ActiveTexture((TextureUnit)((int)TextureUnit.Texture0 + j));
						GL.BindTexture(TextureTarget.Texture2D, tex.Id);

						if (activeShader.TryGetUniformLocation(tex.Name, out int location)) {
							GL.Uniform1(location, j);
						}
					}

					//TODO: Update & optimize this
					activeShader.TryGetUniformLocation("lightRange", out int uniformLightRange);
					activeShader.TryGetUniformLocation("lightPosition", out int uniformLightPosition);
					activeShader.TryGetUniformLocation("lightDirection", out int uniformLightDirection);
					activeShader.TryGetUniformLocation("lightIntensity", out int uniformLightIntensity);
					activeShader.TryGetUniformLocation("lightColor", out int uniformLightColor);

					foreach (var light in lightingData.Lights) {
						if (light.Type != lightType) {
							continue;
						}

						worldMatrix = light.Matrix;

						/*
						activeShader.SetupMatrixUniforms(
							in worldMatrix, ref inverseWorldMatrix,
							ref worldViewMatrix, ref inverseWorldViewMatrix,
							ref worldViewProjMatrix, ref inverseWorldViewProjMatrix,
							renderView.ViewMatrix, renderView.InverseViewMatrix,
							renderView.ProjectionMatrix, renderView.InverseProjectionMatrix
						);
						*/

						if (uniformLightRange != -1 && light.Range.HasValue) {
							GL.Uniform1(uniformLightRange, light.Range.Value);
						}

						if (uniformLightPosition != -1 && light.Position.HasValue) {
							var position = light.Position.Value;

							GL.Uniform3(uniformLightPosition, position.x, position.y, position.z);
						}

						if (uniformLightDirection != -1 && light.Direction.HasValue) {
							var direction = light.Direction.Value;

							GL.Uniform3(uniformLightDirection, direction.x, direction.y, direction.z);
						}

						if (uniformLightIntensity != -1) {
							GL.Uniform1(uniformLightIntensity, light.Intensity);
						}

						if (uniformLightColor != -1) {
							GL.Uniform3(uniformLightColor, light.Color.x, light.Color.y, light.Color.z);
						}

						switch (lightType) {
							case Light.LightType.Point:
								PrimitiveMeshes.IcoSphere.Render();
								break;
							case Light.LightType.Directional:
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
