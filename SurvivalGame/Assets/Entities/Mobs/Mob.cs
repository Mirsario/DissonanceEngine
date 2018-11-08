using System;
using System.Linq;
using System.Collections.Generic;
using GameEngine;

namespace Game
{
	public class Mob : Entity
	{
		public MeshRenderer renderer;
		public CylinderCollider collider;
		public Rigidbody rigidbody;

		public Vector3 size = Vector3.one;
		public Vector3 prevVelocity;
		public Vector3 velocity;

		public float cameraXBonus;

		public bool IsLocal => Main.LocalMob==this;
		public bool OnGround {
			get {
				//TODO: Get BoxCollider offsets to work right, oof.
				return rigidbody.Collisions.Any(c => c.contacts.Any(p => p.point.y>=Transform.Position.y-0.1f));
				//return Physics.Raycast(transform.position+new Vector3(0f,1f,0f),Vector3.down,out RaycastHit hit,1.05f,customFilter: o => o==this ? false : (bool?)null);
			}
		}
		
		public virtual void UpdateIsPlayer(bool isPlayer) {}
		public virtual void UpdateCamera()
		{
			var delta = GameEngine.Game.lockCursor ? Input.MouseDelta : Vector2.zero;
			var cameraRotation = Main.cameraRotation;
			var localVelocity = velocity.RotatedBy(0f,-Main.camera.Transform.EulerRot.y,0f);
			cameraRotation.x = Mathf.Clamp(cameraRotation.x-(delta.y/15f),-89f,89f);
			cameraRotation.y -= delta.x/15f;
			cameraRotation.z = Mathf.Lerp(cameraRotation.z,Mathf.Clamp(localVelocity.x*0.5f,-10f,10f)+(Input.GetDirection(Keys.Q,Keys.E)*15f),Time.DeltaTime*10f);
			Main.camera.Transform.LocalPosition = new Vector3(0f,2f,0f).RotatedBy(0,0f,cameraRotation.z*2f).RotatedBy(0f,cameraRotation.y,0f);
			
			Main.cameraRotation = cameraRotation;
			//cameraXBonus = Mathf.Lerp(cameraXBonus,localVelocity.y*0.5f,Time.DeltaTime*5f)+(localVelocity.z*0.025f);
			//cameraRotation.x += cameraXBonus;
			Main.camera.Transform.EulerRot = cameraRotation;
		}

		public override void OnInit()
		{
			collider = AddComponent<CylinderCollider>(false);
			collider.size = size;
			collider.offset = new Vector3(0f,size.y/2f,0f);
			collider.Enabled = true;

			rigidbody = AddComponent<Rigidbody>();
			rigidbody.Mass = 1f;
			rigidbody.AngularFactor = Vector3.zero;
			
			renderer = AddComponent<MeshRenderer>();
			renderer.Mesh = PrimitiveMeshes.Quad;
			renderer.Material = Resources.Find<Material>("Billboard");
		}
		public override void RenderUpdate()
		{
			if(IsLocal) {
				UpdateCamera();
			}
		}
	}
}

