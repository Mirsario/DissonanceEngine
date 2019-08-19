using GameEngine;
using GameEngine.Graphics;
using GameEngine.Physics;

namespace SurvivalGame
{
	public abstract class Tree : StaticEntity, IHasMaterial
	{
		public MeshRenderer bark; //TODO: Combine these when submeshes are added
		public MeshRenderer branches;
		public MeshCollider collider;
		
		public override void OnInit()
		{
			base.OnInit();

			string typeName = GetType().Name;
			string barkMeshPath = $"{typeName}Bark.obj";

			if(Netplay.isClient) {
				bark = AddComponent<MeshRenderer>(c => {
					c.LODMesh = new MeshLOD(Resources.Get<Mesh>(barkMeshPath),384f);
					c.Material = Resources.Find<Material>($"{typeName}Bark");
				});

				branches = AddComponent<MeshRenderer>(c => {
					c.LODMesh = new MeshLOD(Resources.Get<Mesh>($"{typeName}Branches.obj"),192f);
					c.Material = Resources.Find<Material>($"{typeName}Branch");
				});
			}

			collider = AddComponent<MeshCollider>(c => {
				c.Mesh = Resources.Get<ConvexCollisionMesh>(barkMeshPath);
			});
		}
		public override void FixedUpdate()
		{
			if(Rand.Next(10000)==0) {
				SoundInstance.Create($"Sounds/Atmosphere/Nature/Birds/Bird{Rand.Range(1,10)}.ogg",Transform.Position,2f);
			}
		}
		/*private float soundPlayDelay;
		public override void FixedUpdate()
		{
			if(soundPlayDelay>0f) {
				soundPlayDelay -= Time.DeltaTime;
			}else{
				if(GetGameObjects().FirstOrDefault(
					g => g.Name=="Human"
					&& Vector2.Distance(new Vector2(Transform.Position.x,Transform.Position.z),new Vector2(g.Transform.Position.x,g.Transform.Position.z))<2.75f && Mathf.Abs(Transform.Position.y-g.Transform.Position.y)<7f
				) is Human human && human.velocity.Magnitude>1f) {
					soundPlayDelay = 0.3f;
					SoundInstance.Create($"LeavesThrough{Rand.Range(1,8)}.ogg",human.Transform.Position,Math.Min(1f,(human.velocity.Magnitude-1f)*0.1f));
				}
			}
		}*/

		PhysicMaterial IHasMaterial.GetMaterial(Vector3? atPoint) => PhysicMaterial.GetMaterial<WoodPhysicMaterial>();
	}
}
