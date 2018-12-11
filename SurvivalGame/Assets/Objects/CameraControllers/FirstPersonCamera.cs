using GameEngine;

namespace SurvivalGame
{
	public class FirstPersonCamera : CameraController
	{
		public Vector3 prevPos;
		public Vector3 rotation;
		public Vector3 direction;
		public Vector3 velocity;
		public Vector3 smoothLocalVelocity;
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
			var brain = (Main.LocalEntity as Mob)?.brain;
			bool controlling = brain==null || brain is PlayerBrain;
			var delta = (GameEngine.Game.lockCursor && controlling) ? new Vector2(GameInput.lookX.Value,GameInput.lookY.Value) : Vector2.zero;
			var newRotation = controlling ? rotation : Vector3.LerpAngle(rotation,brain.Transform.EulerRot,Time.DeltaTime*2f);
			newRotation.y -= delta.x;
			smoothLocalVelocity = Vector3.Lerp(smoothLocalVelocity,velocity.RotatedBy(0f,newRotation.y,0f),Time.DeltaTime*10f);
			newRotation.x = Mathf.Clamp(newRotation.x-delta.y,minLockedPitch,maxLockedPitch);
			newRotation.z = Mathf.Clamp(smoothLocalVelocity.x*0.65f,-15f,15f);
			rotation = newRotation;
			//float horizontalSpeed = new Vector2(localVelocity.x,localVelocity.z).Magnitude;
			//bobTime += horizontalSpeed*Time.DeltaTime*3f;
			//float bob = Mathf.Sin(bobTime);
			camera.Transform.EulerRot = new Vector3(Mathf.Clamp(rotation.x+smoothLocalVelocity.y*0.4f,minLockedPitch,maxLockedPitch),rotation.y,rotation.z); //+bob*0.2f);

			direction = Vector3.EulerToDirection(newRotation);
			Vector3 position = entity.Transform.Position+new Vector3(0f,2f,0f);
			float shake = ScreenShake.GetPowerAtPoint(position);
			camera.Transform.Position = position+new Vector3(Rand.Range(-shake,shake),Rand.Range(-shake,shake),Rand.Range(-shake,shake)); //+bob*0.05f
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