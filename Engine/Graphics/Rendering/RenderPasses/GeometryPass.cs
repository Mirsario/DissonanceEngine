using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using ImagingPixelFormat = System.Drawing.Imaging.PixelFormat;
using PrimitiveTypeGL = OpenTK.Graphics.OpenGL.PrimitiveType;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace GameEngine
{
	internal class GeometryPass : RenderPass
	{
		public override string Id => "geometry";
		public override bool RequiresShader => false;

		public override void Render()
		{
			Framebuffer.Bind(framebuffer);
			
			//This sets texture that we'll draw to
			if(framebuffer!=null) {
				GL.DrawBuffers(framebuffer.drawBuffers.Length,framebuffer.drawBuffers);
			}

			//if(drawOutlines) {
			//	GL.PolygonMode(MaterialFace.Front,PolygonMode.Line);
			//}else{
			//GL.PolygonMode(MaterialFace.Front,PolygonMode.Fill);
			//GL.ClearStencil(1);
			GL.Enable(EnableCap.CullFace);
			GL.Enable(EnableCap.DepthTest);
			GL.Enable(EnableCap.Blend);
			//}

			/*int passAmount = 1;
			for(int r=0;r<rendererList.Count;r++) {
				MeshRenderer renderer = rendererList[r];
				for(int m=0;m<renderer.materials.Length;m++) {
					Material mat = renderer.materials[m];
					if(mat==null) {
						continue;
					}
					if(mat.Shaders.Length>passAmount) {
						passAmount = mat.Shaders.Length;
					}
				}
			}*/
			//Debug.Log("camera amount: "+cameraList.Count);

			var uniformComputed = new bool[DefaultShaderUniforms.Count];
			Matrix4x4 world = default, worldInverse = default,
			worldView = default, worldViewInverse = default,
			worldViewProj = default, worldViewProjInverse = default;

			var cullMode = CullMode.Front;
			var polygonMode = PolygonMode.Fill;
			//for(int extraPass=0;extraPass<passAmount;extraPass++) {
			/*Debug.Log(mat.stencilWrite);//optimize this vvv
			int val = mat.stencilWrite>=1 ? 0x01 : 0x00;
			GL.StencilMask(val);
			GL.StencilFunc(StencilFunction.Always,val,val);	//Not testing,but writing
			GL.StencilOp(StencilOp.Keep,StencilOp.Keep,StencilOp.Replace);//Write options*/

			#region CameraLoop
			for(int i=0;i<Graphics.cameraList.Count;i++) {
				var camera = Graphics.cameraList[i];
				var viewRect = camera.ViewPixel;
				GL.Viewport(viewRect.X,viewRect.Y,viewRect.Width,viewRect.Height);
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
					int materialCount = shader.materialAttachments.Count;
					for(int m=0;m<materialCount;m++) {
						var material = shader.materialAttachments[m];
						if(material==null) {
							continue;
						}
						material.ApplyTextures(shader);
						material.ApplyUniforms(shader);
						
						#region RendererLoop
						int rendererCount = material.rendererAttachments.Count;
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
							
							#region Draw
							GL.BindBuffer(BufferTarget.ArrayBuffer,mesh.vertexBufferId);
							int offset = 0;
							GL.VertexAttribPointer((int)AttributeId.Vertex,3,VertexAttribPointerType.Float,false,mesh.vertexSize,(IntPtr)offset);
							offset += sizeof(float)*3;
							#region Normals
							if(mesh.normals!=null) {
								GL.EnableVertexAttribArray((int)AttributeId.Normal);
								GL.VertexAttribPointer((int)AttributeId.Normal,3,VertexAttribPointerType.Float,true,mesh.vertexSize,(IntPtr)offset);
								offset += sizeof(float)*3;
							}else{
								GL.DisableVertexAttribArray((int)AttributeId.Normal);
								GL.VertexAttrib3((int)AttributeId.Normal,Vector3.zero);
							}
							#endregion
							#region Tangents
							if(mesh.tangents!=null) {
								GL.EnableVertexAttribArray((int)AttributeId.Tangent);
								GL.VertexAttribPointer((int)AttributeId.Tangent,4,VertexAttribPointerType.Float,false,mesh.vertexSize,(IntPtr)offset);
								offset += sizeof(float)*4;
							}else{
								GL.DisableVertexAttribArray((int)AttributeId.Tangent);
								GL.VertexAttrib4((int)AttributeId.Tangent,Vector4.zero);
							}
							#endregion
							#region Colors
							if(mesh.colors!=null) {
								GL.EnableVertexAttribArray((int)AttributeId.Color);
								GL.VertexAttribPointer((int)AttributeId.Color,4,VertexAttribPointerType.Float,false,mesh.vertexSize,(IntPtr)offset);
								offset += sizeof(float)*4;
							}else{
								GL.DisableVertexAttribArray((int)AttributeId.Color);
								GL.VertexAttrib4((int)AttributeId.Color,Vector4.one);
							}
							#endregion
							#region BoneWeights
							if(mesh.boneWeights!=null) {
								GL.EnableVertexAttribArray((int)AttributeId.BoneIndices);
								GL.EnableVertexAttribArray((int)AttributeId.BoneWeights);
								GL.VertexAttribPointer((int)AttributeId.BoneIndices,4,VertexAttribPointerType.Float,false,mesh.vertexSize,(IntPtr)offset);
								offset += sizeof(float)*4;
								GL.VertexAttribPointer((int)AttributeId.BoneWeights,4,VertexAttribPointerType.Float,false,mesh.vertexSize,(IntPtr)offset);
								offset += sizeof(float)*4;
							}else{
								GL.DisableVertexAttribArray((int)AttributeId.BoneIndices);
								GL.DisableVertexAttribArray((int)AttributeId.BoneWeights);
								GL.VertexAttrib4((int)AttributeId.BoneIndices,Vector4.zero);
								GL.VertexAttrib4((int)AttributeId.BoneWeights,Vector4.zero);
							}
							#endregion
							#region UV
							if(mesh.uv!=null) {
								GL.EnableVertexAttribArray((int)AttributeId.Uv0);
								GL.VertexAttribPointer((int)AttributeId.Uv0,2,VertexAttribPointerType.Float,false,mesh.vertexSize,(IntPtr)offset);
								//offset += sizeof(float)*2;
							}else{
								GL.DisableVertexAttribArray((int)AttributeId.Uv0);
								GL.VertexAttrib2((int)AttributeId.Uv0,Vector2.zero);
							}
							#endregion
							GL.BindBuffer(BufferTarget.ElementArrayBuffer,mesh.indexBufferId);

							//GL.PolygonMode(MaterialFace.Front,PolygonMode.Line);
							
							GL.DrawElements(PrimitiveTypeGL.Triangles,mesh.indexLength,DrawElementsType.UnsignedInt,0);
							#endregion
						}
						#endregion
					}
					#endregion
				}
				#endregion

				camera.OnRenderEnd?.Invoke(camera);
			}
			#endregion
			//}

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