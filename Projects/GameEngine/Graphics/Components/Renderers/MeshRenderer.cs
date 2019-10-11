using GameEngine.Graphics;
using System;
using System.Linq;

namespace GameEngine
{
	public class MeshRenderer : Renderer
	{
		internal MeshLOD[] lodMeshes = new MeshLOD[1] { new MeshLOD(null,null) };

		public virtual MeshLOD[] LODMeshes {
			get => lodMeshes;
			set {
				if(value==null) {
					lodMeshes = value;
				}else{
					bool hadNull = false;

					if(value.Length==0) {
						throw new Exception("Value cannot be an empty array. Set it to null instead.");
					}

					if(value.Any(l1 => l1==null ? hadNull = true : value.Count(l2 => l1.maxDistance==l2.maxDistance)>1)) { //TODO: dumb check
						throw new Exception(hadNull ? "Array cannot contain null values" : "All maxDistance values must be unique.");
					}

					var list = value.OrderBy(q => q.maxDistance).ToList();
					if(list[0].maxDistance==0f) {
						var val = list[0];
						list.RemoveAt(0);
						list.Add(val);
					}

					lodMeshes = list.ToArray();
				}
			}
		}
		public virtual MeshLOD LODMesh {
			get => lodMeshes?[0];
			set {
				if(lodMeshes!=null) {
					lodMeshes[0] = value;
				}else{
					LODMeshes = new[] { value };
				}
			}
		}
		public virtual Mesh Mesh {
			get => lodMeshes[0].mesh;
			set => lodMeshes[0].mesh = value;
		}

		public override Material Material {
			get => lodMeshes[0].material;
			set => lodMeshes[0].material = value;
		}

		protected override bool GetRenderData(Vector3 rendererPosition,Vector3 cameraPosition,out Mesh mesh,out Material material)
		{
			var lods = LODMeshes;
			if(lods!=null) {
				if(lods.Length==1) {
					var lod = lods[0];

					mesh = lod.mesh;
					material = lod.material;

					return true;
				}

				float sqrDist = Vector3.SqrDistance(cameraPosition,rendererPosition);
				for(int i = 0;i<lods.Length;i++) {
					var lod = lods[i];

					if(sqrDist<=lod.maxDistanceSqr || lod.maxDistance==0f) {
						mesh = lod.mesh;
						material = lod.material;

						return true;
					}
				}
			}

			mesh = null;
			material = null;

			return false;
		}
	}
}