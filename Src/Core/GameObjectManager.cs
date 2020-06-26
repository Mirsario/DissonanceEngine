using System;
using System.Collections.Generic;
using System.Text;
using Dissonance.Engine.Core.Modules;
using Dissonance.Engine.Structures;

namespace Dissonance.Engine.Core
{
	public class GameObjectManager : EngineModule
	{
		internal List<GameObject> gameObjects;

		protected override void Init()
		{
			gameObjects = new List<GameObject>();
		}

		public T Instantiate<T>(string name = default,Vector3 position = default,Quaternion rotation = default,Vector3? scale = null,bool init = true) where T : GameObject
			=> (T)Instantiate(typeof(T),name,position,rotation,scale,init);
		public GameObject Instantiate(Type type,string name = default,Vector3? position = default,Quaternion? rotation = default,Vector3? scale = null,bool init = true)
		{
			if(type==null) {
				throw new ArgumentNullException(nameof(type));
			}

			if(!typeof(GameObject).IsAssignableFrom(type)) {
				throw new ArgumentException($"Type '{type.Name}' does not derive from '{nameof(GameObject)}'.");
			}

			var obj = (GameObject)Activator.CreateInstance(type,true);

			obj.PreInit();

			if(name!=default) {
				obj.Name = name;
			}

			if(position.HasValue) {
				obj.transform.Position = position.Value;
			}

			if(rotation.HasValue) {
				obj.transform.Rotation = rotation.Value;
			}

			if(scale.HasValue) {
				obj.transform.LocalScale = scale.Value;
			}

			if(init) {
				obj.Init();
			}

			return obj;
		}
	}
}
