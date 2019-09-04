namespace GameEngine.Graphics.RenderingPipelines
{
	public class ForwardRendering : RenderingPipeline
	{
		public override void Setup(out Framebuffer[] framebuffers,out RenderPass[] renderPasses)
		{
			//Framebuffers
			framebuffers = new Framebuffer[0];

			//RenderPasses
			renderPasses = new RenderPass[] {
				//Geometry, our everything
				RenderPass.Create<GeometryPass>("Geometry"),

				//GUI
				RenderPass.Create<GUIPass>("GUI")
			};
		}
	}
}