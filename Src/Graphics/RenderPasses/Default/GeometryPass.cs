using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics
{
	public class GeometryPass : RenderPass
	{
		public LayerMask LayerMask { get; set; } = LayerMask.All;

		public override void Render()
		{
			Framebuffer.BindWithDrawBuffers(Framebuffer);

			GL.Enable(EnableCap.CullFace);
			GL.Enable(EnableCap.DepthTest);
			GL.Enable(EnableCap.Blend);
			GL.CullFace(CullFaceMode.Back);

			// Render cache
			Shader lastShader = null;
			Material lastMaterial = null;
			var lastCullMode = CullMode.Back;
			var lastPolygonMode = PolygonMode.Fill;

			var renderViewData = GlobalGet<RenderViewData>();
			var geometryPassData = GlobalGet<GeometryPassData>();

			bool checkLayerMask = LayerMask != LayerMask.All;

			// CameraLoop
			foreach (var renderView in renderViewData.RenderViews) {
				var viewport = renderView.Viewport;

				GL.Viewport(viewport.x, viewport.y, viewport.width, viewport.height);

				var cameraTransform = renderView.Transform;
				var cameraPos = cameraTransform.Position;

				// Render
				for (int i = 0; i < geometryPassData.RenderEntries.Count; i++) {
					var entry = geometryPassData.RenderEntries[i];

					if (checkLayerMask && (entry.LayerMask & LayerMask) == LayerMask.None) {
						continue;
					}

					var material = entry.Material;
					var shader = material.Shader;
					var rendererTransform = entry.Transform;

					// Update Shader
					if (lastShader != shader) {
						Shader.SetShader(shader);

						shader.SetupDefaultUniforms(in rendererTransform, in renderView);
						//shader.SetupDefaultUniforms();
						//shader.SetupCameraUniforms(renderView.NearClip, renderView.FarClip, cameraPos);

						// Update CullMode
						if (lastCullMode != shader.CullMode) {
							if (shader.CullMode == CullMode.Off) {
								GL.Disable(EnableCap.CullFace);
							} else {
								if (lastCullMode == CullMode.Off) {
									GL.Enable(EnableCap.CullFace);
								}

								GL.CullFace((CullFaceMode)shader.CullMode);
							}

							lastCullMode = shader.CullMode;
						}

						// Update PolygonMode
						if (lastPolygonMode != shader.PolygonMode) {
							GL.PolygonMode(MaterialFace.FrontAndBack, lastPolygonMode = shader.PolygonMode);
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

					var worldMatrix = rendererTransform.WorldMatrix;

					/*shader.SetupMatrixUniforms(
						worldMatrix, ref inverseWorldMatrix,
						ref worldViewMatrix, ref inverseWorldViewMatrix,
						ref worldViewProjMatrix, ref inverseWorldViewProjMatrix,
						renderView.ViewMatrix, renderView.InverseViewMatrix,
						renderView.ProjectionMatrix, renderView.InverseProjectionMatrix
					);*/

					entry.Mesh.Render();

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
