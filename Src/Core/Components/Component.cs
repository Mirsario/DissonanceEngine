using Dissonance.Engine.Core.ProgrammableEntities;
using System;

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
				if(enabled == value) {
					return;
				}

				if(value) {
					var type = GetType();
					var parameters = ComponentManager.GetParameters(type);

					if(parameters.allowOnlyOneInWorld && ComponentManager.CountComponents(type) >= 1) {
						throw new InvalidOperationException($"Attempted to enable a second instance of component '{GetType().Name}', but only 1 instance is allowed to be enabled at the same time.");
					}

					if(!beenEnabledBefore) {
						OnInit();

						beenEnabledBefore = true;
					} else {
						ComponentManager.ModifyInstanceLists(GetType(), lists => lists.disabled.Remove(this)); //Remove from the list of disabled components.
					}

					ComponentManager.ModifyInstanceLists(GetType(), lists => lists.enabled.Add(this)); //Add to the list of enabled components.

					OnEnable();

					ProgrammableEntityManager.SubscribeEntity(this);

					enabled = true;
				} else {
					//Remove from the list of enabled components, and add to the list of disabled ones.
					ComponentManager.ModifyInstanceLists(GetType(), lists => {
						lists.enabled.Remove(this);
						lists.disabled.Add(this);
					});

					OnDisable();

					ProgrammableEntityManager.UnsubscribeEntity(this);

					enabled = false;
				}
			}
		}

		public GameObject GameObject => gameObject;
		public Transform Transform => gameObject.Transform;
		public Transform2D Transform2D => gameObject.Transform2D;

		protected Component() : base()
		{
			var type = GetType();

			Name = type.Name;
			NameHash = Name.GetHashCode();

			ComponentManager.ModifyInstanceLists(GetType(), lists => lists.all.Add(this));
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

			ComponentManager.ModifyInstanceLists(GetType(), lists => {
				lists.all.Remove(this);

				if(enabled) {
					lists.enabled.Remove(this);
				} else {
					lists.disabled.Remove(this);
				}
			});
		}

		internal void PreInit() => OnPreInit();
	}
}
