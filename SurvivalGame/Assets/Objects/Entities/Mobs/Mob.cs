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

		public float cameraXBonus; //Move all of these
		public float screenShake;

		public bool IsLocal => Main.LocalEntity==this;
		public bool OnGround {
			get {
				//TODO: Get BoxCollider offsets to work right, oof.
				return rigidbody.Collisions.Any(c => c.contacts.Any(p => p.point.y>=Transform.Position.y-0.1f));
				//return Physics.Raycast(transform.position+new Vector3(0f,1f,0f),Vector3.down,out RaycastHit hit,1.05f,customFilter: o => o==this ? false : (bool?)null);
			}
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
				screenShake = Mathf.StepTowards(screenShake,0f,Time.DeltaTime*0.5f);
			}
		}
	}
}

