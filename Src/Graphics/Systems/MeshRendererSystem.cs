namespace Dissonance.Engine.Graphics
{
	[Callback<LateRenderUpdateCallback>]
	public sealed class MeshRendererSystem : GameSystem
	{
		private EntitySet entities;

		protected override void Initialize()
		{
			entities = World.GetEntitySet(e => e.Has<MeshRenderer>() && e.Has<Transform>());
		}

		protected override void Execute()
		{
			ref var geometryPassData = ref Global.Get<GeometryPassData>();

			foreach (var entity in entities.ReadEntities()) {
				var renderer = entity.Get<MeshRenderer>();
				var transform = entity.Get<Transform>();

				if (renderer.Mesh == null || renderer.Material == null) {
					continue;
				}

				if (!renderer.Mesh.TryGetOrRequestValue(out var mesh) || !renderer.Material.TryGetOrRequestValue(out var material)) {
					continue;
				}

				var layerMask = entity.Has<Layer>() ? entity.Get<Layer>().Mask : LayerMask.All;

				geometryPassData.RenderEntries.Add(new(transform, mesh, material, layerMask));
			}
		}
	}
}
