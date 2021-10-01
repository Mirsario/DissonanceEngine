using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics
{
	public class ScreenHeightUniform : AutomaticUniform<int>
	{
		public override string UniformName { get; } = "screenHeight";

		public override int Calculate(Shader shader, in Transform transform, in RenderViewData.RenderView viewData)
			=> Screen.Height;
	}
}
