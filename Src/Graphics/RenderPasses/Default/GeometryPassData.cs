using System.Collections.Generic;

namespace Dissonance.Engine.Graphics
{
	public struct GeometryPassData : IRenderComponent
	{
		public readonly struct RenderEntry
		{
			public readonly Transform Transform;
			public readonly Mesh Mesh;
			public readonly Material Material;
			public readonly LayerMask LayerMask;

			public RenderEntry(Transform transform, Mesh mesh, Material material)
				: this(transform, mesh, material, LayerMask.All) { }

			public RenderEntry(Transform transform, Mesh mesh, Material material, LayerMask layerMask)
			{
				Transform = transform;
				Mesh = mesh;
				Material = material;
				LayerMask = layerMask;
			}
		}

		public List<RenderEntry> RenderEntries { get; private set; }

		public void Reset()
		{
			if (RenderEntries != null) {
				RenderEntries.Clear();
			} else {
				RenderEntries = new();
			}
		}
	}
}
