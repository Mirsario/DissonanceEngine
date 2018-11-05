/*using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using GameEngine.Graphics;

namespace GameEngine
{
	public class MaterialCollection : ICollection,ICollection<Material>,IEnumerable,IEnumerable<Material>
	{
		private readonly Material[] Array;
		internal Renderer renderer;

		public int Count => Array.Length;
		public object SyncRoot => Array.SyncRoot;
		public bool IsReadOnly => Array.IsSynchronized;
		public bool IsSynchronized => Array.IsSynchronized;

		public Material this[int index] {
			get => Array[index];
			set {
				if(renderer!=null) {
					var oldValue = Array[index];
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
				Array[index] = value;
			}
		}

		public MaterialCollection(Material[] array)
		{
			Array = array;
		}

		public bool Contains(Material material) => Array.Contains(material);
		public void Add(Material material) => throw new NotSupportedException();
		public bool Remove(Material material) => throw new NotSupportedException();
		public void Clear() => throw new NotSupportedException();
		public void CopyTo(Array arr,int index) => Array.CopyTo(arr,index);
		public void CopyTo(Material[] arr,int index) => Array.CopyTo(arr,index);
		public IEnumerator GetEnumerator() => Array.GetEnumerator();
		IEnumerator<Material> IEnumerable<Material>.GetEnumerator() => (IEnumerator<Material>)Array.GetEnumerator();

		public static implicit operator MaterialCollection(Material[] array) => new MaterialCollection(array);
	}
}*/
