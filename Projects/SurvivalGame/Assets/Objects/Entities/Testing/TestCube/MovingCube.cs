using System;
using GameEngine;
using GameEngine.Graphics;
using GameEngine.Physics;

namespace SurvivalGame
{
	public class MovingCube : GameObject
	{
		public Func<Vector3> start;
		public Func<Vector3> end;
		public float moveProgress;
		public float moveGoal = 1f;

		public override void OnInit()
		{
			AddComponent<Rigidbody>(c => c.IsKinematic = true);

			AddComponent<MeshRenderer>(c => {
				c.Mesh = PrimitiveMeshes.Cube;
				c.Material = Resources.Get<Material>("Entities/Testing/TestCube/TestCube.material");
			});
		}
		public override void FixedUpdate()
		{
			moveProgress = Mathf.StepTowards(moveProgress,moveGoal,Time.FixedDeltaTime*0.2f);

			if(moveProgress==moveGoal) {
				moveGoal = 1f-moveGoal;
			}

			Transform.Position = Vector3.Lerp(start(),end(),moveProgress);
		}
		public override void RenderUpdate()
		{
			Transform.LocalScale = Vector3.One*(1.5f+Mathf.Sin(Time.GameTime*Mathf.PI));
		}
	}
}