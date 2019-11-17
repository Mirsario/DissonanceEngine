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

			Shader.SetShader(passShader);

			passShader.SetupCommonUniforms();

			for(int i = 0;i<Rendering.cameraList.Count;i++) {
				var camera = Rendering.cameraList[i];

				var viewport = GetViewport(camera);
				GL.Viewport(viewport.x,viewport.y,viewport.width,viewport.height);

				var cameraPos = camera.Transform.Position;

				passShader.SetupCameraUniforms(camera,cameraPos);

				if(passedTextures!=null) {
					for(int j = 0;j<passedTextures.Length;j++) {
						GL.ActiveTexture((TextureUnit)((int)TextureUnit.Texture0+j));
						GL.BindTexture(TextureTarget.Texture2D,passedTextures[j].Id);

						GL.Uniform1(GL.GetUniformLocation(passShader.Id,passedTextures[j].name),j);
					}
				}

				int uniformLightRange = GL.GetUniformLocation(passShader.Id,"lightRange");
				int uniformLightPosition = GL.GetUniformLocation(passShader.Id,"lightPosition");
				int uniformLightIntensity = GL.GetUniformLocation(passShader.Id,"lightIntensity");
				int uniformLightColor = GL.GetUniformLocation(passShader.Id,"lightColor");
				
				foreach(var light in Rendering.light2DList) {
					var world = Matrix4x4.CreateScale(light.range+1f)*Matrix4x4.CreateTranslation(light.Transform.Position);

					passShader.SetupMatrixUniforms(
						light.Transform,
						ref world,				ref worldInverse,
						ref worldView,			ref worldViewInverse,
						ref worldViewProj,		ref worldViewProjInverse,
						ref camera.matrix_view,	ref camera.matrix_viewInverse,
						ref camera.matrix_proj,	ref camera.matrix_projInverse,
						true
					);
					
					GL.Uniform1(uniformLightRange,light.range);
					GL.Uniform3(uniformLightPosition,light.Transform.Position);
					GL.Uniform1(uniformLightIntensity,light.intensity);
					GL.Uniform3(uniformLightColor,new Vector3(light.color.x,light.color.y,light.color.z));

					PrimitiveMeshes.quad.DrawMesh(true);
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