using GameEngine;
using GameEngine.Graphics;
using GameEngine.Physics;

namespace SurvivalGame
{
	public class Sapling : StaticEntity, IHasMaterial
	{
		public override CollisionMesh CollisionMesh => null;
		public override (Mesh mesh, Material material)[] RendererData => new[] {
			(Resources.Get<Mesh>($"{GetType().Name}.obj"),Resources.Find<Material>(GetType().Name))
		};

		public override void OnInit()
		{
			base.OnInit();

			static float Random() => (Rand.Next(1f)+Rand.Next(1f))*0.5f;
			static float TiltRotation() => Mathf.Lerp(-15f,15f,Random());

			Transform.LocalScale = Vector3.One*Mathf.Lerp(2f,3f,Random());
			Transform.EulerRot = new Vector3(TiltRotation(),Rand.Next(360f),TiltRotation());
		}

		PhysicMaterial IHasMaterial.GetMaterial(Vector3? atPoint) => PhysicMaterial.GetMaterial<WoodPhysicMaterial>();
	}
}
