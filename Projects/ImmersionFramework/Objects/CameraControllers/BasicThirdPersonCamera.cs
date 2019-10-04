using GameEngine;

namespace ImmersionFramework
{
	public class BasicThirdPersonCamera : CameraController
	{
		public Vector3 rotation;
		public Vector3 direction;
		public float distance;
		public float distanceSmooth;

		public override Vector3 Rotation {
			get => rotation;
			set {
				rotation = value;
				direction = Vector3.EulerToDirection(value);
			}
		}
		public override Vector3 Direction {
			get => direction;
			set {
				direction = value;
				rotation = Vector3.DirectionToEuler(value);
			}
		}

		public override void OnInit()
		{
			base.OnInit();

			distance = 3f;
		}
		public override void RenderUpdate()
		{
			var delta = Screen.lockCursor ? Vector2.Zero /*new Vector2(GameInput.lookX.Value,GameInput.lookY.Value)*/ : Vector2.Zero;

			/*if(Screen.lockCursor) {
				distance = Mathf.Max(1f,distance-GameInput.zoom.Value);
			}*/

			distanceSmooth = Mathf.Lerp(distanceSmooth,distance,Time.RenderDeltaTime*4f);

			camera.Transform.EulerRot = rotation = new Vector3(
				Mathf.Clamp(rotation.x-delta.y,-89.99f,89.99f),
				rotation.y-delta.x,
				0f
			);

			direction = Vector3.EulerToDirection(rotation);

			Vector3 position = entity.Transform.Position-(direction*distanceSmooth);

			float shake = ScreenShake.GetPowerAtPoint(position);

			camera.Transform.Position = position+new Vector3(Rand.Range(-shake,shake),Rand.Range(-shake,shake),Rand.Range(-shake,shake));
		}
	}
}