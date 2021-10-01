namespace Dissonance.Engine.Graphics
{
	public abstract class MatrixUniform : AutomaticUniform<Matrix4x4>
	{
		public sealed override void Apply(int location, in Matrix4x4 value)
			=> Shader.UniformMatrix4(location, in value);
	}

	public class WorldUniform : MatrixUniform
	{
		public override string UniformName { get; } = "world";

		public override Matrix4x4 Calculate(Shader shader, in Transform transform, in RenderViewData.RenderView viewData)
			=> transform.WorldMatrix;
	}

	public class ViewUniform : MatrixUniform
	{
		public override string UniformName { get; } = "view";

		public override Matrix4x4 Calculate(Shader shader, in Transform transform, in RenderViewData.RenderView viewData)
			=> viewData.ViewMatrix;
	}

	public class ProjectionUniform : MatrixUniform
	{
		public override string UniformName { get; } = "proj";

		public override Matrix4x4 Calculate(Shader shader, in Transform transform, in RenderViewData.RenderView viewData)
			=> viewData.ProjectionMatrix;
	}

	[AutomaticUniformDependency<WorldUniform>]
	[AutomaticUniformDependency<ViewUniform>]
	public class WorldViewUniform : MatrixUniform
	{
		public override string UniformName { get; } = "worldView";

		public override Matrix4x4 Calculate(Shader shader, in Transform transform, in RenderViewData.RenderView viewData)
			=> GetCache<WorldUniform, Matrix4x4>() * GetCache<ViewUniform, Matrix4x4>();
	}

	[AutomaticUniformDependency<WorldViewUniform>]
	[AutomaticUniformDependency<ProjectionUniform>]
	public class WorldViewProjectionUniform : MatrixUniform
	{
		public override string UniformName { get; } = "worldViewProj";

		public override Matrix4x4 Calculate(Shader shader, in Transform transform, in RenderViewData.RenderView viewData)
			=> GetCache<WorldViewUniform, Matrix4x4>() * GetCache<ProjectionUniform, Matrix4x4>();
	}
}
