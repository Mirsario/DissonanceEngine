using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics
{
	public class GeometryPass : RenderPass
	{
		protected struct RenderQueueEntry
		{
			public Shader shader;
			public Material material;
			public IRenderer renderer;
			public Transform transform;
			public object renderObject;
		}

		public ulong? layerMask;

		public override void Render(World world)
		{
			Framebuffer.BindWithDrawBuffers(framebuffer);

			GL.Enable(EnableCap.CullFace);
			GL.Enable(EnableCap.DepthTest);
			GL.Enable(EnableCap.Blend);

			var uniformComputed = new bool[DefaultShaderUniforms.Count];
			Matrix4x4 worldMatrix = default, inverseWorldMatrix = default,
			worldViewMatrix = default, inverseWorldViewMatrix = default,
			worldViewProjMatrix = default, inverseWorldViewProjMatrix = default;

			bool hasLayerMask = layerMask.HasValue;
			ulong layerMaskValue = layerMask ?? 0;

			//Render cache
			Shader lastShader = null;
			Material lastMaterial = null;
			var lastCullMode = CullMode.Front;
			var lastPolygonMode = PolygonMode.Fill;

			int rendererCount = 0;

			//TODO: This is temporary.
			foreach(var entity in world.ReadEntities()) {
				if(entity.HasComponent<MeshRenderer>()) {
					rendererCount++;
				}
			}

			var renderQueue = new RenderQueueEntry[rendererCount];

			//CameraLoop
			foreach(var cameraEntity in world.ReadEntities()) {
				if(!cameraEntity.HasComponent<Camera>() || !cameraEntity.HasComponent<Transform>()) {
					continue;
				}

				ref var camera = ref cameraEntity.GetComponent<Camera>();
				var cameraTransform = cameraEntity.GetComponent<Transform>();

				var viewport = GetViewport(camera);

				GL.Viewport(viewport.x, viewport.y, viewport.width, viewport.height);

				//RendererLoop
				if(rendererCount == 0) {
					continue;
				}

				var cameraPos = cameraTransform.Position;
				int numToRender = 0;

				foreach(var rendererEntity in world.ReadEntities()) {
					if(!rendererEntity.HasComponent<MeshRenderer>() || !rendererEntity.HasComponent<Transform>()) {
						continue;
					}

					ref var renderer = ref rendererEntity.GetComponent<MeshRenderer>();
					var rendererTransform = rendererEntity.GetComponent<Transform>();

					/*if(!renderer.Enabled) {
						continue;
					}*/

					//TODO: To be optimized
					/*if(hasLayerMask && (Layers.GetLayerMask(renderer.gameObject.Layer) & layerMaskValue) == 0) {
						continue;
					}*/

					if(!renderer.GetRenderData(rendererTransform.Position, cameraPos, out var material, out object renderObject)) {
						continue;
					}

					var shader = material.Shader;

					if(shader == null) {
						continue;
					}

					bool cullResult = camera.Orthographic || camera.BoxInFrustum(renderer.GetAabb(rendererTransform));

					if(cullResult) {
						ref var entry = ref renderQueue[numToRender++];

						entry.shader = shader;
						entry.material = material;
						entry.renderer = renderer;
						entry.renderObject = renderObject;
						entry.transform = rendererTransform;
					}
				}

				//Sort the render queue
				for(int i = 0; i < numToRender - 1; i++) {
					for(int j = i + 1; j < numToRender; j++) {
						ref var iTuple = ref renderQueue[i];
						ref var jTuple = ref renderQueue[j];

						if(iTuple.shader.queue > jTuple.shader.queue || iTuple.shader.Id > jTuple.shader.Id || iTuple.material.Id > jTuple.material.Id) {
							var temp = iTuple;
							iTuple = jTuple;
							jTuple = temp;
						}
					}
				}

				//Temporary bullshit
				var viewMatrix = camera.ViewMatrix;
				var projectionMatrix = camera.ProjectionMatrix;
				var inverseViewMatrix = camera.InverseViewMatrix;
				var inverseProjectionMatrix = camera.InverseProjectionMatrix;

				//Render
				for(int i = 0; i < numToRender; i++) {
					var entry = renderQueue[i];
					var shader = entry.shader;
					var material = entry.material;
					var renderer = entry.renderer;
					var rendererTransform = entry.transform;
					object renderObject = entry.renderObject;

					//Update Shader
					if(lastShader != shader) {
						Shader.SetShader(shader);

						shader.SetupCommonUniforms();
						shader.SetupCameraUniforms(camera, cameraPos);

						//Update CullMode
						if(lastCullMode != shader.cullMode) {
							if(shader.cullMode == CullMode.Off) {
								GL.Disable(EnableCap.CullFace);
							} else {
								if(lastCullMode == CullMode.Off) {
									GL.Enable(EnableCap.CullFace);
								}

								GL.CullFace((CullFaceMode)shader.cullMode);
							}

							lastCullMode = shader.cullMode;
						}

						//Update PolygonMode
						if(lastPolygonMode != shader.polygonMode) {
							GL.PolygonMode(MaterialFace.FrontAndBack, lastPolygonMode = shader.polygonMode);
						}

						lastShader = shader;
					}

					//Update Material
					if(lastMaterial != material) {
						material.ApplyTextures(shader);
						material.ApplyUniforms(shader);

						lastMaterial = material;
					}

					//Render mesh

					//Mark matrices for recalculation
					for(int k = DefaultShaderUniforms.World; k <= DefaultShaderUniforms.ProjInverse; k++) {
						uniformComputed[k] = false;
					}

					shader.SetupMatrixUniformsCached(
						rendererTransform, uniformComputed,
						ref worldMatrix, ref inverseWorldMatrix,
						ref worldViewMatrix, ref inverseWorldViewMatrix,
						ref worldViewProjMatrix, ref inverseWorldViewProjMatrix,
						ref viewMatrix, ref inverseViewMatrix,
						ref projectionMatrix, ref inverseProjectionMatrix
					);

					renderer.ApplyUniforms(shader);
					renderer.Render(renderObject);

					Rendering.DrawCallsCount++;
				}

				//Temporary bullshit
				camera.ViewMatrix = viewMatrix;
				camera.ProjectionMatrix = projectionMatrix;
				camera.InverseViewMatrix = inverseViewMatrix;
				camera.InverseProjectionMatrix = inverseProjectionMatrix;
			}

			GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
			GL.CullFace(CullFaceMode.Front);

			GL.Disable(EnableCap.CullFace);
			GL.Disable(EnableCap.DepthTest);
			GL.Disable(EnableCap.Blend);
			//GL.Disable(EnableCap.AlphaTest);

			Framebuffer.Bind(null);
		}
	}
}
