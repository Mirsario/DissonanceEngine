using System;
using System.Linq;
using GameEngine.Graphics;

namespace GameEngine
{
	public class MeshLOD
	{
		public Mesh mesh;
		public float maxDistance;
		internal float maxDistanceSqr;
		
		public MeshLOD(Mesh mesh)
		{
			this.mesh = mesh;
		}
		public MeshLOD(Mesh mesh,float maxDistance)
		{
			this.mesh = mesh;
			this.maxDistance = maxDistance;
			maxDistanceSqr = maxDistance*maxDistance;
		}
	}
	public class MeshRenderer : Renderer
	{
		private Mesh cachedRenderMesh;
		
		public virtual Mesh Mesh {
			get => lodMeshes?[0]?.mesh;
			set {
				if(lodMeshes!=null) {
					lodMeshes[0].mesh = value;
				}else{
					LODMeshes = new[] { new MeshLOD(value) };
				}
			}
		}
		internal MeshLOD[] lodMeshes;
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
					if(value.Any(l1 => l1==null ? hadNull = true : value.Count(l2 => l1.maxDistance==l2.maxDistance)>1)) {//dumb check
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

		protected override void OnDispose()
		{
			base.OnDispose();

			cachedRenderMesh = null;
		}

		protected override Mesh GetRenderMesh(Vector3 rendererPosition,Vector3 cameraPosition)
		{
			if(cachedRenderMesh!=null && Time.renderUpdateCount%60!=0) {
				return cachedRenderMesh;
			}

			var lods = LODMeshes;
			if(lods==null) {
				return null;
			}

			if(lods.Length==1) {
				return cachedRenderMesh = lods[0].mesh;
			}

			float sqrDist = Vector3.SqrDistance(cameraPosition,rendererPosition);
			for(int k=0;k<lods.Length;k++) {
				MeshLOD lodTemp = lods[k];
				if(sqrDist<=lodTemp.maxDistanceSqr || lodTemp.maxDistance==0f) {
					return cachedRenderMesh = lodTemp.mesh;
				}
			}

			return null;
		}
	}
}