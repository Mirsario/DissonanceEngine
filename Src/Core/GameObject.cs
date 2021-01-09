using System;
using System.Collections.Generic;
using System.Linq;
using Dissonance.Engine.Physics;

namespace Dissonance.Engine
{
	public partial class GameObject : ProgrammableEntity, IDisposable
	{
		private static GameObjectManager Manager => Game.Instance.GetModule<GameObjectManager>();

		internal RigidbodyInternal rigidbodyInternal;

		private string name;
		private byte layer;
		private bool enabled;
		private bool enabledLocal;
		private bool enabledInHierarchy = true;
		private bool initialized;

		public Transform Transform { get; private set; }
		public Transform2D Transform2D { get; private set; }

		public bool Enabled {
			get => enabled;
			private set {
				if(enabled == value) {
					return;
				}

				if(value) {
					enabled = true;

					OnEnabled();
				} else {
					enabled = false;

					OnDisabled();
				}

				//Enable/disable components
				var components = GetComponents();

				foreach(var component in components) {
					component.EnabledInHierarchy = value;
				}

				//Enable/disable children
				foreach(var child in Transform.Children) {
					child.GameObject.EnabledInHierarchy = value;
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
		public string Name {
			get => name;
			set => name = value ?? throw new Exception("GameObject's name cannot be set to null");
		}
		public byte Layer {
			get => layer;
			set {
				if(value >= Layers.MaxLayers) {
					throw new IndexOutOfRangeException($"Layer values must be in [0..{Layers.MaxLayers - 1}] range.");
				}

				layer = value;
			}
		}

		internal bool EnabledInHierarchy {
			get => enabledInHierarchy;
			set {
				enabledInHierarchy = value;
				Enabled = enabledLocal && enabledInHierarchy;
			}
		}

		protected GameObject() : base()
		{
			Name = GetType().Name;

			ComponentPreInit();

			Transform = AddComponent<Transform>();
			Transform2D = AddComponent<Transform2D>();
		}

		public virtual void OnInit() { }
		public virtual void OnDispose() { }
		public virtual GameObject Clone()
		{
			var clone = (GameObject)MemberwiseClone();

			clone.components = new List<Component>(components.Select(c => c.Clone(clone)));
			clone.componentsReadOnly = clone.components.AsReadOnly();
			clone.Transform = clone.GetComponent<Transform>();
			clone.Transform2D = clone.GetComponent<Transform2D>();

			return clone;
		}

		public void Dispose()
		{
			ProgrammableEntityManager.UnsubscribeEntity(this);

			OnDispose();
			ComponentDispose();

			Manager.ModifyInstanceLists(lists => {
				lists.all.Remove(this);

				if(Enabled) {
					lists.enabled.Remove(this);
				} else {
					lists.disabled.Remove(this);
				}
			});
		}

		private void OnEnabled()
		{
			if(!initialized) {
				Init();
			}

			Manager.ModifyInstanceLists(lists => {
				lists.enabled.Add(this);
				lists.disabled.Remove(this);
			});

			ProgrammableEntityManager.SubscribeEntity(this);
		}
		private void OnDisabled()
		{
			Manager.ModifyInstanceLists(lists => {
				lists.disabled.Add(this);
				lists.enabled.Remove(this);
			});

			ProgrammableEntityManager.UnsubscribeEntity(this);
		}
		private void Init()
		{
			Manager.ModifyInstanceLists(lists => lists.all.Add(this));

			OnInit();

			initialized = true;
		}

		public static T Instantiate<T>(Action<T> preinitializer = null, bool enable = true) where T : GameObject
			=> Manager.Instantiate(preinitializer, enable);
		public static GameObject Instantiate(Type type, Action<GameObject> preinitializer = null, bool enable = true)
			=> Manager.Instantiate(type, preinitializer, enable);
	}
}
