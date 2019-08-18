using System;
using GameEngine.Graphics;

namespace GameEngine
{
	public abstract class Renderer : Component
	{
		public Func<bool?> PreCullingModifyResult { get; set; }
		public Func<bool,bool> PostCullingModifyResult { get; set; }
		
		internal MaterialCollection materials;
		public MaterialCollection Materials {
			get => materials;
			set {
				if(materials==value) {
					return;
				}

				if(materials!=null) {
					for(int i=0;i<materials.Count;i++) {
						materials[i]?.RendererDetach(this);
					}
				}

				if(value!=null) {
					for(int i=0;i<value.Count;i++) {
						value[i]?.RendererAttach(this);
					}
					value.renderer = this;
				}

				materials = value;
			}
		}
		public virtual Material Material {
			get {
				if(materials==null || materials.Count==0) {
					return null;
				}
				return materials[0];
			}
			set {
				if(materials==null || materials.Count==0) {
					Materials = new Material[1];
				}
				materials[0] = value;
			}
		}

		protected override void OnInit()
		{
			if(materials==null) {
				Materials = new Material[1];
				Material = Material.defaultMat;
			}
		}
		protected override void OnEnable() => Rendering.rendererList.Add(this);
		protected override void OnDisable() => Rendering.rendererList.Remove(this);
		protected override void OnDispose()
		{
			Rendering.rendererList.Remove(this);
			Materials = null;
		}

		public virtual void ApplyUniforms(Shader shader) {}

		protected virtual Mesh GetRenderMesh(Vector3 rendererPosition,Vector3 cameraPosition) => throw new NotImplementedException();

		internal Mesh GetRenderMeshInternal(Vector3 rendererPosition,Vector3 cameraPosition) => GetRenderMesh(rendererPosition,cameraPosition);
	}
}