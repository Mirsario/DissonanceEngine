namespace Dissonance.Engine.Graphics;

internal static partial class MeshRendering
{
	[EntitySystem, CalledIn<Render>, Tags("RenderData")]
	[Autoload(DisablingGameFlags = GameFlags.NoGraphics)]
	static partial void PushMeshes(Entity entity, ref MeshRenderer renderer, ref Transform transform, [FromGlobal] ref GeometryPassData geometryPassData)
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
