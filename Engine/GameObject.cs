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
		
		#region Constructors
		protected GameObject(string name,Vector3 position,Quaternion rotation)
		{
			Name = name ?? GetType().Name;
			transform = new Transform(this);
			if(position!=default) {
				transform.Position = position;
			}
			if(rotation!=Quaternion.Identity) {
				transform.Rotation = rotation;
			}
			components = new Dictionary<string,List<Component>>();
			gameObjects.Add(this);
			OnInit(); //TODO: hhhhhhhh
		}
		protected GameObject(string name,Vector3 position,Vector3 rotation)
			: this(name,position,Quaternion.FromEuler(rotation)) {}
		protected GameObject(string name,Vector3 position)
			: this(name,position,Quaternion.Identity) {}
		protected GameObject(string name)
			: this(name,default,Quaternion.Identity) {}
		protected GameObject()
			: this(null,default,Quaternion.Identity) {}
		#endregion
		#region Instantiate
		public static T Instantiate<T>(string name,Vector3 position,Quaternion rotation) where T : GameObject
		{
			var obj = (T)FormatterServices.GetUninitializedObject(typeof(T));
			obj.InternalInitBegin();
			obj.Name = name ?? typeof(T).Name;
			obj.transform.Position = position;
			obj.transform.Rotation = rotation;
			obj.InternalInitEnd();
			return obj;
		}
		public static T Instantiate<T>(string name,Vector3 position,Vector3 eulerRot) where T : GameObject
		{
			var obj = (T)FormatterServices.GetUninitializedObject(typeof(T));
			obj.InternalInitBegin();
			obj.Name = name ?? typeof(T).Name;
			obj.transform.Position = position;
			obj.transform.EulerRot = eulerRot;
			obj.InternalInitEnd();
			return obj;
		}
		public static T Instantiate<T>(string name,Vector3 position) where T : GameObject
		{
			var obj = (T)FormatterServices.GetUninitializedObject(typeof(T));
			obj.InternalInitBegin();
			obj.Name = name ?? typeof(T).Name;
			obj.transform.Position = position;
			obj.InternalInitEnd();
			return obj;
		}
		public static T Instantiate<T>(string name) where T : GameObject
		{
			var obj = (T)FormatterServices.GetUninitializedObject(typeof(T));
			obj.InternalInitBegin();
			obj.Name = name ?? typeof(T).Name;
			obj.InternalInitEnd();
			return obj;
		}
		public static T Instantiate<T>() where T : GameObject
		{
			var obj = (T)FormatterServices.GetUninitializedObject(typeof(T));
			obj.InternalInitBegin();
			obj.InternalInitEnd();
			return obj;
		}
		public static T Instantiate<T>(Vector3 position,Quaternion rotation) where T : GameObject
		{
			var obj = (T)FormatterServices.GetUninitializedObject(typeof(T));
			obj.InternalInitBegin();
			obj.Name = typeof(T).Name;
			obj.transform.Position = position;
			obj.transform.Rotation = rotation;
			obj.InternalInitEnd();
			return obj;
		}
		public static T Instantiate<T>(Vector3 position,Vector3 eulerRot) where T : GameObject
		{
			var obj = (T)FormatterServices.GetUninitializedObject(typeof(T));
			obj.InternalInitBegin();
			obj.Name = typeof(T).Name;
			obj.transform.Position = position;
			obj.transform.EulerRot = eulerRot;
			obj.InternalInitEnd();
			return obj;
		}
		public static T Instantiate<T>(Vector3 position) where T : GameObject
		{
			var obj = (T)FormatterServices.GetUninitializedObject(typeof(T));
			obj.InternalInitBegin();
			obj.Name = typeof(T).Name;
			obj.transform.Position = position;
			obj.InternalInitEnd();
			return obj;
		}
		
		public static GameObject Instantiate(string name,Vector3 position,Quaternion rotation) => new GameObject(name,position,rotation);
		public static GameObject Instantiate(string name,Vector3 position,Vector3 rotation) => new GameObject(name,position,rotation);
		public static GameObject Instantiate(string name,Vector3 position) => new GameObject(name,position);
		public static GameObject Instantiate(string name) => new GameObject(name);
		public static GameObject Instantiate() => new GameObject();
		#endregion

		internal void InternalInitBegin()
		{
			transform = new Transform(this);
			components = new Dictionary<string,List<Component>>();
		}
		internal void InternalInitEnd()
		{
			gameObjects.Add(this);
			OnInit();
		}
		internal static void Init()
		{
			gameObjects = new List<GameObject>();
		}

		#region VirtualMethods
		public virtual void OnInit() {}
		public virtual void FixedUpdate() {}
		public virtual void RenderUpdate() {}
		public virtual void OnGUI() {}
		public virtual void OnDispose() {}
		#endregion
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
		public static GameObject[] GetGameObjects() => gameObjects.ToArray();
	}
}