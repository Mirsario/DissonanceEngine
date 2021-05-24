using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics
{
	[RenderPassInfo(AcceptedShaderNames = new[] { "point", "directional", "spot" })]
	public class DeferredLightingPass : RenderPass
	{
		//public override string[] AcceptedShaderNames => Enum.GetNames(typeof(LightType)).Select(q => q.ToLower()).ToArray();

		//public Shader pointShader;
		//public Shader directionalShader;
		//public Shader spotShader;

		//public DeferredLightingPass(string name,Framebuffer framebuffer,RenderTexture[] renderTextures,Shader pointShader,Shader directionalShader,Shader spotShader) : base(name,framebuffer,renderTextures)
		//{
		//	this.pointShader = pointShader;
		//	this.directionalShader = directionalShader;
		//	this.spotShader = spotShader;
		//}

		public override void Render(World world)
		{
			Framebuffer.BindWithDrawBuffers(framebuffer);

			GL.Enable(EnableCap.Blend);
			GL.Enable(EnableCap.CullFace);
			GL.DepthMask(false);

			//test if it equals 1
			//GL.StencilFunc(StencilFunction.Notequal,0x01,0x01);
			//GL.StencilMask(0);

			Matrix4x4 worldMatrix, inverseWorldMatrix = default,
			worldViewMatrix = default, inverseWorldViewMatrix = default,
			worldViewProjMatrix = default, inverseWorldViewProjMatrix = default;

			foreach(var cameraEntity in world.ReadEntities()) {
				if(!cameraEntity.Has<Camera>() || !cameraEntity.Has<Transform>()) {
					continue;
				}

				var camera = cameraEntity.Get<Camera>();
				var cameraTransform = cameraEntity.Get<Transform>();
				var viewRect = camera.ViewPixel;

				GL.Viewport(viewRect.x, viewRect.y, viewRect.width, viewRect.height);

				var cameraPos = cameraTransform.Position;

				//Temporary
				var viewMatrix = camera.ViewMatrix;
				var projectionMatrix = camera.ProjectionMatrix;
				var inverseViewMatrix = camera.InverseViewMatrix;
				var inverseProjectionMatrix = camera.InverseProjectionMatrix;

				for(int i = 0; i < shaders.Length; i++) {
					var activeShader = shaders[i];

					if(activeShader == null) {
						continue;
					}

					Shader.SetShader(activeShader);

					activeShader.SetupCommonUniforms();
					activeShader.SetupCameraUniforms(camera, cameraPos);

					var lightType = (Light.LightType)i;

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

					foreach(var lightEntity in world.ReadEntities()) {
						if(!lightEntity.Has<Light>() || !lightEntity.Has<Transform>()) {
							continue;
						}

						var light = lightEntity.Get<Light>();

						if(light.Type != lightType) {
							continue;
						}

						var lightTransform = lightEntity.Get<Transform>();
						var lightPosition = lightTransform.Position;

						worldMatrix = Matrix4x4.CreateScale(light.Range) * Matrix4x4.CreateTranslation(lightPosition);

						activeShader.SetupMatrixUniforms(
							lightTransform,
							ref worldMatrix, ref inverseWorldMatrix,
							ref worldViewMatrix, ref inverseWorldViewMatrix,
							ref worldViewProjMatrix, ref inverseWorldViewProjMatrix,
							ref viewMatrix, ref inverseViewMatrix,
							ref projectionMatrix, ref inverseProjectionMatrix,
							true
						);

						if(uniformLightRange != -1) {
							GL.Uniform1(uniformLightRange, light.Range);
						}

						if(uniformLightPosition != -1) {
							GL.Uniform3(uniformLightPosition, lightPosition.x, lightPosition.y, lightPosition.z);
						}

						if(uniformLightDirection != -1) {
							var forward = lightTransform.Forward;

							GL.Uniform3(uniformLightDirection, forward.x, forward.y, forward.z);
						}

						if(uniformLightIntensity != -1) {
							GL.Uniform1(uniformLightIntensity, light.Intensity);
						}

						if(uniformLightColor != -1) {
							GL.Uniform3(uniformLightColor, light.Color.x, light.Color.y, light.Color.z);
						}

						switch(lightType) {
							case Light.LightType.Point:
								PrimitiveMeshes.IcoSphere.Render();
								break;
							case Light.LightType.Directional:
								PrimitiveMeshes.ScreenQuad.Render();
								break;
						}
					}
				}

				//Temporary
				camera.ViewMatrix = viewMatrix;
				camera.ProjectionMatrix = projectionMatrix;
				camera.InverseViewMatrix = inverseViewMatrix;
				camera.InverseProjectionMatrix = inverseProjectionMatrix;
			}

			GL.DepthMask(true);
			GL.Disable(EnableCap.Blend);
			GL.Disable(EnableCap.CullFace);

			Shader.SetShader(null);

			GL.BindTexture(TextureTarget.Texture2D, 0);
		}
	}
}
