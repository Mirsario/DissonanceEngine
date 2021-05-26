namespace Dissonance.Engine.Graphics
{
	public sealed class RenderPassSystem : GameSystem
	{
		public override void RenderUpdate()
		{
			var pipeline = Rendering.RenderingPipeline;

			for(int i = 0; i < pipeline.RenderPasses.Length; i++) {
				var pass = pipeline.RenderPasses[i];

				if(pass.enabled) {
					pass.Render(World);

					Rendering.CheckGLErrors($"Rendering pass {pass.name} ({pass.GetType().Name})");
				}
			}

			Framebuffer.Bind(null);
		}
	}
}
