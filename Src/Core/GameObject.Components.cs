using System;
using System.Collections.Generic;
using System.Reflection;
using Dissonance.Engine.Core.Components;
using Dissonance.Engine.Core.ProgrammableEntities;
using Dissonance.Engine.Utils.Internal;

namespace Dissonance.Engine.Core
{
	//TODO: 
	public partial class GameObject : ProgrammableEntity, IDisposable
	{
		internal List<Component> components;
		internal Dictionary<int,List<Component>> componentsByNameHash;

		//AddComponent
		public T AddComponent<T>(Action<T> initializer) where T : Component
			=> AddComponent(true,initializer);
		public T AddComponent<T>(bool enable,Action<T> initializer) where T : Component
		{
			T component = AddComponent<T>(false);

			initializer(component);

			if(enable) {
				component.Enabled = true;
			}

			return component;
		}
		public T AddComponent<T>(bool enable = true) where T : Component
			=> (T)AddComponentInternal(typeof(T),enable);
		public Component AddComponent(Type type,bool enable = true)
		{
			AssertionUtils.TypeIsAssignableFrom(typeof(Component),type);

			return AddComponentInternal(type,enable);
		}
		//GetComponent(s)
		public T GetComponent<T>(bool noSubclasses = false) where T : Component
		{
			var type = typeof(T);

			for(int i = 0;i<components.Count;i++) {
				var component = components[i];
				var thisType = component.GetType();

				if(thisType==type || !noSubclasses && thisType.IsSubclassOf(type)) {
					return (T)component;
				}
			}

			return null;
		}
		public IEnumerable<T> GetComponents<T>(bool noSubclasses = false) where T : Component
		{
			var list = new List<T>();
			var type = typeof(T);

			for(int i = 0;i<components.Count;i++) {
				var component = components[i];
				var thisType = component.GetType();

				if(thisType==type || !noSubclasses && thisType.IsSubclassOf(type)) {
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

			for(int i = 0;i<components.Count;i++) {
				var component = components[i];
				var thisType = component.GetType();

				if(thisType==type || !exactType && thisType.IsSubclassOf(type)) {
					count++;
				}
			}

			return count;
		}
		public int CountComponents(Type type,bool exactType = false)
		{
			AssertionUtils.TypeIsAssignableFrom(typeof(Component),type);

			return CountComponentsInternal(type,exactType);
		}

		//Init/Dispose fields.
		private void ComponentPreInit()
		{
			components = new List<Component>();
			componentsByNameHash = new Dictionary<int,List<Component>>();
		}
		private void ComponentDispose()
		{
			if(components!=null) {
				for(int i = 0;i<components.Count;i++) {
					components[i].Dispose();
				}

				components.Clear();

				components = null;
			}

			if(componentsByNameHash!=null) {
				componentsByNameHash.Clear();

				componentsByNameHash = null;
			}
		}
		//Etc
		private Component AddComponentInternal(Type type,bool enable = true)
		{
			var parameters = ComponentManager.GetParameters(type);

			if(parameters.allowOnlyOnePerObject && CountComponents(type)>=1) {
				throw new Exception($"Cannot add more than 1 component of type '{type.Name}' to a single {nameof(GameObject)}.");
			}

			Component newComponent;

			try {
				newComponent = (Component)Activator.CreateInstance(type);
			}
			catch(TargetInvocationException e) {
				throw e.InnerException;
			}

			int key = newComponent.NameHash;

			if(!componentsByNameHash.TryGetValue(key,out var componentList)) {
				componentsByNameHash[key] = componentList = new List<Component>();
			}

			componentList.Add(newComponent);
			components.Add(newComponent);

			newComponent.gameObject = this;

			newComponent.PreInit();

			if(enable) {
				newComponent.Enabled = true;
			}

			return newComponent;
		}
		private int CountComponentsInternal(Type type,bool exactType = false)
		{
			int count = 0;

			for(int i = 0;i<components.Count;i++) {
				var component = components[i];
				var thisType = component.GetType();

				if(thisType==type || !exactType && thisType.IsSubclassOf(type)) {
					count++;
				}
			}

			return count;
		}
	}
}