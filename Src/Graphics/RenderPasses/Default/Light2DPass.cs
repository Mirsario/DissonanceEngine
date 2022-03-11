namespace Dissonance.Engine.Graphics
{
	/*public class Light2DPass : RenderPass
	{
		public override void Render()
		{
			Framebuffer.BindWithDrawBuffers(framebuffer);

			OpenGL.Api.Enable(EnableCap.Blend);
			OpenGL.Api.Enable(EnableCap.CullFace);
			OpenGL.Api.DepthMask(false);

			Matrix4x4 worldInverse = default,
			worldView = default, worldViewInverse = default,
			worldViewProj = default, worldViewProjInverse = default;

			Shader.SetShader(passShader);

			passShader.SetupCommonUniforms();

			foreach (var camera in ComponentManager.EnumerateComponents<Camera>()) {
				var viewport = GetViewport(camera);
				OpenGL.Api.Viewport(viewport.x, viewport.y, viewport.width, viewport.height);

				var cameraPos = camera.Transform.Position;

				passShader.SetupCameraUniforms(camera, cameraPos);

				if (passedTextures != null) {
					for (int j = 0; j < passedTextures.Length; j++) {
						OpenGL.Api.ActiveTexture((TextureUnit)((int)TextureUnit.Texture0 + j));
						OpenGL.Api.BindTexture(TextureTarget.Texture2D, passedTextures[j].Id);

						OpenGL.Api.Uniform1(OpenGL.Api.GetUniformLocation(passShader.Id, passedTextures[j].name), j);
					}
				}

				int uniformLightRange = OpenGL.Api.GetUniformLocation(passShader.Id, "lightRange");
				int uniformLightPosition = OpenGL.Api.GetUniformLocation(passShader.Id, "lightPosition");
				int uniformLightIntensity = OpenGL.Api.GetUniformLocation(passShader.Id, "lightIntensity");
				int uniformLightColor = OpenGL.Api.GetUniformLocation(passShader.Id, "lightColor");

				foreach (var light in ComponentManager.EnumerateComponents<Light2D>()) {
					var lightPos = light.Transform.Position;
					var world = Matrix4x4.CreateScale(light.range + 1f) * Matrix4x4.CreateTranslation(lightPos);

					passShader.SetupMatrixUniforms(
						light.Transform,
						ref world, ref worldInverse,
						ref worldView, ref worldViewInverse,
						ref worldViewProj, ref worldViewProjInverse,
						ref camera.matrix_view, ref camera.matrix_viewInverse,
						ref camera.matrix_proj, ref camera.matrix_projInverse,
						true
					);

					unsafe {
						OpenGL.Api.Uniform1(uniformLightRange, light.range);
						OpenGL.Api.Uniform1(uniformLightIntensity, light.intensity);
						OpenGL.Api.Uniform3(uniformLightPosition, lightPos.x, lightPos.y, lightPos.z);
						OpenGL.Api.Uniform3(uniformLightColor, light.color.x, light.color.y, light.color.z);
					}

					PrimitiveMeshes.ScreenQuad.Render();
				}
			}

			OpenGL.Api.DepthMask(true);
			OpenGL.Api.Disable(EnableCap.Blend);
			OpenGL.Api.Disable(EnableCap.CullFace);
			Shader.SetShader(null);
			OpenGL.Api.BindTexture(TextureTarget.Texture2D, 0);
		}
	}*/
}
