using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics
{
	public class ScreenWidthUniform : AutomaticUniform<int>
	{
		public override string UniformName { get; } = "screenWidth";

		public override int Calculate(Shader shader, in Transform transform, in RenderViewData.RenderView viewData)
			=> Screen.Width;

		public override void Apply(int location, in int value)
			=> GL.Uniform1(location, value);
	}
}
