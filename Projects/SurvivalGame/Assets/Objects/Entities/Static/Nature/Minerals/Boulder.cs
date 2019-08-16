using GameEngine;
using GameEngine.Graphics;

namespace SurvivalGame
{
	public class Boulder : StaticEntity, IHasMaterial
	{
		public MeshRenderer renderer;
		public MeshCollider collider;
		
		public override void OnInit()
		{
			base.OnInit();
			
			var mesh = Resources.Get<Mesh>($"{GetType().Name}.obj");

			renderer = AddComponent<MeshRenderer>();
			renderer.Mesh = mesh;
			renderer.Material = Resources.Find<Material>(GetType().Name);

			collider = AddComponent<MeshCollider>(enable:false);
			collider.Mesh = mesh;
			collider.Convex = false;
			collider.Enabled = true;
		}

		PhysicMaterial IHasMaterial.GetMaterial(Vector3? atPoint) => PhysicMaterial.GetMaterial<StonePhysicMaterial>();
	}
}
