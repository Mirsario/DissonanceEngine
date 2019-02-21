using System;
using OpenTK.Graphics.OpenGL;
using PrimitiveTypeGL = OpenTK.Graphics.OpenGL.PrimitiveType;

namespace GameEngine.Graphics
{
	[RenderPassInfo(acceptedShaderNames = new[] { "point","directional","spot" })]
	public class DeferredLightingPass : RenderPass
	{
		//public override string[] AcceptedShaderNames => Enum.GetNames(typeof(LightType)).Select(q => q.ToLower()).ToArray();

		//public Shader pointShader;
		//public Shader directionalShader;
		//public Shader spotShader;

		public DeferredLightingPass(string name) : base(name) {}

		/*public DeferredLightingPass(string name,Framebuffer framebuffer,RenderTexture[] renderTextures,Shader pointShader,Shader directionalShader,Shader spotShader) : base(name,framebuffer,renderTextures)
		{
			this.pointShader = pointShader;
			this.directionalShader = directionalShader;
			this.spotShader = spotShader;
		}*/

		public override void Render()
		{
			Framebuffer.BindWithDrawBuffers(framebuffer);

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

			for(int i=0;i<Rendering.cameraList.Count;i++) {
				var camera = Rendering.cameraList[i];
				var viewRect = camera.ViewPixel;
				GL.Viewport((int)viewRect.x,(int)viewRect.y,(int)viewRect.width,(int)viewRect.height);

				var cameraPos = camera.Transform.Position;

				for(int j=0;j<shaders.Length;j++) {
					var activeShader = shaders[j];
					if(activeShader==null) {
						continue;
					}
					Shader.SetShader(activeShader);

					var lightType = (LightType)j;
					
					for(int k=0;k<passedTextures.Length;k++) {
						GL.ActiveTexture((TextureUnit)((int)TextureUnit.Texture0+k));
						GL.BindTexture(TextureTarget.Texture2D,passedTextures[k].Id);
						if(activeShader!=null) {
							GL.Uniform1(GL.GetUniformLocation(activeShader.program,passedTextures[k].name),k);
						}
					}

					int uniformLightRange = GL.GetUniformLocation(activeShader.program,"lightRange");
					int uniformLightPosition = GL.GetUniformLocation(activeShader.program,"lightPosition");
					int uniformLightDirection = GL.GetUniformLocation(activeShader.program,"lightDirection");
					int uniformLightIntensity = GL.GetUniformLocation(activeShader.program,"lightIntensity");
					int uniformLightColor = GL.GetUniformLocation(activeShader.program,"lightColor");
				
					foreach(var light in Rendering.lightList) {
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
							GL.Uniform1(uniformLightRange,light.range);
							GL.Uniform3(uniformLightPosition,light.Transform.Position);
						}
						if(lightType!=LightType.Point) {
							GL.Uniform3(uniformLightDirection,light.Transform.Forward);
						}
						GL.Uniform1(uniformLightIntensity,light.intensity);
						GL.Uniform3(uniformLightColor,new Vector3(light.color.x,light.color.y,light.color.z));

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
								GL.Vertex2(	 1.0f,1.0f);
								GL.TexCoord2(1.0f,1.0f);
								GL.Vertex2(	 1.0f,-1.0f);
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