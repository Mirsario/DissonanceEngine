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
		public T Instantiate<T>(Action<T> preinitializer = null) where T : GameObject
		{
			var obj = (T)Activator.CreateInstance(typeof(T),true);

			preinitializer?.Invoke(obj);

			obj.Init();

			return obj;
		}
		public GameObject Instantiate(Type type,Action<GameObject> preinitializer = null)
		{
			AssertionUtils.TypeIsAssignableFrom(typeof(GameObject),type ?? throw new ArgumentNullException(nameof(type)));

			var obj = (GameObject)Activator.CreateInstance(type,true);

			preinitializer?.Invoke(obj);

			obj.Init();

			return obj;
		}
		//Enumeration
		public static IEnumerable<GameObject> EnumerateGameObjects()
		{
			foreach(var entry in Instance.gameObjects) {
				yield return entry;
			}
		}
	}
}
