using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Linq;

namespace GameEngine
{
	public class GameObject : IDisposable
	{
		public string _name;
		public string Name {
			get => _name;
			set => _name = value ?? throw new Exception("GameObject's name cannot be set to null");
		}
		internal Transform transform;
		public Transform Transform => transform;

		public byte layer;
		internal static List<GameObject> gameObjects;
		internal Dictionary<string,List<Component>> components;
		internal RigidbodyInternal rigidbodyInternal;
		internal bool initialized;

		internal static void StaticInit()
		{
			gameObjects = new List<GameObject>();
		}

		public virtual void OnInit() {}
		public virtual void FixedUpdate() {}
		public virtual void RenderUpdate() {}
		public virtual void OnGUI() {}
		public virtual void OnDispose() {}
		
		internal void PreInit()
		{
			Name = GetType().Name;
			transform = new Transform(this);
			components = new Dictionary<string,List<Component>>();
		}
		public void Init()
		{
			if(initialized) {
				return;
			}
			gameObjects.Add(this);
			OnInit();
			initialized = true;
		}
		public void Dispose()
		{
			OnDispose();
			var compList = components.Values.ToArray();
			for(int i=0;i<compList.Length;i++) {
				for(int j=0;j<compList[i].Count;j++) {
					compList[i][j].Dispose();
				}
				compList[i].Clear();
				compList[i] = null;
			}
			components.Clear();
			if(gameObjects.Contains(this)) {
				gameObjects.Remove(this);
			}
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
			string key = newComponent.name;
			if(!components.ContainsKey(key)) {
				components[key] = new List<Component>();
			}
			components[key].Add(newComponent);
			newComponent.gameObject = this;
			if(enable) {
				newComponent.Enabled = true;
			}
			
			
			return (T)newComponent;
		}
		public T GetComponent<T>(bool noSubclasses = false) where T : Component
		{
			var type = typeof(T);
			foreach(var pair in components) {
				for(int i=0;i<pair.Value.Count;i++) {
					var thisType = pair.Value[i].GetType();
					if(thisType==type || !noSubclasses && thisType.IsSubclassOf(type)) {
						return (T)pair.Value[i];
					}
				}
			}
			return null;
		}
		public T[] GetComponents<T>(bool noSubclasses = false) where T : Component
		{
			var list = new List<T>();
			var type = typeof(T);
			foreach(var pair in components) {
				for(int i=0;i<pair.Value.Count;i++) {
					var thisType = pair.Value[i].GetType();
					if(thisType==type || !noSubclasses && thisType.IsSubclassOf(type)) {
						list.Add((T)pair.Value[i]);
					}
				}
			}
			return list.ToArray();
		}
		public int CountComponents<T>(bool noSubclasses = false) where T : Component
		{
			int count = 0;
			var type = typeof(T);
			foreach(var pair in components) {
				for(int i=0;i<pair.Value.Count;i++) {
					var thisType = pair.Value[i].GetType();
					if(thisType==type || !noSubclasses && thisType.IsSubclassOf(type)) {
						count++;
					}
				}
			}
			return count;
		}

		#region Instantiate
		public static T Instantiate<T>(string name = default,Vector3 position = default,Quaternion rotation = default,bool init = true) where T : GameObject
			=> (T)Instantiate(typeof(T),name,position,rotation,init);

		public static GameObject Instantiate(Type type,string name = default,Vector3 position = default,Quaternion rotation = default,bool init = true)
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
			if(init) {
				obj.Init();
			}
			return obj;
		}
		#endregion

		public static IEnumerable<GameObject> GetGameObjects() => gameObjects;
	}
}