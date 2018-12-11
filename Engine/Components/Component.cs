using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace GameEngine
{
	public class Component : IDisposable
	{
		internal static Dictionary<Type,ComponentParameters> typeParameters = new Dictionary<Type,ComponentParameters>();
		internal static Dictionary<Type,List<Component>> typeInstances = new Dictionary<Type,List<Component>>();

		protected bool beenEnabledBefore;
		protected bool enabled;
		public bool Enabled {
			get => enabled;
			set {
				if(enabled==value) {
					return;
				}
				if(value) {
					var type = GetType();
					var parameters = typeParameters[type];
					if(!parameters.allowOnlyOneInWorld || !typeInstances.TryGetValue(type,out var list) || !list.Any(q => q.enabled)) {
						if(!beenEnabledBefore) {
							OnInit();
							beenEnabledBefore = true;
						}
						OnEnable();
						enabled = true;
					}
				}else{
					OnDisable();
					enabled = false;
				}
			}
		}
		public string name;
		internal GameObject gameObject;
		public GameObject GameObject => gameObject;
		public Transform Transform => gameObject.transform;
		
		internal Component()
		{
			var type = GetType();
			name = type.Name;
			if(!typeInstances.ContainsKey(type)) {
				typeInstances[type] = new List<Component>();
			}
			typeInstances[type].Add(this);
		}
		internal static void Init()
		{
			foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
				foreach(var type in assembly.GetTypes()) {
					if(typeof(Component).IsAssignableFrom(type)) {
						if(!typeParameters.ContainsKey(type)) {
							typeParameters[type] = new ComponentParameters();
						}
						var attributes = type.GetCustomAttributes<ComponentAttribute>();
						foreach(var attribute in attributes) {
							attribute.SetParameters(type);
						}
					}
				}
			}
		}

		#region VirtualMethods
		protected virtual void OnInit() {}
		protected virtual void OnEnable() {}
		protected virtual void OnDisable() {}
		protected virtual void OnDispose() {}
		public virtual void FixedUpdate() {}
		public virtual void RenderUpdate() {}
		#endregion

		public void Dispose()
		{
			OnDispose();
			typeInstances[GetType()]?.Remove(this);
			if(gameObject.components.ContainsKey(name) && gameObject.components[name].Contains(this)) {
				gameObject.components[name].Remove(this);
				if(gameObject.components[name].Count==0) {
					gameObject.components.Remove(name);
				}
			}
		}
	}
}