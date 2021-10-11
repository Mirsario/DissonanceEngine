using System;

namespace Dissonance.Engine.Graphics
{
	[Reads<Camera>]
	[Reads<Transform>]
	[Writes<Camera>]
	public sealed class CameraUpdateSystem : GameSystem
	{
		private EntitySet entities;

		protected internal override void Initialize()
		{
			entities = World.GetEntitySet(e => e.Has<Camera>() && e.Has<Transform>());
		}

		protected internal override void RenderUpdate()
		{
			// Calculate view and projection matrices, culling frustums
			foreach (var entity in entities.ReadEntities()) {
				ref var camera = ref entity.Get<Camera>();
				var transform = entity.Get<Transform>();

				var viewSize = camera.ViewPixel;
				float aspectRatio = viewSize.Width / (float)viewSize.Height;

				camera.ViewMatrix = Matrix4x4.LookAt(transform.Position, transform.Position + transform.Forward, transform.Up);

				if (camera.Orthographic) {
					float max = MathF.Max(Screen.Width, Screen.Height);

					camera.ProjectionMatrix = Matrix4x4.CreateOrthographic(Screen.Width / max * camera.OrthographicSize, Screen.Height / max * camera.OrthographicSize, camera.NearClip, camera.FarClip);
				} else {
					camera.ProjectionMatrix = Matrix4x4.CreatePerspectiveFOV(camera.Fov * MathHelper.Deg2Rad, aspectRatio, camera.NearClip, camera.FarClip);
				}

				camera.InverseViewMatrix = Matrix4x4.Invert(camera.ViewMatrix);
				camera.InverseProjectionMatrix = Matrix4x4.Invert(camera.ProjectionMatrix);

				camera.CalculateFrustum(camera.ViewMatrix * camera.ProjectionMatrix);
			}
		}
	}
}
