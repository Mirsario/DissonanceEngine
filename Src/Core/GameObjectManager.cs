using System;
using System.Collections.Generic;
using Dissonance.Engine.Core.Modules;
using Dissonance.Engine.Structures;
using Dissonance.Engine.Utils.Internal;

namespace Dissonance.Engine.Core
{
	public class GameObjectManager : EngineModule
	{
		private static GameObjectManager Instance => Game.Instance.GetModule<GameObjectManager>();

		internal List<GameObject> gameObjects;

		protected override void Init()
		{
			gameObjects = new List<GameObject>();
		}

		//Instantiation
		public T Instantiate<T>(string name = default,Vector3 position = default,Quaternion rotation = default,Vector3? scale = null,bool init = true) where T : GameObject
			=> (T)Instantiate(typeof(T),name,position,rotation,scale,init);
		public GameObject Instantiate(Type type,string name = default,Vector3? position = default,Quaternion? rotation = default,Vector3? scale = null,bool init = true)
		{
			if(type==null) {
				throw new ArgumentNullException(nameof(type));
			}

			AssertionUtils.TypeIsAssignableFrom(typeof(GameObject),type);

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
		
		public static IEnumerable<GameObject> EnumerateGameObjects()
		{
			foreach(var entry in Instance.gameObjects) {
				yield return entry;
			}
		}
	}
}
