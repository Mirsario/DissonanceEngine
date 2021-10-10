using Dissonance.Engine.IO;
using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics
{
	public class DebugPass : RenderPass
	{
		public override void Render()
		{
			Framebuffer.BindWithDrawBuffers(Framebuffer);

			GL.Enable(EnableCap.DepthTest);

			var shader = Resources.Find<Shader>("Debug").GetValueImmediately();

			Shader.SetShader(shader);

			// CameraLoop
			/*foreach (var cameraEntity in renderFrame.World.ReadEntities()) {
				if (!cameraEntity.Has<Camera>()) {
					continue;
				}

				var camera = cameraEntity.Get<Camera>();
				var viewport = GetViewport(camera);

				GL.Viewport(viewport.x, viewport.y, viewport.width, viewport.height);

				var viewProj = camera.ViewMatrix * camera.ProjectionMatrix;

				Shader.UniformMatrix4(shader.GetUniformLocation("viewProj"), ref viewProj);

				Debug.FlushRendering();
			}*/

			Shader.SetShader(null);
			GL.Disable(EnableCap.DepthTest);

			Framebuffer.Bind(null);
		}
	}
}
