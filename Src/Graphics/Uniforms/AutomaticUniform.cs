using System.Collections.Generic;

namespace Dissonance.Engine.Graphics
{
	public abstract class AutomaticUniform
	{
		internal List<int> dependencyIndices = new();

		public abstract string UniformName { get; }

		public IReadOnlyList<int> DependencyIndices { get; }

		internal AutomaticUniform()
		{
			DependencyIndices = (dependencyIndices = new List<int>()).AsReadOnly();
		}

		protected static TData GetCache<TUniform, TData>()
			where TUniform : AutomaticUniform<TData>
			where TData : unmanaged
			=> UniformCache<TUniform, TData>.Value;
	}

	public abstract class AutomaticUniform<TData> : AutomaticUniform where TData : unmanaged
	{
		/// <summary> The function called to upload uniform data to the GPU. </summary>
		public unsafe static delegate*<int, in TData, void> ApplyFunction { get; set; }

		/// <summary> Calculates data to later be uploaded to the gpu and/or used in another uniform. </summary>
		public abstract TData Calculate(Shader shader, in Transform transform, in RenderViewData.RenderView viewData);

		internal void CalculateAndCache<TThis>(Shader shader, in Transform transform, in RenderViewData.RenderView viewData)
			where TThis : AutomaticUniform<TData>
		{
			UniformCache<TThis, TData>.Value = Calculate(shader, in transform, in viewData);
		}

		internal unsafe void ApplyFromCache<TThis>(int location)
			where TThis : AutomaticUniform<TData>
		{
			ApplyFunction(location, in UniformCache<TThis, TData>.Value);
		}
	}
}
