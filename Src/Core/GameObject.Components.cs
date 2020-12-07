using Dissonance.Engine.Core.Components;
using Dissonance.Engine.Utils.Internal;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dissonance.Engine.Core
{
	partial class GameObject
	{
		internal List<Component> components;

		//AddComponent
		public T AddComponent<T>(Action<T> initializer) where T : Component
			=> AddComponent(true, initializer);
		public T AddComponent<T>(bool enable, Action<T> initializer) where T : Component
		{
			T component = AddComponent<T>(false);

			initializer(component);

			if(enable) {
				component.Enabled = true;
			}

			return component;
		}
		public T AddComponent<T>(bool enable = true) where T : Component
			=> (T)AddComponentInternal(typeof(T), enable);
		public Component AddComponent(Type type, bool enable = true)
		{
			AssertionUtils.TypeIsAssignableFrom(typeof(Component), type);

			return AddComponentInternal(type, enable);
		}
		//(Try)GetComponent(s)
		public T GetComponent<T>(bool exactType = false) where T : Component
		{
			TryGetComponent<T>(out var result, exactType);

			return result;
		}
		public bool TryGetComponent<T>(out T result, bool exactType = false) where T : Component
		{
			var type = typeof(T);

			for(int i = 0; i < components.Count; i++) {
				var component = components[i];
				var thisType = component.GetType();

				if(thisType == type || (!exactType && thisType.IsSubclassOf(type))) {
					result = (T)component;

					return true;
				}
			}

			result = default;

			return false;
		}
		public IEnumerable<T> GetComponents<T>(bool exactType = false) where T : Component
		{
			var list = new List<T>();
			var type = typeof(T);

			for(int i = 0; i < components.Count; i++) {
				var component = components[i];
				var thisType = component.GetType();

				if(thisType == type || (!exactType && thisType.IsSubclassOf(type))) {
					list.Add((T)component);
				}
			}

			return list;
		}
		//CountComponents
		public int CountComponents<T>(bool exactType = false) where T : Component
		{
			int count = 0;
			var type = typeof(T);

			for(int i = 0; i < components.Count; i++) {
				var component = components[i];
				var thisType = component.GetType();

				if(thisType == type || !exactType && thisType.IsSubclassOf(type)) {
					count++;
				}
			}

			return count;
		}
		public int CountComponents(Type type, bool exactType = false)
		{
			AssertionUtils.TypeIsAssignableFrom(typeof(Component), type);

			return CountComponentsInternal(type, exactType);
		}

		//Init/Dispose fields.
		private void ComponentPreInit() => components = new List<Component>();
		private void ComponentDispose()
		{
			if(components != null) {
				for(int i = 0; i < components.Count; i++) {
					components[i].Dispose();
				}

				components.Clear();

				components = null;
			}
		}
		//Etc
		private Component AddComponentInternal(Type type, bool enable = true)
		{
			var parameters = ComponentManager.GetParameters(type);

			if(parameters.allowOnlyOnePerObject && CountComponents(type) >= 1) {
				throw new Exception($"Cannot add more than 1 component of type '{type.Name}' to a single {nameof(GameObject)}.");
			}

			Component newComponent;

			try {
				newComponent = (Component)Activator.CreateInstance(type);
			}
			catch(TargetInvocationException e) {
				throw e.InnerException;
			}

			components.Add(newComponent);

			newComponent.gameObject = this;

			newComponent.PreInit();

			if(enable) {
				newComponent.Enabled = true;
			}

			return newComponent;
		}
		private int CountComponentsInternal(Type type, bool exactType = false)
		{
			int count = 0;

			for(int i = 0; i < components.Count; i++) {
				var component = components[i];
				var thisType = component.GetType();

				if(thisType == type || !exactType && thisType.IsSubclassOf(type)) {
					count++;
				}
			}

			return count;
		}
	}
}
