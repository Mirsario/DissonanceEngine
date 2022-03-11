using Dissonance.Engine.IO;
using Silk.NET.OpenGL;
using static Dissonance.Engine.Graphics.OpenGLApi;

namespace Dissonance.Engine.Graphics
{
	public class PostProcessPass : RenderPass
	{
		public Asset<Shader> PassShader { get; set; }

		public override void Render()
		{
			Framebuffer.BindWithDrawBuffers(Framebuffer);

			OpenGL.Enable(EnableCap.Blend);
			OpenGL.DepthMask(false);

			var passShaderValue = PassShader.GetValueImmediately();

			Shader.SetShader(passShaderValue);

			passShaderValue.SetupCommonUniforms();

			OpenGL.Viewport(0, 0, (uint)Screen.Width, (uint)Screen.Height);

			var renderViewData = GlobalGet<RenderViewData>();

			foreach (var renderView in renderViewData.RenderViews) {
				var transform = renderView.Transform;
				var viewport = renderView.Viewport;

				passShaderValue.SetupCameraUniforms(renderView.NearClip, renderView.FarClip, transform.Position);

				if (PassedTextures != null) {
					for (int j = 0; j < PassedTextures.Length; j++) {
						var texture = PassedTextures[j];

						OpenGL.ActiveTexture((TextureUnit)((int)TextureUnit.Texture0 + j));
						OpenGL.BindTexture(TextureTarget.Texture2D, texture.Id);

						OpenGL.Uniform1(OpenGL.GetUniformLocation(passShaderValue.Id, texture.Name), j);
					}
				}

				var vpPointsA = new Vector4(
					viewport.X / (float)Screen.Width,
					viewport.Y / (float)Screen.Height,
					viewport.Right / (float)Screen.Width,
					viewport.Bottom / (float)Screen.Height
				);
				var vpPointsB = vpPointsA * 2f - Vector4.One;

				DrawUtils.DrawQuadUv0(vpPointsB, vpPointsA);
			}

			OpenGL.DepthMask(true);
			OpenGL.Disable(EnableCap.Blend);
		}
	}
}
