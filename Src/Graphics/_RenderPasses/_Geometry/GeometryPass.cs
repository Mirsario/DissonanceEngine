using Silk.NET.OpenGL;
using static Dissonance.Engine.Graphics.OpenGLApi;

namespace Dissonance.Engine.Graphics
{
	public class GeometryPass : RenderPass
	{
		public LayerMask LayerMask { get; set; } = LayerMask.All;

		public override void Render()
		{
			Framebuffer.BindWithDrawBuffers(Framebuffer);

			OpenGL.Enable(EnableCap.CullFace);
			OpenGL.Enable(EnableCap.DepthTest);
			OpenGL.Enable(EnableCap.Blend);
			OpenGL.CullFace(CullFaceMode.Back);

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

				OpenGL.Viewport(viewport.X, viewport.Y, (uint)viewport.Width, (uint)viewport.Height);

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
								OpenGL.Disable(EnableCap.CullFace);
							} else {
								if (lastCullMode == CullMode.Off) {
									OpenGL.Enable(EnableCap.CullFace);
								}

								OpenGL.CullFace((CullFaceMode)shader.CullMode);
							}

							lastCullMode = shader.CullMode;
						}

						// Update PolygonMode
						if (lastPolygonMode != shader.PolygonMode) {
							OpenGL.PolygonMode(MaterialFace.FrontAndBack, shader.PolygonMode);

							lastPolygonMode = shader.PolygonMode;
						}

						// Update depth testing
						if (lastDepthTest != shader.DepthTest) {
							OpenGL.DepthFunc(shader.DepthTest);

							lastDepthTest = shader.DepthTest;
						}

						// Update depth writing
						if (lastDepthWrite != shader.DepthWrite) {
							OpenGL.DepthMask(shader.DepthWrite);

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

			OpenGL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
			OpenGL.CullFace(CullFaceMode.Back);

			OpenGL.Disable(EnableCap.CullFace);
			OpenGL.Disable(EnableCap.DepthTest);
			OpenGL.Disable(EnableCap.Blend);

			Framebuffer.Bind(null);
		}
	}
}
