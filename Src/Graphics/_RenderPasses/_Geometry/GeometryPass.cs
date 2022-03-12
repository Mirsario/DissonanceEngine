﻿using Dissonance.Framework.Graphics;

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

			bool[] uniformComputed = new bool[DefaultShaderUniforms.Count];
			Matrix4x4 worldMatrix = default, inverseWorldMatrix = default,
			worldViewMatrix = default, inverseWorldViewMatrix = default,
			worldViewProjMatrix = default, inverseWorldViewProjMatrix = default;

			// Render cache
			Shader lastShader = null;
			Material lastMaterial = null;
			CullMode? lastCullMode = null;
			PolygonMode? lastPolygonMode = null;
			bool? lastDepthWrite = null;
			DepthFunction? lastDepthTest = null;

			var renderViewData = GlobalGet<RenderViewData>();
			var geometryPassData = GlobalGet<GeometryPassData>();

			// CameraLoop
			foreach (var renderView in renderViewData.RenderViews) {
				var viewport = renderView.Viewport;

				GL.Viewport(viewport.X, viewport.Y, viewport.Width, viewport.Height);

				var cameraTransform = renderView.Transform;
				var cameraPos = cameraTransform.Position;

				var usedLayerMask = LayerMask & renderView.LayerMask;
				bool checkLayerMask = usedLayerMask != LayerMask.All;

				// Render
				for (int i = 0; i < geometryPassData.RenderEntries.Count; i++) {
					var entry = geometryPassData.RenderEntries[i];

					if (checkLayerMask && (entry.LayerMask & usedLayerMask) == LayerMask.None) {
						continue;
					}

					var material = entry.Material;

					if (!material.Shader.TryGetOrRequestValue(out var shader)) {
						continue;
					}

					var rendererTransform = entry.Transform;

					// Update Shader
					if (lastShader != shader) {
						Shader.SetShader(shader);

						shader.SetupCommonUniforms();
						shader.SetupCameraUniforms(renderView.NearClip, renderView.FarClip, cameraPos);

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
							GL.PolygonMode(MaterialFace.FrontAndBack, shader.PolygonMode);

							lastPolygonMode = shader.PolygonMode;
						}

						// Update depth testing
						if (lastDepthTest != shader.DepthTest) {
							GL.DepthFunc(shader.DepthTest);

							lastDepthTest = shader.DepthTest;
						}

						// Update depth writing
						if (lastDepthWrite != shader.DepthWrite) {
							GL.DepthMask(shader.DepthWrite);

							lastDepthWrite = shader.DepthWrite;
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
						renderView.ViewMatrix, renderView.InverseViewMatrix,
						renderView.ProjectionMatrix, renderView.InverseProjectionMatrix
					);

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