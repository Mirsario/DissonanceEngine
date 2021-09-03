using System.Collections.Generic;

namespace Dissonance.Engine.Graphics
{
	public struct GeometryPassData : IRenderComponent
	{
		public readonly struct RenderEntry
		{
			public readonly Transform transform;
			public readonly Mesh mesh;
			public readonly Material material;

			public RenderEntry(Transform transform, Mesh mesh, Material material)
			{
				this.transform = transform;
				this.mesh = mesh;
				this.material = material;
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
