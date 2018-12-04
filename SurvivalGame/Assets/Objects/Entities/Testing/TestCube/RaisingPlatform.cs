using System;
using System.Collections.Generic;
using GameEngine;

namespace Game
{
	public class RaisingPlatform : Entity
	{
		public MeshRenderer renderer;
		public BoxCollider collider;
		public Rigidbody rigidbody;

		public override void OnInit()
		{
			//layer = Layers.GetLayerIndex("Box");
			
			rigidbody = AddComponent<Rigidbody>();
			rigidbody.IsKinematic = true;

			collider = AddComponent<BoxCollider>();

			renderer = AddComponent<MeshRenderer>();
			renderer.Mesh = PrimitiveMeshes.Cube;
			renderer.Material = Resources.Get<Material>("Entities/Testing/TestCube/TestCube.material");
		}
		public override void FixedUpdate()
		{
			Transform.Position += new Vector3(0f,Mathf.Cos(Time.GameTime)*50f*Time.DeltaTime,0f);
			//rigidbody.AngularFactor = new Vector3(0f,0f,1f);
			//rigidbody.AngularVelocity = new Vector3(0f,0f,100f);
		}
	}
}