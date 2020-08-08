using System;
using System.Linq;
using Dissonance.Engine.Core.ProgrammableEntities;

namespace Dissonance.Engine.Core.Components
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
					var parameters = ComponentManager.GetParameters(type);

					if(!parameters.allowOnlyOneInWorld || !ComponentManager.EnumerateComponents(type).Any(q => q.enabled)) {
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
		public Transform Transform => gameObject.Transform;

		protected Component() : base()
		{
			var type = GetType();

			Name = type.Name;
			NameHash = Name.GetHashCode();

			ComponentManager.RegisterInstance(type,this);
		}

		protected virtual void OnPreInit() { }
		protected virtual void OnInit() { }
		protected virtual void OnEnable() { }
		protected virtual void OnDisable() { }
		protected virtual void OnDispose() { }

		public void Dispose()
		{
			ProgrammableEntityManager.UnsubscribeEntity(this);

			OnDispose();

			ComponentManager.UnregisterInstance(GetType(),this);

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