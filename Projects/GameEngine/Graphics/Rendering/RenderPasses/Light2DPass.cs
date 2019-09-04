using System;
using OpenTK.Graphics.OpenGL;
using GLPrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType;

namespace GameEngine.Graphics
{
	public class Light2DPass : RenderPass
	{
		public override void Render()
		{
			Framebuffer.BindWithDrawBuffers(framebuffer);

			GL.Enable(EnableCap.Blend);
			GL.Enable(EnableCap.CullFace);
			GL.DepthMask(false);

			Matrix4x4 worldInverse = default,
			worldView = default, worldViewInverse = default,
			worldViewProj = default, worldViewProjInverse = default;

			for(int i=0;i<Rendering.cameraList.Count;i++) {
				var camera = Rendering.cameraList[i];
				var viewport = GetViewport(camera);
				GL.Viewport(viewport.x,viewport.y,viewport.width,viewport.height);

				var cameraPos = camera.Transform.Position;

				Shader.SetShader(passShader);

				if(passedTextures!=null) {
					for(int j=0;j<passedTextures.Length;j++) {
						GL.ActiveTexture((TextureUnit)((int)TextureUnit.Texture0+j));
						GL.BindTexture(TextureTarget.Texture2D,passedTextures[j].Id);

						GL.Uniform1(GL.GetUniformLocation(passShader.program,passedTextures[j].name),j);
					}
				}

				int uniformLightRange = GL.GetUniformLocation(passShader.program,"lightRange");
				int uniformLightPosition = GL.GetUniformLocation(passShader.program,"lightPosition");
				int uniformLightIntensity = GL.GetUniformLocation(passShader.program,"lightIntensity");
				int uniformLightColor = GL.GetUniformLocation(passShader.program,"lightColor");
				
				foreach(var light in Rendering.light2DList) {
					var world = Matrix4x4.CreateScale(light.range+1f)*Matrix4x4.CreateTranslation(light.Transform.Position);

					passShader.SetupUniforms(
						ref camera,ref cameraPos,light.Transform,
						ref world,ref worldInverse,
						ref worldView,ref worldViewInverse,
						ref worldViewProj,ref worldViewProjInverse,
						ref camera.matrix_view,ref camera.matrix_viewInverse,
						ref camera.matrix_proj,ref camera.matrix_projInverse,
						true
					);
					
					GL.Uniform1(uniformLightRange,light.range);
					GL.Uniform3(uniformLightPosition,light.Transform.Position);
					GL.Uniform1(uniformLightIntensity,light.intensity);
					GL.Uniform3(uniformLightColor,new Vector3(light.color.x,light.color.y,light.color.z));
					
					GL.BindBuffer(BufferTarget.ArrayBuffer,PrimitiveMeshes.quad.vertexBufferId);
					GL.VertexAttribPointer((int)AttributeId.Vertex,3,VertexAttribPointerType.Float,false,PrimitiveMeshes.quad.vertexSize,(IntPtr)0);
					GL.BindBuffer(BufferTarget.ElementArrayBuffer,PrimitiveMeshes.quad.indexBufferId);
					GL.DrawElements(GLPrimitiveType.Triangles,PrimitiveMeshes.quad.indexLength,DrawElementsType.UnsignedInt,0);
				}
			}

			GL.DepthMask(true);
			GL.Disable(EnableCap.Blend);
			GL.Disable(EnableCap.CullFace);
			Shader.SetShader(null);
			GL.BindTexture(TextureTarget.Texture2D,0);
		}
	}
}