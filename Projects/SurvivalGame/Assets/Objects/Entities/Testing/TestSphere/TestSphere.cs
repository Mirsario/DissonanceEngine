using GameEngine;
using GameEngine.Graphics;

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
			collider = AddComponent<SphereCollider>();
			collider.radius = 10f;

			renderer = AddComponent<MeshRenderer>();
			renderer.Mesh = PrimitiveMeshes.Sphere;
			renderer.Material = Resources.Get<Material>("Entities/Testing/TestSphere/TestSphere.material");

			Transform.LocalScale *= 10f;
		}
		public override void FixedUpdate()
		{
			Transform.EulerRot = new Vector3(-Time.GameTime*5f,Time.GameTime*10f,-Time.GameTime*20f);
		}

		PhysicMaterial IHasMaterial.GetMaterial(Vector3? atPoint) => PhysicMaterial.GetMaterial<DirtPhysicMaterial>();
	}
}