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
		public float moveProgress;
		public float moveGoal = 1f;

		public override void OnInit()
		{
			layer = Layers.GetLayerIndex("Box");
			
			rigidbody = AddComponent<Rigidbody>();
			rigidbody.IsKinematic = true;

			renderer = AddComponent<MeshRenderer>();
			renderer.Mesh = PrimitiveMeshes.Cube;
			renderer.Material = Resources.Get<Material>("Entities/Testing/TestCube/TestCube.material");
		}
		public override void FixedUpdate()
		{
			moveProgress = Mathf.StepTowards(moveProgress,moveGoal,Time.DeltaTime*0.2f);
			if(moveProgress==moveGoal) {
				moveGoal = 1f-moveGoal;
			}
			Transform.Position = Vector3.Lerp(start(),end(),moveProgress);
		}
		public override void RenderUpdate()
		{
			Transform.LocalScale = Vector3.one*(1.5f+Mathf.Sin(Time.GameTime*Mathf.PI));
		}
	}
}