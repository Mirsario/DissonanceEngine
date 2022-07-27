using System.Collections.Generic;
using Dissonance.Engine.IO;

namespace Dissonance.Engine.Graphics;

public class ForwardRendering : RenderingPipeline
{
	public override Asset<Shader> DefaultGeometryShader { get; } = Assets.Find<Shader>("Forward/Unlit/Texture");

	public override void Setup(List<Framebuffer> framebuffers, List<RenderPass> renderPasses)
	{
		// RenderPasses
		renderPasses.AddRange(new RenderPass[] {
			// Geometry, our everything
			RenderPass.Create<GeometryPass>("Geometry"),

			// Debug
			RenderPass.Create<DebugPass>("Debug"),

			// GUI
			RenderPass.Create<GUIPass>("GUI")
		});
	}
}
