namespace Dissonance.Engine.Graphics;

[Callback<RenderingCallback>]
[ExecuteAfter<CameraUpdateSystem>]
public sealed partial class CameraRenderSystem : GameSystem
{
	[EntitySubsystem]
	partial void UpdateCameras(in Camera camera, in Transform transform, [FromGlobal] ref RenderViewData renderViewData)
	{
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
