namespace Dissonance.Engine.Graphics
{
	[Callback<LateRenderUpdateCallback>]
	[ExecuteAfter<CameraUpdateSystem>]
	public sealed class CameraRenderSystem : GameSystem
	{
		private EntitySet entities;

		protected override void Initialize()
		{
			entities = World.GetEntitySet(e => e.Has<Camera>() && e.Has<Transform>());
		}

		protected override void Execute()
		{
			ref var renderViewData = ref Global.Get<RenderViewData>();

			foreach (var entity in entities.ReadEntities()) {
				var camera = entity.Get<Camera>();
				var transform = entity.Get<Transform>();

				renderViewData.RenderViews.Add(new RenderViewData.RenderView(
					transform,
					camera.ViewPixel,
					camera.NearClip,
					camera.FarClip,
					camera.ViewMatrix,
					camera.ProjectionMatrix,
					camera.InverseViewMatrix,
					camera.InverseProjectionMatrix
				));
			}
		}
	}
}
