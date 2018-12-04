using GameEngine;

namespace Game
{
	public class FirstPersonCamera : CameraController
	{
		public Vector3 prevPos;
		public Vector3 rotation;
		public Vector3 direction;
		public Vector3 velocity;
		public Vector3 smoothVelocity;
		public float pitchBonus;
		public float bobTime;

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

		public override void RenderUpdate()
		{
			var delta = GameEngine.Game.lockCursor ? Input.MouseDelta*Main.mouseSensitivity : Vector2.zero;
			var newRotation = rotation;
			newRotation.y -= delta.x;
			smoothVelocity = Vector3.Lerp(smoothVelocity,velocity,Time.DeltaTime*5f);
			var localVelocity = smoothVelocity.RotatedBy(0f,newRotation.y,0f);
			newRotation.x = Mathf.Clamp(newRotation.x-delta.y,minLockedPitch,maxLockedPitch);
			newRotation.z = Mathf.Clamp(localVelocity.x*0.65f,-15f,15f);
			rotation = newRotation;
			//float horizontalSpeed = new Vector2(localVelocity.x,localVelocity.z).Magnitude;
			//bobTime += horizontalSpeed*Time.DeltaTime*3f;
			//float bob = Mathf.Sin(bobTime);
			camera.Transform.EulerRot = new Vector3(Mathf.Clamp(rotation.x+smoothVelocity.y*0.4f,minLockedPitch,maxLockedPitch),rotation.y,rotation.z); //+bob*0.2f);

			direction = Vector3.EulerToDirection(newRotation);
			camera.Transform.Position = entity.Transform.Position+new Vector3(0f,2f,0f); //+bob*0.05f
		}
		public override void FixedUpdate()
		{
			Vector3 pos = entity.Transform.Position;
			if(Vector3.SqrDistance(pos,prevPos)>64f*64f) {
				prevPos = pos;
			}else{
				velocity = (pos-prevPos)*GameEngine.Game.targetUpdates;
			}
			prevPos = pos;
		}
	}
}