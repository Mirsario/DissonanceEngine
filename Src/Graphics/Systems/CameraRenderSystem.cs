namespace Dissonance.Engine.Graphics
{
	[Reads<RenderViewData>]
	[Reads<Camera>]
	[Reads<Transform>]
	[Writes<RenderViewData>]
	public sealed class CameraRenderSystem : GameSystem
	{
		private EntitySet entities;

		protected internal override void Initialize()
		{
			entities = World.GetEntitySet(e => e.Has<Camera>() && e.Has<Transform>());
		}

		protected internal override void RenderUpdate()
		{
			ref var renderViewData = ref GlobalGet<RenderViewData>();

			foreach (var entity in entities.ReadEntities()) {
				var camera = entity.Get<Camera>();
				var transform = entity.Get<Transform>();

				renderViewData.RenderViews.Add(new RenderViewData.RenderView(camera, transform));
			}
		}
	}
}
