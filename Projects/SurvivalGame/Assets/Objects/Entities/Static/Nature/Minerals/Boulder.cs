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

			string typeName = GetType().Name;
			string meshPath = $"{typeName}.obj";

			AddComponent<MeshRenderer>(c => {
				c.Mesh = Resources.Get<Mesh>(meshPath);
				c.Material = Resources.Find<Material>(typeName);
			});

			AddComponent<MeshCollider>(c => {
				c.Mesh = Resources.Get<ConvexCollisionMesh>(meshPath);
			});
		}

		PhysicMaterial IHasMaterial.GetMaterial(Vector3? atPoint) => PhysicMaterial.GetMaterial<StonePhysicMaterial>();
	}
}
