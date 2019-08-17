using OpenTK.Graphics.OpenGL;

namespace GameEngine.Graphics
{
	public class GeometryPass : RenderPass
	{
		public GeometryPass(string name) : base(name) {}

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

			var cullMode = CullMode.Front;
			var polygonMode = PolygonMode.Fill;

			#region CameraLoop
			for(int i=0;i<Rendering.cameraList.Count;i++) {
				var camera = Rendering.cameraList[i];
				var viewRect = camera.ViewPixel;
				GL.Viewport(viewRect.x,viewRect.y,viewRect.width,viewRect.height);
				camera.OnRenderStart?.Invoke(camera);
				var cameraPos = camera.Transform.Position;
				
				GL.EnableVertexAttribArray((int)AttributeId.Vertex);

				#region ShaderLoop
				int shaderCount = Shader.shaders.Count;
				for(int s=0;s<shaderCount;s++) {
					var shader = Shader.shaders[s];
					if(shader==null) {
						continue;
					}

					int materialCount = shader.materialAttachments.Count;
					if(materialCount==0) {
						continue;
					}

					if(cullMode!=shader.cullMode) {
						if(shader.cullMode==CullMode.Off) {
							GL.Disable(EnableCap.CullFace);
						}else{
							if(cullMode==CullMode.Off) {
								GL.Enable(EnableCap.CullFace);
							}
							GL.CullFace((CullFaceMode)shader.cullMode);
						}
						cullMode = shader.cullMode;
					}

					if(polygonMode!=shader.polygonMode) {
						GL.PolygonMode(MaterialFace.FrontAndBack,(OpenTK.Graphics.OpenGL.PolygonMode)(polygonMode = shader.polygonMode));
					}

					if(Shader.activeShader!=shader) {
						Shader.SetShader(shader);
					}
					
					#region MaterialLoop
					for(int m=0;m<materialCount;m++) {
						var material = shader.materialAttachments[m];
						if(material==null) {
							continue;
						}

						int rendererCount = material.rendererAttachments.Count;
						if(rendererCount==0) {
							continue;
						}

						material.ApplyTextures(shader);
						material.ApplyUniforms(shader);
						
						#region RendererLoop
						for(int r=0;r<rendererCount;r++) {
							var renderer = material.rendererAttachments[r];
							if(!renderer.Enabled) {
								continue;
							}

							var meshPos = renderer.Transform.Position;
							var mesh = renderer.GetRenderMeshInternal(meshPos,cameraPos);
							if(mesh==null || !mesh.IsReady) {
								continue;
							}

							bool cullResult = renderer.PreCullingModifyResult?.Invoke() ?? camera.orthographic || camera.BoxInFrustum(meshPos+mesh.boundsCenter,mesh.boundsExtent*renderer.Transform.LocalScale);
							var postCullResult = renderer.PostCullingModifyResult?.Invoke(cullResult);
							if(postCullResult!=null) {
								cullResult = postCullResult.Value;
							}

							if(!cullResult) {
								continue;
							}

							//Mark matrices for recalculation
							for(int k=DefaultShaderUniforms.World;k<=DefaultShaderUniforms.ProjInverse;k++) {
								uniformComputed[k] = false;
							}

							shader.SetupUniformsCached(ref camera,ref cameraPos,renderer.Transform,uniformComputed,
								ref world,				ref worldInverse,
								ref worldView,			ref worldViewInverse,
								ref worldViewProj,		ref worldViewProjInverse,
								ref camera.matrix_view,	ref camera.matrix_viewInverse,
								ref camera.matrix_proj,	ref camera.matrix_projInverse
							);
							renderer.ApplyUniforms(shader);

							mesh.DrawMesh();
							
							Rendering.drawCallsCount++;
						}
						#endregion
					}
					#endregion
				}
				#endregion

				camera.OnRenderEnd?.Invoke(camera);
			}
			#endregion

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