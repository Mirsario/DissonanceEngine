using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics
{
	public sealed class ClearScreenSystem : SystemBase
	{
		public override void RenderUpdate()
		{
			GL.Viewport(0, 0, Screen.Width, Screen.Height);

			var pipeline = Rendering.RenderingPipeline;
			var clearColor = Rendering.ClearColor;

			if(pipeline.Framebuffers != null) {
				int length = pipeline.Framebuffers.Length;

				for(int i = 0; i <= length; i++) {
					var framebuffer = i == length ? null : pipeline.Framebuffers[i];

					framebuffer?.PrepareAttachments();

					Framebuffer.BindWithDrawBuffers(framebuffer);

					GL.ClearColor(clearColor.x, clearColor.y, clearColor.z, clearColor.w);
					//GL.StencilMask(~0);
					GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
				}
			}

			Framebuffer.Bind(null);
		}
	}
}
