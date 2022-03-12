namespace Dissonance.Engine.Graphics
{
	[Callback<RenderingCallback>]
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

				renderViewData.RenderViews.Add(new RenderViewData.RenderView {
					Transform = transform,
					Viewport = camera.ViewPixel,
					NearClip = camera.NearClip,
					FarClip = camera.FarClip,
					ViewMatrix = camera.ViewMatrix,
					ProjectionMatrix = camera.ProjectionMatrix,
					InverseViewMatrix = camera.InverseViewMatrix,
					InverseProjectionMatrix = camera.InverseProjectionMatrix,
					LayerMask = camera.LayerMask,
				});
			}
		}
	}
}
