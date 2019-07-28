using GameEngine;
using GameEngine.Graphics;

namespace SurvivalGame
{
	public class CubeObj : GameObject, IHasMaterial
	{
		public MeshRenderer renderer;
		public BoxCollider collider;
		public Rigidbody rigidbody;

		public override void OnInit()
		{
			layer = Layers.GetLayerIndex("Entity");
			collider = AddComponent<BoxCollider>();
			collider.Size = new Vector3(0.2f,1f,0.2f);
			rigidbody = AddComponent<Rigidbody>();
			rigidbody.Mass = 1f;

			renderer = AddComponent<MeshRenderer>();
			renderer.Mesh = PrimitiveMeshes.Cube;

			//Material creation example
			var mat = Resources.Get<Material>("Entities/Testing/TestCube/TestCube.material");
			mat.SetVector3("color",new Vector3(1f,1f,1f));
			renderer.Material = mat;

			/*Light light = AddComponent<Light>();
			light.color = new Vector3(1f,0.8f,1f);
			light.range = 4f;*/
		}
		public override void FixedUpdate()
		{
			var direction = ((Vector3.Right*((Input.GetKey(Keys.Right) ? 1f : 0f)-(Input.GetKey(Keys.Left) ? 1f : 0f)))+
								(Vector3.Forward*((Input.GetKey(Keys.Up) ? 1f : 0f)-(Input.GetKey(Keys.Down) ? 1f : 0f )))).Normalized;
			if(direction!=Vector3.Zero) {
				rigidbody.ApplyForce(direction*250f*Time.FixedDeltaTime,Vector3.Zero);
			}
		}

		PhysicMaterial IHasMaterial.GetMaterial(Vector3? atPoint) => PhysicMaterial.GetMaterial<StonePhysicMaterial>();
	}
}