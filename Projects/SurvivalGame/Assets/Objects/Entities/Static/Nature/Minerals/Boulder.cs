using GameEngine;
using GameEngine.Graphics;
using GameEngine.Physics;

namespace SurvivalGame
{
	public class Boulder : StaticEntity, IHasMaterial
	{
		public override CollisionMesh CollisionMesh => Resources.Get<ConvexCollisionMesh>($"{GetType().Name}.obj");
		public override (Mesh mesh,Material material)[] RendererData => new[] {
			(Resources.Get<Mesh>($"{GetType().Name}.obj"),Resources.Find<Material>(GetType().Name))
		};

		PhysicMaterial IHasMaterial.GetMaterial(Vector3? atPoint) => PhysicMaterial.GetMaterial<StonePhysicMaterial>();
	}
}
