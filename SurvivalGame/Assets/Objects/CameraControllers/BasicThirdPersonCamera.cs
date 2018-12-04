using GameEngine;

namespace Game
{
	public class BasicThirdPersonCamera : CameraController
	{
		public Vector3 rotation;
		public Vector3 direction;
		public float distance;

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
			var delta = GameEngine.Game.lockCursor ? Input.MouseDelta*Main.mouseSensitivity : Vector2.zero;

			var newRotation = rotation;
			newRotation.x = Mathf.Clamp(newRotation.x-delta.y,-89.99f,89.99f);
			newRotation.y -= delta.x;
			newRotation.z = 0f;
			camera.Transform.EulerRot = rotation = newRotation;

			direction = Vector3.EulerToDirection(newRotation);
			camera.Transform.Position = entity.Transform.Position-(direction*distance);
		}
	}
}