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

			AddComponent<MeshRenderer>(c => {
				c.Mesh = mesh;
				c.Material = Resources.Find<Material>(GetType().Name);
			});

			AddComponent<MeshCollider>(c => {
				c.Mesh = mesh;
				c.Convex = true;
			});
		}

		PhysicMaterial IHasMaterial.GetMaterial(Vector3? atPoint) => PhysicMaterial.GetMaterial<StonePhysicMaterial>();
	}
}
