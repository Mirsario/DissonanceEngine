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
	internal class DeferredLightingPass : RenderPass
	{
		public override string Id => "lighting";
		public override string[] AcceptedShaderNames => Enum.GetNames(typeof(LightType)).Select(q => q.ToLower()).ToArray();

		public override void Render()
		{
			Framebuffer.Bind(framebuffer);
			//TestStencils2();
			//return;
			if(framebuffer!=null) {
				GL.DrawBuffers(framebuffer.drawBuffers.Length,framebuffer.drawBuffers);
			}else{
				//GL.DrawBuffers(1,Graphics.nullDrawBuffers);
			}

			GL.Enable(EnableCap.Blend);
			GL.Enable(EnableCap.CullFace);
			GL.BlendFunc(BlendingFactor.SrcAlpha,BlendingFactor.One);
			GL.DepthMask(false);
			//test if it equals 1
			//GL.StencilFunc(StencilFunction.Notequal,0x01,0x01);
			//GL.StencilMask(0);

			Matrix4x4 worldInverse = default,
			worldView = default, worldViewInverse = default,
			worldViewProj = default, worldViewProjInverse = default;

			for(int i=0;i<Graphics.cameraList.Count;i++) {
				var camera = Graphics.cameraList[i];
				var viewRect = camera.ViewPixel;
				GL.Viewport(viewRect.X,viewRect.Y,viewRect.Width,viewRect.Height);

				var cameraPos = camera.Transform.Position;

				for(int j=0;j<shaders.Length;j++) {
					var activeShader = shaders[j];
					if(activeShader==null) {
						continue;
					}
					Shader.SetShader(activeShader);

					var lightType = (LightType)j;
					
					for(int k=0;k<textures.Length;k++) {
						GL.ActiveTexture((TextureUnit)((int)TextureUnit.Texture0+k));
						GL.BindTexture(TextureTarget.Texture2D,textures[k].Id);
						if(activeShader!=null) {
							GL.Uniform1(GL.GetUniformLocation(activeShader.program,textures[k].name),k);
						}
					}
				
					foreach(var light in Graphics.lightList) {
						if(light.type!=lightType) {
							continue;
						}

						var world = Matrix4x4.CreateScale(light.range)*Matrix4x4.CreateTranslation(light.Transform.Position);

						activeShader.SetupUniforms(
							ref camera,ref cameraPos,light.Transform,
							ref world,ref worldInverse,
							ref worldView,ref worldViewInverse,
							ref worldViewProj,ref worldViewProjInverse,
							ref camera.matrix_view,ref camera.matrix_viewInverse,
							ref camera.matrix_proj,ref camera.matrix_projInverse,
							true
						);

						//TODO: Get rid of GetUniformLocation somehow?
						if(lightType!=LightType.Directional) {
							GL.Uniform1(GL.GetUniformLocation(activeShader.program,"lightRange"),light.range);
							GL.Uniform3(GL.GetUniformLocation(activeShader.program,"lightPosition"),light.Transform.Position);
						}
						if(lightType!=LightType.Point) {
							GL.Uniform3(GL.GetUniformLocation(activeShader.program,"lightDirection"),light.Transform.Forward);
						}
						GL.Uniform1(GL.GetUniformLocation(activeShader.program,"lightIntensity"),light.intensity);
						GL.Uniform3(GL.GetUniformLocation(activeShader.program,"lightColor"),new Vector3(light.color.x,light.color.y,light.color.z));

						switch(lightType) {
							case LightType.Point:
								GL.BindBuffer(BufferTarget.ArrayBuffer,PrimitiveMeshes.icoSphere.vertexBufferId);
								GL.VertexAttribPointer((int)AttributeId.Vertex,3,VertexAttribPointerType.Float,false,PrimitiveMeshes.icoSphere.vertexSize,(IntPtr)0);
								GL.BindBuffer(BufferTarget.ElementArrayBuffer,PrimitiveMeshes.icoSphere.indexBufferId);
								GL.DrawElements(PrimitiveTypeGL.Triangles,PrimitiveMeshes.icoSphere.indexLength,DrawElementsType.UnsignedInt,0);
								break;
							case LightType.Directional:
								//TODO: Draw like this should be made into a function
								GL.Begin(PrimitiveTypeGL.Quads);
								GL.Vertex2(	-1.0f,-1.0f);
								GL.TexCoord2(0.0f,0.0f);
								GL.Vertex2(	-1.0f,1.0f);
								GL.TexCoord2(0.0f,1.0f);
								GL.Vertex2(	1.0f,1.0f);
								GL.TexCoord2(1.0f,1.0f);
								GL.Vertex2(	1.0f,-1.0f);
								GL.TexCoord2(1.0f,0.0f);
								GL.End();
								break;
						}
					}
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