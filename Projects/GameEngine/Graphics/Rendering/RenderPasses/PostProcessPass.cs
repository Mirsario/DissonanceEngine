using OpenTK.Graphics.OpenGL;
using PrimitiveTypeGL = OpenTK.Graphics.OpenGL.PrimitiveType;

namespace GameEngine.Graphics
{
	public class PostProcessPass : RenderPass
	{
		public override void Render()
		{
			Framebuffer.BindWithDrawBuffers(framebuffer);

			Shader.SetShader(passShader);

			passShader.SetupCommonUniforms();

			for(int i = 0;i<Rendering.cameraList.Count;i++) {
				var camera = Rendering.cameraList[i];

				var viewport = GetViewport(camera);
				GL.Viewport(viewport.x,viewport.y,viewport.width,viewport.height);

				passShader.SetupCameraUniforms(camera,camera.Transform.Position);

				if(passedTextures!=null) {
					for(int j = 0;j<passedTextures.Length;j++) {
						var texture = passedTextures[j];

						GL.ActiveTexture((TextureUnit)((int)TextureUnit.Texture0+j));
						GL.BindTexture(TextureTarget.Texture2D,texture.Id);

						if(passShader!=null) {
							GL.Uniform1(GL.GetUniformLocation(passShader.program,texture.name),j);
						}
					}
				}

				GL.Begin(PrimitiveTypeGL.Quads);
				GL.Vertex2(-1f,1f);
				GL.TexCoord2(0f,1f);
				GL.Vertex2(-1f,-1f);
				GL.TexCoord2(0f,0f);
				GL.Vertex2(1f,-1f);
				GL.TexCoord2(1f,0f);
				GL.Vertex2(1f,1f);
				GL.TexCoord2(1f,1f);
				GL.End();
			}
		}
	}
}