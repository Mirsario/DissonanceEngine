using Dissonance.Engine.IO;
using Silk.NET.OpenGL;
using static Dissonance.Engine.Graphics.OpenGLApi;

namespace Dissonance.Engine.Graphics;

public class DebugPass : RenderPass
{
	public override void Render()
	{
		Framebuffer.BindWithDrawBuffers(Framebuffer);

		OpenGL.Disable(EnableCap.DepthTest);

		var shader = Assets.Find<Shader>("Debug").GetValueImmediately();

		Shader.SetShader(shader);

		var renderViewData = GlobalGet<RenderViewData>();

		// CameraLoop
		foreach (var renderView in renderViewData.RenderViews) {
			var viewport = renderView.Viewport;

			OpenGL.Viewport(viewport.X, viewport.Y, (uint)viewport.Width, (uint)viewport.Height);

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

			OpenGL.Api.Viewport(viewport.x, viewport.y, viewport.width, viewport.height);

			var viewProj = camera.ViewMatrix * camera.ProjectionMatrix;

			Shader.UniformMatrix4(shader.GetUniformLocation("viewProj"), ref viewProj);

			Debug.FlushRendering();
		}*/

		Shader.SetShader(null);
		//OpenGL.Api.Disable(EnableCap.DepthTest);

		Framebuffer.Bind(null);
	}
}
