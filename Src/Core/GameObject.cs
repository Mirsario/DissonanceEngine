using System;
using Dissonance.Engine.Physics;

namespace Dissonance.Engine
{
	public partial class GameObject : IDisposable
	{
		private static GameObjectManager Manager => Game.Instance.GetModule<GameObjectManager>();

		internal RigidbodyInternal rigidbodyInternal;

		private string name;
		private byte layer;
		private bool enabled;
		private bool enabledLocal;
		private bool enabledInHierarchy = true;
		private bool initialized;

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
		}

		public virtual void OnInit() { }
		public virtual void OnDispose() { }
		public virtual GameObject Clone()
		{
			var clone = (GameObject)MemberwiseClone();

			ComponentClone(clone);

			return clone;
		}

		public void Dispose()
		{
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
		}
		private void OnDisabled()
		{
			Manager.ModifyInstanceLists(lists => {
				lists.disabled.Add(this);
				lists.enabled.Remove(this);
			});
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
