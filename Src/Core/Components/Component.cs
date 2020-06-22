using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Dissonance.Engine.Core.ProgrammableEntities;
using Dissonance.Engine.Core.Components;

namespace Dissonance.Engine
{
	public class Component : ProgrammableEntity, IDisposable
	{
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
					var parameters = ComponentManager.typeParameters[type];

					if(!parameters.allowOnlyOneInWorld || !ComponentManager.typeInstances.TryGetValue(type,out var list) || !list.Any(q => q.enabled)) {
						if(!beenEnabledBefore) {
							OnInit();

							beenEnabledBefore = true;
						}

						OnEnable();

						ProgrammableEntityManager.SubscribeEntity(this);

						enabled = true;
					}
				} else {
					OnDisable();

					ProgrammableEntityManager.UnsubscribeEntity(this);

					enabled = false;
				}
			}
		}

		public GameObject GameObject => gameObject;
		public Transform Transform => gameObject.transform;
		
		protected Component() : base()
		{
			var type = GetType();
			Name = type.Name;
			NameHash = Name.GetHashCode();

			if(!ComponentManager.typeInstances.TryGetValue(type,out var list)) {
				ComponentManager.typeInstances[type] = list = new List<Component>();
			}

			list.Add(this);
		}

		protected virtual void OnPreInit() {}
		protected virtual void OnInit() { }
		protected virtual void OnEnable() {}
		protected virtual void OnDisable() {}
		protected virtual void OnDispose() {}

		public void Dispose()
		{
			ProgrammableEntityManager.UnsubscribeEntity(this);

			OnDispose();

			ComponentManager.typeInstances[GetType()]?.Remove(this);

			var dict = gameObject.componentsByNameHash;

			if(dict.TryGetValue(NameHash,out var list)) {
				list.Remove(this);

				if(list.Count==0) {
					dict.Remove(NameHash);
				}
			}
		}

		internal void PreInit() => OnPreInit();
	}
}