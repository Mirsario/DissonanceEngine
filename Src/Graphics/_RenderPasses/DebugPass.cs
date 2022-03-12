using Dissonance.Engine.IO;
using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics
{
	public class DebugPass : RenderPass
	{
		public override void Render()
		{
			Framebuffer.BindWithDrawBuffers(Framebuffer);

			GL.Disable(EnableCap.DepthTest);

			var shader = Assets.Find<Shader>("Debug").GetValueImmediately();

			Shader.SetShader(shader);

			var renderViewData = GlobalGet<RenderViewData>();

			// CameraLoop
			foreach (var renderView in renderViewData.RenderViews) {
				var viewport = renderView.Viewport;

				GL.Viewport(viewport.X, viewport.Y, viewport.Width, viewport.Height);

				var viewProj = renderView.ViewMatrix * renderView.ProjectionMatrix;

				Shader.UniformMatrix4(shader.GetUniformLocation("viewProj"), in viewProj);

				Debug.FlushRendering();
			}

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
			//GL.Disable(EnableCap.DepthTest);

			Framebuffer.Bind(null);
		}
	}
}
