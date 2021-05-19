using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics
{
	[SystemDependency(typeof(RenderPassSystem))]
	public sealed class RenderFramebufferDebugSystem : RenderSystem
	{
		public override void Update()
		{
			if(!Rendering.DebugFramebuffers) {
				return;
			}

			static bool IsDepthTexture(RenderTexture tex)
				=> tex.PixelFormat == PixelFormat.DepthComponent;

			var renderingPipeline = Rendering.RenderingPipeline;
			var framebuffers = renderingPipeline.framebuffers;
			int textureCount = 0;

			for(int i = 0; i < framebuffers.Length; i++) {
				var fb = framebuffers[i];

				for(int j = 0; j < fb.renderTextures.Count; j++) {
					var tex = fb.renderTextures[j];

					if(!IsDepthTexture(tex)) {
						textureCount++;
					}
				}
			}

			int size = 1;

			while(size * size < textureCount) {
				size++;
			}

			int x = 0;
			int y = 0;

			for(int i = 0; i < framebuffers.Length; i++) {
				var framebuffer = framebuffers[i];

				Framebuffer.Bind(framebuffer, FramebufferTarget.ReadFramebuffer);

				for(int j = 0; j < framebuffer.renderTextures.Count; j++) {
					var tex = framebuffer.renderTextures[j];

					if(IsDepthTexture(tex)) {
						continue;
					}

					GL.ReadBuffer(ReadBufferMode.ColorAttachment0 + j);

					int wSize = Screen.Width / size;
					int hSize = Screen.Height / size;

					GL.BlitFramebuffer(
						0, 0, tex.Width, tex.Height,
						x * wSize, (size - y - 1) * hSize, (x + 1) * wSize, (size - y) * hSize,
						ClearBufferMask.ColorBufferBit,
						BlitFramebufferFilter.Nearest
					);

					if(++x >= size) {
						x = 0;
						y++;
					}
				}
			}
		}
	}
}
