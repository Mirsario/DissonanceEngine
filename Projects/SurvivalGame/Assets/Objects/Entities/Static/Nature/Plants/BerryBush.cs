using GameEngine;
using GameEngine.Graphics;

namespace SurvivalGame
{
	public class BerryBush : StaticEntity,IHasMaterial
	{
		public MeshRenderer renderer;
		
		public override void OnInit()
		{
			base.OnInit();
			
			renderer = AddComponent<MeshRenderer>();
			renderer.LODMeshes = new[] {
				new MeshLOD(Resources.Get<Mesh>("BerryBush.obj"),96f),
				new MeshLOD(Resources.Get<Mesh>("BerryBushLOD2.obj"),384f),
			};
			//TEST
			/*if(!changedNormals) {
				var mesh1 = renderer.LODMeshes[0].mesh;
				var normals1 = new Vector3[mesh1.vertexCount];
				for(int i = 0;i<normals1.Length;i++) {
					normals1[1] = Vector3.up*0.5f;
				}
				mesh1.normals = normals1;
				mesh1.Apply();

				var mesh2 = renderer.LODMeshes[1].mesh;
				var normals2 = new Vector3[mesh2.vertexCount];
				for(int i = 0;i<normals2.Length;i++) {
					normals2[1] = Vector3.up*0.5f;
				}
				mesh2.normals = normals2;
				mesh2.Apply();
				
				changedNormals = true;
			}*/
			renderer.Material = Resources.Find<Material>($"{GetType().Name}");
		}
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
