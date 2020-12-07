using System;
using System.Collections.Generic;
using System.Linq;
using Dissonance.Engine.Core.Components;
using Dissonance.Engine.Core.ProgrammableEntities;
using Dissonance.Engine.Physics;

namespace Dissonance.Engine.Core
{
	public partial class GameObject : ProgrammableEntity, IDisposable
	{
		private static GameObjectManager Manager => Game.Instance.GetModule<GameObjectManager>();

		internal bool initialized;
		internal RigidbodyInternal rigidbodyInternal;

		private string name;
		private byte layer;

		public Transform Transform { get; }
		public Transform2D Transform2D { get; }
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

		protected GameObject() : base()
		{
			Name = GetType().Name;

			ComponentPreInit();

			Transform = AddComponent<Transform>();
			Transform2D = AddComponent<Transform2D>();
		}

		public virtual void OnInit() { }
		public virtual void OnDispose() { }

		public void Init()
		{
			if(initialized) {
				return;
			}

			var manager = Manager;

			lock(manager.gameObjects) {
				manager.gameObjects.Add(this);
			}

			ProgrammableEntityManager.SubscribeEntity(this);

			OnInit();

			initialized = true;
		}
		public void Dispose()
		{
			ProgrammableEntityManager.UnsubscribeEntity(this);

			OnDispose();
			ComponentDispose();

			var manager = Manager;

			lock(manager.gameObjects) {
				manager.gameObjects.Remove(this);
			}
		}

		public static T Instantiate<T>(Action<T> preinitializer = null) where T : GameObject
			=> Manager.Instantiate(preinitializer);
		public static GameObject Instantiate(Type type, Action<GameObject> preinitializer = null)
			=> Manager.Instantiate(type, preinitializer);
	}
}
