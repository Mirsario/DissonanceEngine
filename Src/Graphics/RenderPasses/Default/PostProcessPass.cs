using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics
{
	public class PostProcessPass : RenderPass
	{
		public override void Render(World world)
		{
			Framebuffer.BindWithDrawBuffers(framebuffer);

			GL.Enable(EnableCap.Blend);
			GL.DepthMask(false);

			Shader.SetShader(passShader);

			passShader.SetupCommonUniforms();

			GL.Viewport(0, 0, Screen.Width, Screen.Height);

			foreach(var entity in world.ReadEntities()) {
				if(!entity.Has<Camera>() || !entity.Has<Transform>()) {
					continue;
				}

				var camera = entity.Get<Camera>();
				var transform = entity.Get<Transform>();
				var viewport = GetViewport(camera);

				passShader.SetupCameraUniforms(camera, transform.Position);

				if(passedTextures != null) {
					for(int j = 0; j < passedTextures.Length; j++) {
						var texture = passedTextures[j];

						GL.ActiveTexture((TextureUnit)((int)TextureUnit.Texture0 + j));
						GL.BindTexture(TextureTarget.Texture2D, texture.Id);

						if(passShader != null) {
							GL.Uniform1(GL.GetUniformLocation(passShader.Id, texture.name), j);
						}
					}
				}

				var vpPointsA = new Vector4(
					viewport.x / (float)Screen.Width,
					viewport.y / (float)Screen.Height,
					viewport.Right / (float)Screen.Width,
					viewport.Bottom / (float)Screen.Height
				);
				var vpPointsB = vpPointsA * 2f - Vector4.One;

				DrawUtils.DrawQuadUv0(vpPointsB, vpPointsA);
			}

			GL.DepthMask(true);
			GL.Disable(EnableCap.Blend);
		}
	}
}
