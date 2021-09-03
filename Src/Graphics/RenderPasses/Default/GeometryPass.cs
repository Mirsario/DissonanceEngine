using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics
{
	public class GeometryPass : RenderPass
	{
		//public ulong? layerMask;

		public override void Render()
		{
			Framebuffer.BindWithDrawBuffers(Framebuffer);

			GL.Enable(EnableCap.CullFace);
			GL.Enable(EnableCap.DepthTest);
			GL.Enable(EnableCap.Blend);
			GL.CullFace(CullFaceMode.Back);

			bool[] uniformComputed = new bool[DefaultShaderUniforms.Count];
			Matrix4x4 worldMatrix = default, inverseWorldMatrix = default,
			worldViewMatrix = default, inverseWorldViewMatrix = default,
			worldViewProjMatrix = default, inverseWorldViewProjMatrix = default;

			// Render cache
			Shader lastShader = null;
			Material lastMaterial = null;
			var lastCullMode = CullMode.Back;
			var lastPolygonMode = PolygonMode.Fill;

			var renderViewData = GlobalGet<RenderViewData>();
			var geometryPassData = GlobalGet<GeometryPassData>();

			// CameraLoop
			foreach (var renderView in renderViewData.RenderViews) {
				var camera = renderView.camera;
				var cameraTransform = renderView.transform;
				var viewport = GetViewport(camera);

				GL.Viewport(viewport.x, viewport.y, viewport.width, viewport.height);

				var cameraPos = cameraTransform.Position;

				// Render
				for (int i = 0; i < geometryPassData.RenderEntries.Count; i++) {
					var entry = geometryPassData.RenderEntries[i];
					var material = entry.material;
					var shader = material.Shader;
					var rendererTransform = entry.transform;

					// Update Shader
					if (lastShader != shader) {
						Shader.SetShader(shader);

						shader.SetupCommonUniforms();
						shader.SetupCameraUniforms(camera, cameraPos);

						// Update CullMode
						if (lastCullMode != shader.cullMode) {
							if (shader.cullMode == CullMode.Off) {
								GL.Disable(EnableCap.CullFace);
							} else {
								if (lastCullMode == CullMode.Off) {
									GL.Enable(EnableCap.CullFace);
								}

								GL.CullFace((CullFaceMode)shader.cullMode);
							}

							lastCullMode = shader.cullMode;
						}

						// Update PolygonMode
						if (lastPolygonMode != shader.polygonMode) {
							GL.PolygonMode(MaterialFace.FrontAndBack, lastPolygonMode = shader.polygonMode);
						}

						lastShader = shader;
					}

					// Update Material
					if (lastMaterial != material) {
						material.ApplyTextures(shader);
						material.ApplyUniforms(shader);

						lastMaterial = material;
					}

					// Render mesh

					// Mark matrices for recalculation
					for (int k = DefaultShaderUniforms.World; k <= DefaultShaderUniforms.ProjInverse; k++) {
						uniformComputed[k] = false;
					}

					worldMatrix = rendererTransform.WorldMatrix;

					shader.SetupMatrixUniforms(
						worldMatrix, ref inverseWorldMatrix,
						ref worldViewMatrix, ref inverseWorldViewMatrix,
						ref worldViewProjMatrix, ref inverseWorldViewProjMatrix,
						camera.ViewMatrix, camera.InverseViewMatrix,
						camera.ProjectionMatrix, camera.InverseProjectionMatrix
					);

					entry.mesh.Render();

					Rendering.DrawCallsCount++;
				}
			}

			GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
			GL.CullFace(CullFaceMode.Back);

			GL.Disable(EnableCap.CullFace);
			GL.Disable(EnableCap.DepthTest);
			GL.Disable(EnableCap.Blend);

			Framebuffer.Bind(null);
		}
	}
}
