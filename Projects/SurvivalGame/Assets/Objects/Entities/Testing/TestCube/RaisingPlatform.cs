using GameEngine;
using GameEngine.Graphics;
using GameEngine.Physics;
using ImmersionFramework;

namespace SurvivalGame
{
	public class RaisingPlatform : Entity
	{
		public MeshRenderer renderer;
		public BoxCollider collider;
		public Rigidbody rigidbody;

		public override void OnInit()
		{
			rigidbody = AddComponent<Rigidbody>(c => c.IsKinematic = true);

			collider = AddComponent<BoxCollider>();

			renderer = AddComponent<MeshRenderer>(c => {
				c.Mesh = PrimitiveMeshes.Cube;
				c.Material = Resources.Get<Material>("Entities/Testing/TestCube/TestCube.material");
			});
		}
		public override void FixedUpdate()
		{
			Transform.Position += new Vector3(0f,Mathf.Cos(Time.GameTime)*50f*Time.FixedDeltaTime,0f);
			//rigidbody.AngularFactor = new Vector3(0f,0f,1f);
			//rigidbody.AngularVelocity = new Vector3(0f,0f,100f);
		}
	}
}