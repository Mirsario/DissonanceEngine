namespace Dissonance.Engine.Graphics;

[Callback<RenderingCallback>]
[ExecuteBefore<GeometrySortingSystem>]
public sealed partial class MeshRendererSystem : GameSystem
{
	[EntitySubsystem]
	partial void RenderMeshes(Entity entity, in MeshRenderer renderer, in Transform transform, [FromGlobal] ref GeometryPassData geometryPassData)
	{
		if (renderer.Mesh == null || renderer.Material == null) {
			return;
		}

		if (!renderer.Mesh.TryGetOrRequestValue(out var mesh) || !renderer.Material.TryGetOrRequestValue(out var material)) {
			return;
		}

		var layerMask = entity.Has<Layer>() ? entity.Get<Layer>().Mask : Layers.DefaultLayer.Mask;

		geometryPassData.RenderEntries.Add(new(transform, mesh, material, layerMask));
	}
}
