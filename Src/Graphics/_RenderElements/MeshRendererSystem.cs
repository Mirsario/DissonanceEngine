namespace Dissonance.Engine.Graphics;

[Callback<RenderingCallback>]
[ExecuteBefore<GeometrySortingSystem>]
public sealed class MeshRendererSystem : GameSystem
{
	private EntitySet entities;

	protected override void Initialize(World world)
	{
		entities = world.GetEntitySet(e => e.Has<MeshRenderer>() && e.Has<Transform>());
	}

	protected override void Execute(World world)
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

			var layerMask = entity.Has<Layer>() ? entity.Get<Layer>().Mask : Layers.DefaultLayer.Mask;

			geometryPassData.RenderEntries.Add(new(transform, mesh, material, layerMask));
		}
	}
}
