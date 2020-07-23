using Dissonance.Engine.Core.Components;
using Dissonance.Engine.Graphics.Components;
using Dissonance.Engine.Graphics.Shaders;
using Dissonance.Engine.Structures;
using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics.RenderPasses.Default
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

			foreach(var camera in ComponentManager.EnumerateComponents<Camera>()) {
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
					viewport.x/(float)Screen.Width,
					viewport.y/(float)Screen.Height,
					viewport.Right/(float)Screen.Width,
					viewport.Bottom/(float)Screen.Height
				);
				var vpPointsB = vpPointsA*2f-Vector4.One;

				DrawUtils.DrawQuadUv0(vpPointsB,vpPointsA);
			}

			GL.Disable(EnableCap.Blend);
		}
	}
}