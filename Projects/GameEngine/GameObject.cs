using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace GameEngine
{
	public class GameObject : ProgrammableEntity, IDisposable
	{
		internal static List<GameObject> gameObjects;

		internal List<Component> components;
		internal Dictionary<int,List<Component>> componentsByNameHash;
		internal RigidbodyInternal rigidbodyInternal;
		internal bool initialized;
		public byte layer;
		
		public string name;
		public string Name {
			get => name;
			set => name = value ?? throw new Exception("GameObject's name cannot be set to null");
		}
		internal Transform transform;
		public Transform Transform => transform;

		internal static void StaticInit()
		{
			gameObjects = new List<GameObject>();
		}

		public virtual void OnInit() {}
		public virtual void OnDispose() {}
		
		internal void PreInit()
		{
			Name = GetType().Name;
			transform = new Transform(this);
			components = new List<Component>();
			componentsByNameHash = new Dictionary<int,List<Component>>();
		}
		public void Init()
		{
			if(initialized) {
				return;
			}
			gameObjects.Add(this);
			ProgrammableEntityHooks.SubscribeEntity(this);
			OnInit();
			initialized = true;
		}
		public void Dispose()
		{
			ProgrammableEntityHooks.UnsubscribeEntity(this);

			OnDispose();

			for(int i = 0;i<components.Count;i++) {
				components[i].Dispose();
			}
			components.Clear();
			components = null;

			componentsByNameHash.Clear();
			componentsByNameHash = null;

			gameObjects.Remove(this);
		}

		public T AddComponent<T>(bool enable = true) where T : Component
		{
			var componentType = typeof(T);
			var parameters = Component.typeParameters[componentType];	
			if(parameters.allowOnlyOnePerObject) {
				if(CountComponents<T>()>=1) {
					throw new Exception("You can't add more than 1 component of type "+componentType.Name+" to a single gameobject");
				}
			}
			Component newComponent;	
			try {
				newComponent = (Component)Activator.CreateInstance(typeof(T));
			}
			catch(TargetInvocationException e) {
				Debug.Log(e.InnerException);
				return null;
			}
			int key = newComponent.NameHash;
			if(!componentsByNameHash.TryGetValue(key,out var componentList)) {
				componentsByNameHash[key] = componentList = new List<Component>();
			}
			componentList.Add(newComponent);
			components.Add(newComponent);

			newComponent.gameObject = this;
			if(enable) {
				newComponent.Enabled = true;
			}
			
			
			return (T)newComponent;
		}
		public T GetComponent<T>(bool noSubclasses = false) where T : Component
		{
			var type = typeof(T);
			for(int i=0;i<components.Count;i++) {
				var component = components[i];
				var thisType = component.GetType();
				if(thisType==type || !noSubclasses && thisType.IsSubclassOf(type)) {
					return (T)component;
				}
			}
			return null;
		}
		public IEnumerable<T> GetComponents<T>(bool noSubclasses = false) where T : Component
		{
			var list = new List<T>();
			var type = typeof(T);
			for(int i=0;i<components.Count;i++) {
				var component = components[i];
				var thisType = component.GetType();
				if(thisType==type || !noSubclasses && thisType.IsSubclassOf(type)) {
					list.Add((T)component);
				}
			}
			return list;
		}
		public int CountComponents<T>(bool noSubclasses = false) where T : Component
		{
			int count = 0;
			var type = typeof(T);
			for(int i=0;i<components.Count;i++) {
				var component = components[i];
				var thisType = component.GetType();
				if(thisType==type || !noSubclasses && thisType.IsSubclassOf(type)) {
					count++;
				}
			}
			return count;
		}

		#region Instantiate
		public static T Instantiate<T>(string name = default,Vector3 position = default,Quaternion rotation = default,Vector3? scale = null,bool init = true) where T : GameObject => (T)Instantiate(typeof(T),name,position,rotation,scale,init);

		public static GameObject Instantiate(Type type,string name = default,Vector3 position = default,Quaternion rotation = default,Vector3? scale = null,bool init = true)
		{
			if(!typeof(GameObject).IsAssignableFrom(type)) {
				throw new ArgumentException("'type' must derive from GameObject class.");
			}
			var obj = (GameObject)FormatterServices.GetUninitializedObject(type);
			obj.PreInit();
			if(name!=default) {
				obj.Name = name;
			}
			if(position!=default) {
				obj.transform.Position = position;
			}
			if(rotation!=default) {
				obj.transform.Rotation = rotation;
			}
			if(scale.HasValue) {
				obj.transform.LocalScale = scale.Value;
			}
			if(init) {
				obj.Init();
			}
			return obj;
		}
		#endregion

		public static IEnumerable<GameObject> GetGameObjects() => gameObjects;
	}
}