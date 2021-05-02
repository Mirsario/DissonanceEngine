using Dissonance.Engine.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dissonance.Engine
{
	partial class GameObject
	{
		internal List<IComponent> componentsInternal;

		public IReadOnlyList<IComponent> Components { get; private set; }
		
		public void SetComponent<T>(T component) where T : struct, IComponent
		{
			
		}
		
		public T GetComponent<T>() where T : struct, IComponent
		{
			TryGetComponent<T>(out var result);

			return result;
		}
		
		public void RemoveComponent<T>() where T : struct, IComponent
		{
			for(int i = 0; i < componentsInternal.Count; i++) {
				if(componentsInternal[i] is T component) {
					componentsInternal.RemoveAt(i--);
				}
			}
		}

		private void ComponentPreInit()
		{
			componentsInternal = new List<IComponent>();
			Components = componentsInternal.AsReadOnly();
		}

		private void ComponentClone(GameObject clone)
		{
			clone.componentsInternal = new List<IComponent>(componentsInternal);
			clone.Components = clone.componentsInternal.AsReadOnly();
		}

		private void ComponentDispose()
		{
			if(componentsInternal != null) {
				componentsInternal.Clear();

				componentsInternal = null;
			}
		}
	}
}
