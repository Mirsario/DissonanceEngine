using GameEngine;
using GameEngine.Graphics;
using GameEngine.Physics;

namespace SurvivalGame
{
	public class BerryBush : StaticEntity, IHasMaterial
	{
		public MeshRenderer renderer;

		public override CollisionMesh CollisionMesh => null;
		public override (Mesh mesh, Material material)[] RendererData => new[] {
			(Resources.Get<Mesh>($"{GetType().Name}.obj"),Resources.Find<Material>(GetType().Name))
		};

		/*public override void OnInit()
		{
			base.OnInit();
			
			renderer = AddComponent<MeshRenderer>(c => {
				c.LODMeshes = new[] {
					new MeshLOD(Resources.Get<Mesh>("BerryBush.obj"),96f),
					new MeshLOD(Resources.Get<Mesh>("BerryBushLOD2.obj"),384f),
				};
				c.Material = Resources.Find<Material>($"{GetType().Name}");
			});
		}*/

		//private float soundPlayDelay;
		/*public override void FixedUpdate()
		{
			if(soundPlayDelay>0f) {
				soundPlayDelay -= Time.DeltaTime;
			}else{
				if(GetGameObjects().FirstOrDefault(g => g.Name=="Human" && Vector3.Distance(Transform.Position,g.Transform.Position) < 2f) is Human human && human.velocity.Magnitude>1f) {
					soundPlayDelay = 0.3f;
					SoundInstance.Create($"LeavesThrough{Rand.Range(1,8)}.ogg",Transform.Position,Math.Min(1f,(human.velocity.Magnitude-1f)*0.1f));
				}
			}
		}*/

		PhysicMaterial IHasMaterial.GetMaterial(Vector3? atPoint) => PhysicMaterial.GetMaterial<StonePhysicMaterial>();
	}
}
