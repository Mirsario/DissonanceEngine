using System;

namespace Dissonance.Engine.Graphics;

[Callback<RenderingCallback>]
public sealed partial class CameraUpdateSystem : GameSystem
{
	[EntitySubsystem]
	partial void OnUpdate(ref Camera camera, in Transform transform)
	{
		// Calculate view and projection matrices, culling frustums
		
		var viewSize = camera.ViewPixel;
		float aspectRatio = viewSize.Width / (float)viewSize.Height;

		camera.ViewMatrix = Matrix4x4.LookAt(transform.Position, transform.Position + transform.Forward, transform.Up);

		if (camera.Orthographic) {
			float max = MathF.Max(Screen.Width, Screen.Height);

			camera.ProjectionMatrix = Matrix4x4.CreateOrthographic(Screen.Width / max * camera.OrthographicSize, Screen.Height / max * camera.OrthographicSize, camera.NearClip, camera.FarClip);
		} else {
			camera.ProjectionMatrix = Matrix4x4.CreatePerspectiveFOV(camera.FieldOfView * MathHelper.Deg2Rad, aspectRatio, camera.NearClip, camera.FarClip);
		}

		camera.InverseViewMatrix = Matrix4x4.Invert(camera.ViewMatrix);
		camera.InverseProjectionMatrix = Matrix4x4.Invert(camera.ProjectionMatrix);

		camera.CalculateFrustum(camera.ViewMatrix * camera.ProjectionMatrix);
	}
}
