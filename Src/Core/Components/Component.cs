using System;

namespace Dissonance.Engine
{
	public interface IComponent
	{
		virtual void EnsureInitialized() { }
	}

	/*public class Component : ProgrammableEntity, IDisposable
	{
		public readonly string Name;

		internal readonly int NameHash;

		internal GameObject gameObject;

		private bool initialized;
		private bool enabled;
		private bool enabledLocal;
		private bool enabledInHierarchy;

		public GameObject GameObject => gameObject;
		public Transform Transform => gameObject.Transform;
		public Transform2D Transform2D => gameObject.Transform2D;

		public bool Enabled {
			get => enabled;
			internal set {
				if(enabled == value) {
					return;
				}

				if(value) {
					enabled = true;

					OnEnableInternal();
				} else {
					enabled = false;

					OnDisableInternal();
				}
			}
		}
		public bool EnabledLocal {
			get => enabledLocal;
			set {
				enabledLocal = value;
				Enabled = enabledLocal && enabledInHierarchy;
			}
		}

		internal bool EnabledInHierarchy {
			get => enabledInHierarchy;
			set {
				enabledInHierarchy = value;
				Enabled = enabledLocal && enabledInHierarchy;
			}
		}

		protected Component() : base()
		{
			var type = GetType();

			Name = type.Name;
			NameHash = Name.GetHashCode();

			ComponentManager.ModifyInstanceLists(GetType(), lists => lists.all.Add(this));
		}

		public virtual Component Clone(GameObject newGameObject)
		{
			var clone = (Component)MemberwiseClone();

			clone.gameObject = newGameObject;

			return clone;
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

		private void OnEnableInternal()
		{
			//Call ComponentAttribute hooks.
			foreach(var componentAttribute in ComponentAttribute.EnumerateForType(GetType())) {
				componentAttribute.OnComponentEnabled(gameObject, this);
			}

			if(!initialized) {
				OnInit();

				initialized = true;
			} else {
				ComponentManager.ModifyInstanceLists(GetType(), lists => lists.disabled.Remove(this)); //Remove from the list of disabled components.
			}

			ComponentManager.ModifyInstanceLists(GetType(), lists => lists.enabled.Add(this)); //Add to the list of enabled components.

			OnEnable();

			ProgrammableEntityManager.SubscribeEntity(this);
		}
		private void OnDisableInternal()
		{
			//Call ComponentAttribute hooks.
			foreach(var componentAttribute in ComponentAttribute.EnumerateForType(GetType())) {
				componentAttribute.OnComponentDisabled(gameObject, this);
			}

			//Remove from the list of enabled components, and add to the list of disabled ones.
			ComponentManager.ModifyInstanceLists(GetType(), lists => {
				lists.enabled.Remove(this);
				lists.disabled.Add(this);
			});

			OnDisable();

			ProgrammableEntityManager.UnsubscribeEntity(this);
		}
	}*/
}
