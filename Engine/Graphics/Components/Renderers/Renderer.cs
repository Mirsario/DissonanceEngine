using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GameEngine
{
	public abstract class Renderer : Component
	{
		#region InstanceFields
		public Func<bool?> PreCullingModifyResult { get; set; }
		public Func<bool,bool> PostCullingModifyResult { get; set; }
		#endregion
		#region Properties
		internal MaterialCollection _materials;
		public MaterialCollection Materials {
			get => _materials;
			set {
				if(_materials==value) {
					return;
				}
				if(_materials!=null) {
					for(int i=0;i<_materials.Count;i++) {
						_materials[i]?.RendererDetach(this);
					}
				}
				if(value!=null) {
					for(int i=0;i<value.Count;i++) {
						value[i]?.RendererAttach(this);
					}
					value.renderer = this;
				}
				_materials = value;
			}
		}
		public virtual Material Material {
			get {
				if(Materials==null || Materials.Count==0) {
					return null;
				}
				return Materials[0];
			}
			set {
				if(Materials==null || Materials.Count==0) {
					Materials = new Material[1];
				}
				Materials[0] = value;
			}
		}
		#endregion

		protected override void OnEnable()
		{
			Graphics.rendererList.Add(this);
		}
		protected override void OnDisable()
		{
			Graphics.rendererList.Remove(this);
		}
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

		public virtual void ApplyUniforms(Shader shader) {}
		protected virtual Mesh GetRenderMesh(Vector3 rendererPosition,Vector3 cameraPosition) => throw new NotImplementedException();

		internal Mesh GetRenderMeshInternal(Vector3 rendererPosition,Vector3 cameraPosition) => GetRenderMesh(rendererPosition,cameraPosition);
	}
	public class MaterialCollection : ICollection,ICollection<Material>,IEnumerable,IEnumerable<Material>
	{
		private Material[] array;
		internal Renderer renderer;

		public int Count			=> array.Length;
		public object SyncRoot		=> array.SyncRoot;
		public bool IsReadOnly		=> array.IsSynchronized;
		public bool IsSynchronized	=> array.IsSynchronized;

		public Material this[int index] {
			get => array[index];
			set {
				if(renderer!=null) {
					var oldValue = array[index];
					if(oldValue==value) {
						return;
					}
					if(oldValue!=null) {
						oldValue.RendererDetach(renderer);
					}
					if(value!=null) {
						value.RendererAttach(renderer);
					}
				}
				array[index] = value;
			}
		}

		public MaterialCollection(Material[] array)
		{
			this.array = array;
		}

		public bool Contains(Material material)						=> array.Contains(material);
		public void Add(Material material)							=> throw new NotSupportedException();
		public bool Remove(Material material)						=> throw new NotSupportedException();
		public void Clear()											=> throw new NotSupportedException();
		public void CopyTo(Array arr,int index)						=> array.CopyTo(arr,index);
		public void CopyTo(Material[] arr,int index)				=> array.CopyTo(arr,index);
		public IEnumerator GetEnumerator()							=> array.GetEnumerator();
		IEnumerator<Material> IEnumerable<Material>.GetEnumerator()	=> (IEnumerator<Material>)array.GetEnumerator();

		public static implicit operator MaterialCollection(Material[] array)	=> new MaterialCollection(array);
	}
}