using Dissonance.Engine.IO;
using Dissonance.Engine.Physics;
using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics
{
	/*public class DebugPass : RenderPass
	{
		public override void Render()
		{
			Framebuffer.BindWithDrawBuffers(framebuffer);

			GL.Enable(EnableCap.DepthTest);

			var shader = Resources.Find<Shader>("Debug");

			Shader.SetShader(shader);

			//CameraLoop
			foreach(var camera in ComponentManager.EnumerateComponents<Camera>()) {
				var viewport = GetViewport(camera);

				GL.Viewport(viewport.x, viewport.y, viewport.width, viewport.height);

				var viewProj = camera.matrix_view * camera.matrix_proj;

				Shader.UniformMatrix4(shader.GetUniformLocation("viewProj"), ref viewProj);

				Debug.FlushRendering();
			}

			Shader.SetShader(null);
			GL.Disable(EnableCap.DepthTest);

			Framebuffer.Bind(null);
		}
	}*/
}
