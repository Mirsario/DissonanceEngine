using OpenTK.Graphics.OpenGL;
using PrimitiveTypeGL = OpenTK.Graphics.OpenGL.PrimitiveType;

namespace GameEngine.Graphics
{
	public class PostProcessPass : RenderPass
	{
		public override void Render()
		{
			Framebuffer.BindWithDrawBuffers(framebuffer);

			GL.Enable(EnableCap.Blend);

			Shader.SetShader(passShader);

			passShader.SetupCommonUniforms();

			GL.Viewport(0,0,Screen.Width,Screen.Height);

			for(int i = 0;i<Rendering.cameraList.Count;i++) {
				var camera = Rendering.cameraList[i];

				var viewport = GetViewport(camera);

				passShader.SetupCameraUniforms(camera,camera.Transform.Position);

				if(passedTextures!=null) {
					for(int j = 0;j<passedTextures.Length;j++) {
						var texture = passedTextures[j];

						GL.ActiveTexture((TextureUnit)((int)TextureUnit.Texture0+j));
						GL.BindTexture(TextureTarget.Texture2D,texture.Id);

						if(passShader!=null) {
							GL.Uniform1(GL.GetUniformLocation(passShader.Id,texture.name),j);
						}
					}
				}

				var vpPointsA = new Vector4(
					viewport.x/(float)Screen.width,
					viewport.y/(float)Screen.height,
					viewport.Right/(float)Screen.width,
					viewport.Bottom/(float)Screen.height
				);
				var vpPointsB = (vpPointsA*2f)-Vector4.One;

				GL.Begin(PrimitiveTypeGL.Quads);

				GL.Vertex2(vpPointsB.x,vpPointsB.w); GL.TexCoord2(vpPointsA.x,vpPointsA.w);
				GL.Vertex2(vpPointsB.x,vpPointsB.y); GL.TexCoord2(vpPointsA.x,vpPointsA.y);
				GL.Vertex2(vpPointsB.z,vpPointsB.y); GL.TexCoord2(vpPointsA.z,vpPointsA.y);
				GL.Vertex2(vpPointsB.z,vpPointsB.w); GL.TexCoord2(vpPointsA.z,vpPointsA.w);

				GL.End();
			}

			GL.Disable(EnableCap.Blend);
		}
	}
}