namespace Dissonance.Engine.Graphics
{
	[Reads<GeometryPassData>]
	[Reads<Transform>]
	[Reads<MeshRenderer>]
	[Reads<Layer>]
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

				if (renderer.Mesh == null || renderer.Material == null) {
					continue;
				}

				var layerMask = entity.Has<Layer>() ? entity.Get<Layer>().Mask : LayerMask.All;

				geometryPassData.RenderEntries.Add(new(transform, renderer.Mesh, renderer.Material, layerMask));
			}
		}
	}
}
