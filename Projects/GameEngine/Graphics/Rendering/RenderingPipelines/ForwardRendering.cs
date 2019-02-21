namespace GameEngine.Graphics.RenderingPipelines
{
	public class ForwardRendering : RenderingPipeline
	{
		public override void Setup(out Framebuffer[] framebuffers,out RenderPass[] renderPasses)
		{
			//Framebuffers
			framebuffers = new Framebuffer[0];

			//RenderPasses
			renderPasses = new[] { new GeometryPass("Geometry") };
		}
		public override void Resize()
		{
			
		}
	}
}