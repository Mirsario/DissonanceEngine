using GameEngine;
using GameEngine.Graphics;
using GameEngine.Extensions.Chains;
using GameEngine.Physics;

namespace SurvivalGame
{
	public class Boulder : StaticEntity, IHasMaterial
	{
		public override void OnInit()
		{
			base.OnInit();

			var mesh = Resources.Get<Mesh>($"{GetType().Name}.obj");

			AddComponent<MeshRenderer>()
				.WithMesh(mesh)
				.WithMaterial(Resources.Find<Material>(GetType().Name));

			AddComponent<MeshCollider>(false)
				.WithMesh(mesh)
				.WithConvex(true)
				.Enable();
		}

		PhysicMaterial IHasMaterial.GetMaterial(Vector3? atPoint) => PhysicMaterial.GetMaterial<StonePhysicMaterial>();
	}
}
