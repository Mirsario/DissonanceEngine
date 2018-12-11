using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine;

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
			
			var barkMesh = Resources.Get<Mesh>($"{GetType().Name}Bark.obj");

			if(Netplay.isClient) {
				bark = AddComponent<MeshRenderer>();
				bark.LODMesh = new MeshLOD(barkMesh,384f);
				bark.Material = Resources.Find<Material>($"{GetType().Name}Bark");

				branches = AddComponent<MeshRenderer>();
				branches.LODMesh = new MeshLOD(Resources.Get<Mesh>($"{GetType().Name}Branches.obj"),192f);
				branches.Material = Resources.Find<Material>($"{GetType().Name}Branch");
			}
			collider = AddComponent<MeshCollider>();
			collider.Mesh = barkMesh;
			collider.Convex = false;
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
