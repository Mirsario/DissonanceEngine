using System;
using System.Collections.Generic;
using GameEngine;

namespace Game
{
	public class MovingCube : GameObject
	{
		public MeshRenderer renderer;
		public BoxCollider collider;
		public Rigidbody rigidbody;
		public Func<Vector3> start;
		public Func<Vector3> end;
		public float moveProgress = 0f;
		public float moveGoal = 1f;
		public AudioSource source;

		public override void OnInit()
		{
			layer = Layers.GetLayerIndex("Box");
			
			rigidbody = AddComponent<Rigidbody>();
			rigidbody.IsKinematic = true;

			renderer = AddComponent<MeshRenderer>();
			renderer.Mesh = PrimitiveMeshes.Cube;
			renderer.Material = Resources.Get<Material>("Entities/Testing/TestCube/TestCube.material");

			source = AddComponent<AudioSource>();
			source.Clip = Resources.Get<AudioClip>("Sounds/Action2v2.ogg");
			source.Loop = true;
			source.Volume = 10f;
			if(Main.enableMusic) {
				source.Play();
			}
		}
		public override void FixedUpdate()
		{
			//Debug.Log(Vector3.Distance(transform.position,Main.camera.transform.position));
			moveProgress = Mathf.StepTowards(moveProgress,moveGoal,Time.DeltaTime*0.2f);
			if(moveProgress==moveGoal) {
				moveGoal = 1f-moveGoal;
			}
			Transform.Position = Vector3.Lerp(start(),end(),moveProgress);
		}
		public override void RenderUpdate()
		{
			Transform.LocalScale = Vector3.one*(1.5f+Mathf.Sin01(Time.GameTime));
		}
	}
}