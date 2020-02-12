using Dissonance.Framework.OpenGL;

namespace Dissonance.Engine.Graphics
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

				DrawUtils.DrawQuadUv0(vpPointsB,vpPointsA);
			}

			GL.Disable(EnableCap.Blend);
		}
	}
}