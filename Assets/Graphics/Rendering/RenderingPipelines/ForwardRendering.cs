using System.Collections.Generic;

namespace GameEngine.Graphics.RenderingPipelines
{
	public class ForwardRendering : RenderingPipeline
	{
		public override void Setup(List<Framebuffer> framebuffers,List<RenderPass> renderPasses)
		{
			//RenderPasses
			renderPasses.AddRange(new RenderPass[] {
				//Geometry, our everything
				RenderPass.Create<GeometryPass>("Geometry"),

				//GUI
				RenderPass.Create<GUIPass>("GUI")
			});
		}
	}
}