using GameEngine;
using GameEngine.Graphics;
using GameEngine.Physics;

namespace SurvivalGame
{
	public class Boulder : StaticEntity, IHasMaterial
	{
		private int variant;

		public override CollisionMesh CollisionMesh => Resources.Get<ConvexCollisionMesh>($"{GetType().Name}{variant}.obj");
		public override (Mesh mesh,Material material)[] RendererData => new[] {
			(Resources.Get<Mesh>($"{GetType().Name}{variant}.obj"),Resources.Find<Material>(GetType().Name))
		};

		public override void OnInit()
		{
			base.OnInit();

			variant = Rand.Next(2)+1;

			static float Random() => (Rand.Next(1f)+Rand.Next(1f))*0.5f;
			static float TiltRotation() => Mathf.Lerp(-15f,15f,Random());

			Transform.LocalScale = Vector3.One*Mathf.Lerp(0.25f,1.75f,Random());
			Transform.EulerRot = new Vector3(TiltRotation(),Rand.Next(360f),TiltRotation());
		}

		PhysicMaterial IHasMaterial.GetMaterial(Vector3? atPoint) => PhysicMaterial.GetMaterial<StonePhysicMaterial>();
	}
}
