using Dissonance.Engine.IO;
using Silk.NET.OpenGL;
using static Dissonance.Engine.Graphics.OpenGLApi;

namespace Dissonance.Engine.Graphics;

public class DeferredLightingPass : RenderPass
{
	public readonly Asset<Shader>[] ShadersByLightType = new Asset<Shader>[2];

	public override void Render()
	{
		Framebuffer.BindWithDrawBuffers(Framebuffer);

		OpenGL.Enable(EnableCap.Blend);
		OpenGL.Enable(EnableCap.CullFace);
		OpenGL.CullFace(CullFaceMode.Back);
		OpenGL.DepthMask(false);

		// test if it equals 1
		// OpenGL.Api.StencilFunc(StencilFunction.Notequal, 0x01, 0x01);
		// OpenGL.Api.StencilMask(0);

		Matrix4x4 worldMatrix, inverseWorldMatrix = default,
		worldViewMatrix = default, inverseWorldViewMatrix = default,
		worldViewProjMatrix = default, inverseWorldViewProjMatrix = default;

		var renderViewData = GlobalGet<RenderViewData>();
		var lightingData = GlobalGet<LightingPassData>();

		foreach (var renderView in renderViewData.RenderViews) {
			var viewport = renderView.Viewport;

			OpenGL.Viewport(viewport.X, viewport.Y, (uint)viewport.Width, (uint)viewport.Height);

			var cameraTransform = renderView.Transform;
			var cameraPosition = cameraTransform.Position;

			for (int i = 0; i < ShadersByLightType.Length; i++) {
				if (!ShadersByLightType[i].TryGetOrRequestValue(out var activeShader)) {
					continue;
				}

				Shader.SetShader(activeShader);

				activeShader.SetupCommonUniforms();
				activeShader.SetupCameraUniforms(renderView.NearClip, renderView.FarClip, cameraPosition);

				var lightType = (Light.LightType)i;

				//TODO: Update & optimize this
				for (int j = 0; j < PassedTextures.Length; j++) {
					var tex = PassedTextures[j];

					OpenGL.ActiveTexture((TextureUnit)((int)TextureUnit.Texture0 + j));
					OpenGL.BindTexture(TextureTarget.Texture2D, tex.Id);

					if (activeShader.TryGetUniformLocation(tex.Name, out int location)) {
						OpenGL.Uniform1(location, j);
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

					activeShader.SetupMatrixUniforms(
						in worldMatrix, ref inverseWorldMatrix,
						ref worldViewMatrix, ref inverseWorldViewMatrix,
						ref worldViewProjMatrix, ref inverseWorldViewProjMatrix,
						renderView.ViewMatrix, renderView.InverseViewMatrix,
						renderView.ProjectionMatrix, renderView.InverseProjectionMatrix
					);

					if (uniformLightRange != -1 && light.Range.HasValue) {
						OpenGL.Uniform1(uniformLightRange, light.Range.Value);
					}

					if (uniformLightPosition != -1 && light.Position.HasValue) {
						var position = light.Position.Value;

						OpenGL.Uniform3(uniformLightPosition, position.X, position.Y, position.Z);
					}

					if (uniformLightDirection != -1 && light.Direction.HasValue) {
						var direction = light.Direction.Value;

						OpenGL.Uniform3(uniformLightDirection, direction.X, direction.Y, direction.Z);
					}

					if (uniformLightIntensity != -1) {
						OpenGL.Uniform1(uniformLightIntensity, light.Intensity);
					}

					if (uniformLightColor != -1) {
						OpenGL.Uniform3(uniformLightColor, light.Color.X, light.Color.Y, light.Color.Z);
					}

					switch (lightType) {
						case Light.LightType.Point:
							PrimitiveMeshes.IcoSphere.Value.Render();
							break;
						case Light.LightType.Directional:
							PrimitiveMeshes.ScreenQuad.Value.Render();
							break;
					}
				}
			}
		}

		OpenGL.DepthMask(true);
		OpenGL.Disable(EnableCap.Blend);
		OpenGL.Disable(EnableCap.CullFace);

		Shader.SetShader(null);

		OpenGL.BindTexture(TextureTarget.Texture2D, 0);
	}
}
