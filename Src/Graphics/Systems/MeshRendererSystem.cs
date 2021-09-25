namespace Dissonance.Engine.Graphics
{
	[Reads<GeometryPassData>]
	[Reads<Transform>]
	[Reads<MeshRenderer>]
	[Writes<GeometryPassData>]
	public sealed class MeshRendererSystem : GameSystem
	{
		private EntitySet entities;

		protected internal override void Initialize()
		{
			entities = World.GetEntitySet(e => e.Has<MeshRenderer>() && e.Has<Transform>());
		}

		protected internal override void RenderUpdate()
		{
			ref var geometryPassData = ref GlobalGet<GeometryPassData>();

			foreach (var entity in entities.ReadEntities()) {
				var renderer = entity.Get<MeshRenderer>();
				var transform = entity.Get<Transform>();

				var mesh = renderer.Mesh;
				var material = renderer.Material;

				if (mesh == null || material == null) {
					continue;
				}

				geometryPassData.RenderEntries.Add(new(transform, mesh, material));
			}
		}
	}
}
