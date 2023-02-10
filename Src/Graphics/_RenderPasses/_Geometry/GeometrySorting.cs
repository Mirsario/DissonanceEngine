using System.Runtime.InteropServices;

namespace Dissonance.Engine.Graphics;

internal static partial class GeometrySorting
{
	[System, CalledIn<Render>, RequiresTags("RenderData")]
	static partial void SortGeometryRenderEntries([FromGlobal] ref GeometryPassData geometryPassData)
	{
		var renderEntries = CollectionsMarshal.AsSpan(geometryPassData.RenderEntries);

		for (int i = 0; i < renderEntries.Length - 1; i++) {
			for (int j = i + 1; j < renderEntries.Length; j++) {
				ref var a = ref renderEntries[i];
				ref var b = ref renderEntries[j];

				var materialA = a.Material;
				var materialB = b.Material;
				//TODO: This shouldn't be required.
				var shaderA = materialA.Shader.GetValueImmediately();
				var shaderB = materialB.Shader.GetValueImmediately();

				if (shaderA.Priority > shaderB.Priority || shaderA.Id > shaderB.Id || materialA.Id > materialB.Id) {
					(b, a) = (a, b);
				}
			}
		}
	}
}
