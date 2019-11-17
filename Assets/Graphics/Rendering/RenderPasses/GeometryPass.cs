using GameEngine.Physics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Diagnostics;

namespace GameEngine.Graphics
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

		private Stopwatch sw;

		public override void OnInit()
		{
			sw = new Stopwatch();
		}
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

			int rendererCount = Rendering.rendererList.Count;
			var renderQueue = new RenderQueueEntry[rendererCount];

			sw.Restart();

			void UseStopwatch(ref long addTo,Action action)
			{
				long tempMs = sw.ElapsedTicks;

				action();

				addTo += sw.ElapsedTicks-tempMs;
			}

			long rendererLoopMs = 0;
			long sortingMs = 0;
			long renderMs = 0;
			long totalMs = 0;

			bool doSort = !Input.GetKey(Keys.U);

			//CameraLoop
			UseStopwatch(ref totalMs,() => {
				for(int c = 0;c<Rendering.cameraList.Count;c++) {
					var camera = Rendering.cameraList[c];

					var viewport = GetViewport(camera);
					GL.Viewport(viewport.x,viewport.y,viewport.width,viewport.height);

					camera.OnRenderStart?.Invoke(camera);

					var cameraPos = camera.Transform.Position;

					GL.EnableVertexAttribArray((int)AttributeId.Vertex);

					//RendererLoop
					if(rendererCount==0) {
						continue;
					}

					int numToRenderer = 0;

					UseStopwatch(ref rendererLoopMs,() => {
						for(int r = 0;r<rendererCount;r++) {
							var renderer = Rendering.rendererList[r];

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
					});
					 
					//Sort the render queue
					UseStopwatch(ref sortingMs,() => {
						if(doSort) {
							for(int i = 0;i<numToRenderer-1;i++) {
								for(int j = i+1;j<numToRenderer;j++) {
									ref var iTuple = ref renderQueue[i];
									ref var jTuple = ref renderQueue[j];

									if(iTuple.shader.Id<jTuple.shader.Id || iTuple.material.Id<jTuple.material.Id) {
										var temp = iTuple;
										iTuple = jTuple;
										jTuple = temp;
									}
								}
							}
						}
					});

					//Render
					UseStopwatch(ref renderMs,() => {
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
									GL.PolygonMode(MaterialFace.FrontAndBack,(OpenTK.Graphics.OpenGL.PolygonMode)(lastPolygonMode = shader.polygonMode));
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

							//mesh.DrawMesh();
							renderer.Render(renderObject);

							Rendering.drawCallsCount++;
						}
					});

					camera.OnRenderEnd?.Invoke(camera);
				}
			});

			if(Input.GetKeyDown(Keys.Y)) {
				void LogStat(string name,long ticks,int tabs = 2) => Debug.Log($"{name}: {new string('\t',tabs)}{ticks} ticks \t({(ticks/(float)totalMs)*100f:0.00}%)");

				Debug.Log($"RENDER STATS:");
				LogStat(nameof(totalMs),totalMs);
				LogStat(nameof(rendererLoopMs),rendererLoopMs,1);
				LogStat(nameof(sortingMs),sortingMs);
				LogStat(nameof(renderMs),renderMs);
				Debug.Log($"");
			}

			GL.PolygonMode(MaterialFace.FrontAndBack,OpenTK.Graphics.OpenGL.PolygonMode.Fill);
			GL.CullFace(CullFaceMode.Front);

			GL.Disable(EnableCap.CullFace);
			GL.Disable(EnableCap.DepthTest);
			GL.Disable(EnableCap.Blend);
			GL.Disable(EnableCap.AlphaTest);
			Framebuffer.Bind(null);
			GLDraw.Draw();
		}
	}
}