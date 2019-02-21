using GameEngine;

namespace SurvivalGame
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
			var delta = Screen.lockCursor ? new Vector2(GameInput.lookX.Value,GameInput.lookY.Value) : Vector2.Zero;
			if(Screen.lockCursor) {
				distance = Mathf.Max(1f,distance-GameInput.zoom.Value);
			}
			distanceSmooth = Mathf.Lerp(distanceSmooth,distance,Time.RenderDeltaTime*4f);

			var newRotation = rotation;
			newRotation.x = Mathf.Clamp(newRotation.x-delta.y,-89.99f,89.99f);
			newRotation.y -= delta.x;
			newRotation.z = 0f;
			camera.Transform.EulerRot = rotation = newRotation;

			direction = Vector3.EulerToDirection(newRotation);
			Vector3 position = entity.Transform.Position-(direction*distanceSmooth);
			float shake = ScreenShake.GetPowerAtPoint(position);
			camera.Transform.Position = position+new Vector3(Rand.Range(-shake,shake),Rand.Range(-shake,shake),Rand.Range(-shake,shake));
		}
	}
}