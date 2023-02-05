namespace Dissonance.Engine.Graphics;

internal static partial class CameraRendering
{
	[EntitySystem, CalledIn<Render>, Tags("RenderData")]
	[Autoload(DisablingGameFlags = GameFlags.NoGraphics)]
	static partial void PushCameras(in Camera camera, in Transform transform, [FromGlobal] ref RenderViewData renderViewData)
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
