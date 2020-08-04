using System;
using Dissonance.Engine.Core.ProgrammableEntities;
using Dissonance.Engine.Physics;
using Dissonance.Engine.Structures;

namespace Dissonance.Engine.Core
{
	public partial class GameObject : ProgrammableEntity, IDisposable
	{
		private static GameObjectManager Manager => Game.Instance.GetModule<GameObjectManager>();

		public byte layer;
		public string name;

		internal bool initialized;
		internal RigidbodyInternal rigidbodyInternal;
		internal Transform transform;

		public Transform Transform => transform;
		public string Name {
			get => name;
			set => name = value ?? throw new Exception("GameObject's name cannot be set to null");
		}

		protected GameObject() : base() { }

		public virtual void OnInit() { }
		public virtual void OnDispose() { }

		public void Init()
		{
			if(initialized) {
				return;
			}

			Manager.gameObjects.Add(this);

			ProgrammableEntityManager.SubscribeEntity(this);

			OnInit();

			initialized = true;
		}
		public void Dispose()
		{
			ProgrammableEntityManager.UnsubscribeEntity(this);

			OnDispose();
			ComponentDispose();

			Manager.gameObjects.Remove(this);
		}

		internal void PreInit()
		{
			Name = GetType().Name;
			transform = new Transform(this);

			ComponentPreInit();
		}

		public static T Instantiate<T>(string name = default,Vector3 position = default,Quaternion rotation = default,Vector3? scale = null,bool init = true) where T : GameObject
			=> Manager.Instantiate<T>(name,position,rotation,scale,init);
		public static GameObject Instantiate(Type type,string name = default,Vector3? position = default,Quaternion? rotation = default,Vector3? scale = null,bool init = true)
			=> Manager.Instantiate(type,name,position,rotation,scale,init);
	}
}