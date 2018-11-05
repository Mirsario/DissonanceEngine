using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

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
		#region Properties
		internal Mesh _mesh;
		public virtual Mesh Mesh {
			get => _lodMeshes?[0]?.mesh;
			set {
				if(_lodMeshes!=null) {
					_lodMeshes[0].mesh = value;
				}else{
					LODMeshes = new[] { new MeshLOD(value) };
				}
			}
		}
		internal MeshLOD[] _lodMeshes;
		public virtual MeshLOD[] LODMeshes {
			get => _lodMeshes;
			set {
				if(value==null) {
					_lodMeshes = value;
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
					_lodMeshes = list.ToArray();
				}
			}
		}
		public virtual MeshLOD LODMesh {
			get => _lodMeshes?[0];
			set {
				if(_lodMeshes!=null) {
					_lodMeshes[0] = value;
				}else{
					LODMeshes = new[] { value };
				}
			}
		}
		#endregion

		protected override void OnDispose()
		{
			Graphics.rendererList.Remove(this);
			Materials = null;
		}
		protected override void OnInit()
		{
			Materials = new Material[1];
			Material = Material.defaultMat;
		}
		protected override Mesh GetRenderMesh(Vector3 rendererPosition,Vector3 cameraPosition)
		{
			MeshLOD lodTemp;
			var lods = LODMeshes;
			if(lods==null) {
				return null;
			}
			if(lods.Length==1) {
				return lods[0].mesh;
			}
			float sqrDist = Vector3.SqrDistance(cameraPosition,rendererPosition);
			for(int k=0;k<lods.Length;k++) {
				if(sqrDist<=(lodTemp = lods[k]).maxDistanceSqr || lodTemp.maxDistance==0f) {
					return lodTemp.mesh;
				}
			}
			return null;
		}
	}
}