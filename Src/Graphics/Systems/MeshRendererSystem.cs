namespace Dissonance.Engine.Graphics
{
	[Reads(typeof(Transform), typeof(MeshRenderer))]
	[Writes(typeof(GeometryPassData))]
	public sealed class MeshRendererSystem : GameSystem
	{
		private EntitySet entities;

		public override void Initialize()
		{
			entities = World.GetEntitySet(e => e.Has<MeshRenderer>() && e.Has<Transform>());
		}

		public override void RenderUpdate()
		{
			ref var geometryPassData = ref GlobalGet<GeometryPassData>();

			geometryPassData.RenderEntries ??= new();

			geometryPassData.RenderEntries.Clear(); //temp

			foreach(var entity in entities.ReadEntities()) {
				var renderer = entity.Get<MeshRenderer>();
				var transform = entity.Get<Transform>();

				var mesh = renderer.Mesh;
				var material = renderer.Material;

				if(mesh == null || material == null) {
					continue;
				}

				geometryPassData.RenderEntries.Add(new(transform, mesh, material));
			}
		}
	}
}
