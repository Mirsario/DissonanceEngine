using GameEngine;
using GameEngine.Graphics;
using GameEngine.Physics;

namespace SurvivalGame
{
	public abstract class Tree : StaticEntity, IHasMaterial
	{
		public MeshRenderer[] renderers; //TODO: Combine these when submeshes are added
		public MeshCollider collider;

		public override CollisionMesh CollisionMesh => Resources.Get<ConvexCollisionMesh>($"{GetType().Name}Bark.obj");
		public override (Mesh mesh, Material material)[] RendererData => new[] {
			(Resources.Get<Mesh>($"{GetType().Name}Bark.obj"),Resources.Find<Material>($"{GetType().Name}Bark")),
			(Resources.Get<Mesh>($"{GetType().Name}Branches.obj"),Resources.Find<Material>($"{GetType().Name}Branch"))
		};

		public override void OnInit()
		{
			base.OnInit();

			static float Random() => (Rand.Next(1f)+Rand.Next(1f))*0.5f;
			static float TiltRotation() => Mathf.Lerp(-5f,5f,Random());

			Transform.LocalScale = Vector3.One*Mathf.Lerp(0.5f,2f,Random());
			Transform.EulerRot = new Vector3(TiltRotation(),Rand.Next(360f),TiltRotation());

			/*string typeName = GetType().Name;

			string barkMeshPath = $"{typeName}Bark.obj";
			string branchesMeshPath = $"{typeName}Branches.obj";

			collider = AddComponent<MeshCollider>(c => {
				c.Mesh = Resources.Get<ConvexCollisionMesh>(barkMeshPath);
			});

			if(Netplay.isClient) {
				renderers = new[] {
					AddComponent<MeshRenderer>(c => {
						c.LODMesh = new MeshLOD(Resources.Get<Mesh>(barkMeshPath),Resources.Find<Material>($"{typeName}Bark"),384f);
					}),
					AddComponent<MeshRenderer>(c => {
						c.LODMesh = new MeshLOD(Resources.Get<Mesh>(branchesMeshPath),Resources.Find<Material>($"{typeName}Branch"),192f);
					})
				};
			}*/
		}

		/*public override void FixedUpdate()
		{
			if(Rand.Next(10000)==0) {
				SoundInstance.Create($"Sounds/Atmosphere/Nature/Birds/Bird{Rand.Range(1,10)}.ogg",Transform.Position,2f);
			}
		}

		private float soundPlayDelay;
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
