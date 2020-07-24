using Dissonance.Engine.Physics;
using Dissonance.Framework.Graphics;
using Dissonance.Engine.Structures;
using Dissonance.Engine.Graphics.Renderers;
using Dissonance.Engine.Graphics.Enums;
using Dissonance.Engine.Graphics.Shaders;
using Dissonance.Engine.Core.Components;
using Dissonance.Engine.Graphics.Components;

namespace Dissonance.Engine.Graphics.RenderPasses.Default
{
	public class GeometryPass : RenderPass
	{
		protected struct RenderQueueEntry
		{
			public Shader shader;
			public Material material;
			public Renderer renderer;
			public object renderObject;
		}

		public ulong? layerMask;

		public override void Render()
		{
			Framebuffer.BindWithDrawBuffers(framebuffer);

			GL.Enable(EnableCap.CullFace);
			GL.Enable(EnableCap.DepthTest);
			GL.Enable(EnableCap.Blend);

			var uniformComputed = new bool[DefaultShaderUniforms.Count];
			Matrix4x4 world = default, worldInverse = default,
			worldView = default, worldViewInverse = default,
			worldViewProj = default, worldViewProjInverse = default;

			bool hasLayerMask = layerMask.HasValue;
			ulong layerMaskValue = layerMask ?? 0;

			//Render cache
			Shader lastShader = null;
			Material lastMaterial = null;
			var lastCullMode = CullMode.Front;
			var lastPolygonMode = PolygonMode.Fill;

			int rendererCount = ComponentManager.CountComponents<Renderer>();
			var renderQueue = new RenderQueueEntry[rendererCount];

			//CameraLoop
			foreach(var camera in ComponentManager.EnumerateComponents<Camera>()) {
				var viewport = GetViewport(camera);
				GL.Viewport(viewport.x,viewport.y,viewport.width,viewport.height);

				camera.OnRenderStart?.Invoke(camera);

				var cameraPos = camera.Transform.Position;

				//RendererLoop
				if(rendererCount==0) {
					continue;
				}

				int numToRenderer = 0;

				foreach(var renderer in ComponentManager.EnumerateComponents<Renderer>()) {
					if(!renderer.Enabled) {
						continue;
					}

					//TODO: To be optimized
					if(hasLayerMask && (Layers.GetLayerMask(renderer.gameObject.layer) & layerMaskValue)==0) {
						continue;
					}

					var meshPos = renderer.Transform.Position;

					if(!renderer.GetRenderData(meshPos,cameraPos,out var material,out var bounds,out var renderObject)) {
						continue;
					}

					var shader = material.Shader;
					if(shader==null) {
						continue;
					}

					bool cullResult = renderer.PreCullingModifyResult?.Invoke() ?? camera.orthographic || camera.BoxInFrustum(meshPos+bounds.center,bounds.extents*renderer.Transform.LocalScale);
					var postCullResult = renderer.PostCullingModifyResult?.Invoke(cullResult);
					if(postCullResult!=null) {
						cullResult = postCullResult.Value;
					}

					if(cullResult) {
						ref var entry = ref renderQueue[numToRenderer++];
						entry.shader = shader;
						entry.material = material;
						entry.renderer = renderer;
						entry.renderObject = renderObject;
					}
				}

				//Sort the render queue
				for(int i = 0;i<numToRenderer-1;i++) {
					for(int j = i+1;j<numToRenderer;j++) {
						ref var iTuple = ref renderQueue[i];
						ref var jTuple = ref renderQueue[j];

						if(iTuple.shader.queue>jTuple.shader.queue || iTuple.shader.Id>jTuple.shader.Id || iTuple.material.Id>jTuple.material.Id) {
							var temp = iTuple;
							iTuple = jTuple;
							jTuple = temp;
						}
					}
				}

				//Render
				for(int i = 0;i<numToRenderer;i++) {
					var entry = renderQueue[i];
					var shader = entry.shader;
					var material = entry.material;
					var renderer = entry.renderer;
					var renderObject = entry.renderObject;

					//Update Shader
					if(lastShader!=shader) {
						Shader.SetShader(shader);

						shader.SetupCommonUniforms();
						shader.SetupCameraUniforms(camera,cameraPos);

						//Update CullMode
						if(lastCullMode!=shader.cullMode) {
							if(shader.cullMode==CullMode.Off) {
								GL.Disable(EnableCap.CullFace);
							} else {
								if(lastCullMode==CullMode.Off) {
									GL.Enable(EnableCap.CullFace);
								}

								GL.CullFace((CullFaceMode)shader.cullMode);
							}

							lastCullMode = shader.cullMode;
						}

						//Update PolygonMode
						if(lastPolygonMode!=shader.polygonMode) {
							GL.PolygonMode(MaterialFace.FrontAndBack,lastPolygonMode = shader.polygonMode);
						}

						lastShader = shader;
					}

					//Update Material
					if(lastMaterial!=material) {
						material.ApplyTextures(shader);
						material.ApplyUniforms(shader);

						lastMaterial = material;
					}

					//Render mesh

					//Mark matrices for recalculation
					for(int k = DefaultShaderUniforms.World;k<=DefaultShaderUniforms.ProjInverse;k++) {
						uniformComputed[k] = false;
					}

					shader.SetupMatrixUniformsCached(
						renderer.Transform,uniformComputed,
						ref world,ref worldInverse,
						ref worldView,ref worldViewInverse,
						ref worldViewProj,ref worldViewProjInverse,
						ref camera.matrix_view,ref camera.matrix_viewInverse,
						ref camera.matrix_proj,ref camera.matrix_projInverse
					);

					renderer.ApplyUniforms(shader);
					renderer.Render(renderObject);

					Rendering.drawCallsCount++;
				}

				camera.OnRenderEnd?.Invoke(camera);
			}

			GL.PolygonMode(MaterialFace.FrontAndBack,PolygonMode.Fill);
			GL.CullFace(CullFaceMode.Front);

			GL.Disable(EnableCap.CullFace);
			GL.Disable(EnableCap.DepthTest);
			GL.Disable(EnableCap.Blend);
			//GL.Disable(EnableCap.AlphaTest);

			Framebuffer.Bind(null);
		}
	}
}