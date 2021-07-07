namespace Dissonance.Engine.Graphics
{
	[Reads(typeof(Camera), typeof(Transform))]
	[Writes(typeof(RenderViewData))]
	public sealed class CameraRenderSystem : GameSystem
	{
		private EntitySet entities;

		public override void Initialize()
		{
			entities = World.GetEntitySet(e => e.Has<Camera>() && e.Has<Transform>());
		}

		public override void RenderUpdate()
		{
			ref var renderViewData = ref GlobalGet<RenderViewData>();

			foreach(var entity in entities.ReadEntities()) {
				var camera = entity.Get<Camera>();
				var transform = entity.Get<Transform>();

				renderViewData.RenderViews.Add(new RenderViewData.RenderView(camera, transform));
			}
		}
	}
}
