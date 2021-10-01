namespace Dissonance.Engine.Graphics
{
	// World

	internal sealed class WorldUniform : AutomaticUniform<Matrix4x4>
	{
		public override string UniformName { get; } = "world";

		public override Matrix4x4 Calculate(Shader shader, in Transform transform, in RenderViewData.RenderView viewData)
			=> transform.WorldMatrix;
	}

	[AutomaticUniformDependency<WorldUniform>]
	internal sealed class WorldInverseUniform : AutomaticUniform<Matrix4x4>
	{
		public override string UniformName { get; } = "worldInverse";

		public override Matrix4x4 Calculate(Shader shader, in Transform transform, in RenderViewData.RenderView viewData)
			=> GetCache<WorldUniform, Matrix4x4>().Inverted;
	}

	// View

	internal sealed class ViewUniform : AutomaticUniform<Matrix4x4>
	{
		public override string UniformName { get; } = "view";

		public override Matrix4x4 Calculate(Shader shader, in Transform transform, in RenderViewData.RenderView viewData)
			=> viewData.ViewMatrix;
	}

	[AutomaticUniformDependency<ViewUniform>]
	internal sealed class ViewInverseUniform : AutomaticUniform<Matrix4x4>
	{
		public override string UniformName { get; } = "viewInverse";

		public override Matrix4x4 Calculate(Shader shader, in Transform transform, in RenderViewData.RenderView viewData)
			=> GetCache<ViewUniform, Matrix4x4>().Inverted;
	}

	// Projection

	internal sealed class ProjectionUniform : AutomaticUniform<Matrix4x4>
	{
		public override string UniformName { get; } = "proj";

		public override Matrix4x4 Calculate(Shader shader, in Transform transform, in RenderViewData.RenderView viewData)
			=> viewData.ProjectionMatrix;
	}

	[AutomaticUniformDependency<ProjectionUniform>]
	internal sealed class ProjectionInverseUniform : AutomaticUniform<Matrix4x4>
	{
		public override string UniformName { get; } = "projInverse";

		public override Matrix4x4 Calculate(Shader shader, in Transform transform, in RenderViewData.RenderView viewData)
			=> GetCache<ProjectionUniform, Matrix4x4>().Inverted;
	}

	// World * View

	[AutomaticUniformDependency<WorldUniform>]
	[AutomaticUniformDependency<ViewUniform>]
	internal sealed class WorldViewUniform : AutomaticUniform<Matrix4x4>
	{
		public override string UniformName { get; } = "worldView";

		public override Matrix4x4 Calculate(Shader shader, in Transform transform, in RenderViewData.RenderView viewData)
			=> GetCache<WorldUniform, Matrix4x4>() * GetCache<ViewUniform, Matrix4x4>();
	}

	[AutomaticUniformDependency<WorldViewUniform>]
	internal sealed class WorldViewInverseUniform : AutomaticUniform<Matrix4x4>
	{
		public override string UniformName { get; } = "worldViewInverse";

		public override Matrix4x4 Calculate(Shader shader, in Transform transform, in RenderViewData.RenderView viewData)
			=> GetCache<WorldViewUniform, Matrix4x4>().Inverted;
	}

	// World * View * Projection

	[AutomaticUniformDependency<WorldViewUniform>]
	[AutomaticUniformDependency<ProjectionUniform>]
	internal sealed class WorldViewProjectionUniform : AutomaticUniform<Matrix4x4>
	{
		public override string UniformName { get; } = "worldViewProj";

		public override Matrix4x4 Calculate(Shader shader, in Transform transform, in RenderViewData.RenderView viewData)
			=> GetCache<WorldViewUniform, Matrix4x4>() * GetCache<ProjectionUniform, Matrix4x4>();
	}

	[AutomaticUniformDependency<WorldViewProjectionUniform>]
	internal sealed class WorldViewProjectionInverseUniform : AutomaticUniform<Matrix4x4>
	{
		public override string UniformName { get; } = "worldViewProjInverse";

		public override Matrix4x4 Calculate(Shader shader, in Transform transform, in RenderViewData.RenderView viewData)
			=> GetCache<WorldViewProjectionUniform, Matrix4x4>().Inverted;
	}
}
