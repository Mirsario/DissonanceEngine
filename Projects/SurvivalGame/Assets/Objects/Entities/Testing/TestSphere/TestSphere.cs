using GameEngine;
using GameEngine.Graphics;
using GameEngine.Physics;

namespace SurvivalGame
{
	public class TestSphere : Entity, IHasMaterial
	{
		public MeshRenderer renderer;
		public SphereCollider collider;
		public Rigidbody rigidbody;

		public override void OnInit()
		{
			Debug.Log("TestSphere spawned");
			layer = Layers.GetLayerIndex("Entity");

			collider = AddComponent<SphereCollider>(c => c.Radius = 10f);

			renderer = AddComponent<MeshRenderer>(c => {
				c.Mesh = PrimitiveMeshes.Sphere;
				c.Material = Resources.Get<Material>("Entities/Testing/TestSphere/TestSphere.material");
			});

			Transform.LocalScale *= 10f;
		}
		public override void FixedUpdate()
		{
			Transform.EulerRot = new Vector3(-Time.GameTime*5f,Time.GameTime*10f,-Time.GameTime*20f);
		}

		PhysicMaterial IHasMaterial.GetMaterial(Vector3? atPoint) => PhysicMaterial.GetMaterial<DirtPhysicMaterial>();
	}
}