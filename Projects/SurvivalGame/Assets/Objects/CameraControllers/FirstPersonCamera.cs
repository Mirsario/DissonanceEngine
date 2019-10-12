using GameEngine;
using ImmersionFramework;
using SurvivalGame.Inputs;

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
			var inputSource = (entity as IInputProxyProvaider)?.Proxy;
			//var delta = inputSource.Value<LookX,LookY>();

			var newRotation = inputSource.LookRotation; // : Vector3.LerpAngle(rotation,brain.Transform.EulerRot,Time.RenderDeltaTime*2f);
			//newRotation.y -= delta.x;

			smoothLocalVelocity = Vector3.Lerp(smoothLocalVelocity,velocity.RotatedBy(0f,newRotation.y,0f),Time.RenderDeltaTime*10f);

			//newRotation.x = Mathf.Clamp(newRotation.x,MinLockedPitch,MaxLockedPitch);
			newRotation.z = Mathf.Clamp(smoothLocalVelocity.x*0.65f,-15f,15f);

			rotation = newRotation;

			//Debug.Log("c: "+inputSource.LookRotation);
			//Debug.Log("d: "+rotation);

			camera.Transform.EulerRot = new Vector3(rotation.x+smoothLocalVelocity.y*0.4f,rotation.y,rotation.z); //+bob*0.2f);
			//camera.Transform.EulerRot = new Vector3(Mathf.Clamp(rotation.x+smoothLocalVelocity.y*0.4f,MinLockedPitch,MaxLockedPitch),rotation.y,rotation.z); //+bob*0.2f);

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
				velocity = (pos-prevPos)/Time.FixedDeltaTime;
			}

			prevPos = pos;
		}
	}
}