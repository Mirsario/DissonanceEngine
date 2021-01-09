using Dissonance.Engine.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dissonance.Engine
{
	partial class GameObject
	{
		internal List<Component> components;

		private IReadOnlyList<Component> componentsReadOnly;

		//AddComponent
		public T AddComponent<T>(Action<T> initializer) where T : Component
			=> AddComponent(true, initializer);
		public T AddComponent<T>(bool enable, Action<T> initializer) where T : Component
		{
			T component = AddComponent<T>(false);

			initializer(component);

			if(enable) {
				component.EnabledLocal = true;
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
		public IReadOnlyList<Component> GetComponents() => componentsReadOnly;
		public IEnumerable<T> GetComponents<T>(bool exactType = false) where T : Component
		{
			var type = typeof(T);

			for(int i = 0; i < components.Count; i++) {
				var component = components[i];
				var thisType = component.GetType();

				if(thisType == type || (!exactType && thisType.IsSubclassOf(type))) {
					yield return (T)component;
				}
			}
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
		private void ComponentPreInit()
		{
			components = new List<Component>();
			componentsReadOnly = components.AsReadOnly();
		}
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
			//Call ComponentAttribute hooks.
			foreach(var componentAttribute in ComponentAttribute.EnumerateForType(type)) {
				componentAttribute.PreAddComponent(this, type);
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
			newComponent.EnabledInHierarchy = Enabled;

			newComponent.PreInit();

			if(enable) {
				newComponent.EnabledLocal = true;
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
