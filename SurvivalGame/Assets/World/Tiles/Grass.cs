using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine;

namespace Game
{
	public class Grass : TileType
	{
		protected override string[] Variants => new[] {
			"Grass1.png",
			"Grass2.png"
		};
		public override void OnInit()
		{
			grassMaterial = "TallGrass";
		}

		private static readonly Vector2[] uvPack = {
			new Vector2(0f,0f),
			new Vector2(1f,0f),
			new Vector2(0f,1f),
			new Vector2(1f,1f)
		};
		public override void ModifyGrassMesh(Chunk chunk,Tile tile,Vector2Int tilePos,Vector3 localPos,Vector3 tileNormal,MeshInfo mesh)
		{
			const float grassSize = 2f;
			for(int i = 0;i<3;i++) {
				var pos = new Vector3(localPos.x+(Chunk.tileSize*Rand.Range(0f,1f)),0f,localPos.z+(Chunk.tileSize*Rand.Range(0f,1f)));
				pos.y = chunk.world.HeightAt(pos+chunk.WorldPoint,false)-grassSize*0.5f;

				var rot = Rand.Range(0f,360f);
				var angle = Turn(new Vector3(0,0f,grassSize*0.5f),rot);
							
				mesh.vertices.Add(new Vector3(pos.x+angle.x,pos.y,			pos.z+angle.z));
				mesh.vertices.Add(new Vector3(pos.x-angle.x,pos.y,			pos.z-angle.z));
				mesh.vertices.Add(new Vector3(pos.x+angle.x,pos.y+grassSize,pos.z+angle.z));
				mesh.vertices.Add(new Vector3(pos.x-angle.x,pos.y+grassSize,pos.z-angle.z));
				int c = mesh.vertices.Count;
				mesh.triangles.AddRange(new[] { c-1,c-4,c-3,c-4,c-1,c-2 });
				mesh.normals.AddRange(new[] { tileNormal,tileNormal,tileNormal,tileNormal });
				mesh.uvs.AddRange(uvPack);
			}
		}
		public override PhysicMaterial GetMaterial(Vector3? atPoint = null) => PhysicMaterial.GetMaterial<GrassPhysicMaterial>();

		private static Vector3 Turn(Vector3 v,float degrees)
		{
			float sin = Mathf.Sin(degrees*Mathf.Deg2Rad);
			float cos = Mathf.Cos(degrees*Mathf.Deg2Rad);
			float tx = v.x;
			float tz = v.z;
			v.x = (cos*tx)-(sin*tz);
			v.z = (sin*tx)+(cos*tz);
			return v;
		}
	}
	public class GrassFlowers : TileType
	{
		protected override string[] Variants => new[] {
			"GrassFlowers1.png",
			"GrassFlowers2.png",
			"GrassFlowers3.png"
		};
		public override PhysicMaterial GetMaterial(Vector3? atPoint = null) => PhysicMaterial.GetMaterial<GrassPhysicMaterial>();
	}
}
