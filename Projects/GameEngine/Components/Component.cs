using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace GameEngine
{
	public class Component : ProgrammableEntity, IDisposable
	{
		internal static Dictionary<Type,ComponentParameters> typeParameters = new Dictionary<Type,ComponentParameters>();
		internal static Dictionary<Type,List<Component>> typeInstances = new Dictionary<Type,List<Component>>();
		
		public readonly string Name;
		internal readonly int NameHash;
		internal GameObject gameObject;
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
						ProgrammableEntityHooks.SubscribeEntity(this);

						enabled = true;
					}
				}else{
					OnDisable();
					ProgrammableEntityHooks.UnsubscribeEntity(this);

					enabled = false;
				}
			}
		}

		public GameObject GameObject => gameObject;
		public Transform Transform => gameObject.transform;
		
		internal Component()
		{
			var type = GetType();
			Name = type.Name;
			NameHash = Name.GetHashCode();

			if(!typeInstances.TryGetValue(type,out var list)) {
				typeInstances[type] = list = new List<Component>();
			}
			list.Add(this);
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
		#endregion

		public void Dispose()
		{
			ProgrammableEntityHooks.UnsubscribeEntity(this);

			OnDispose();

			typeInstances[GetType()]?.Remove(this);

			var dict = gameObject.componentsByNameHash;
			if(dict.TryGetValue(NameHash,out var list)) {
				list.Remove(this);
				if(list.Count==0) {
					dict.Remove(NameHash);
				}
			}
		}
	}
}