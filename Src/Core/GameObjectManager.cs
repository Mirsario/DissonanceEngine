using System;
using System.Collections.Generic;
using Dissonance.Engine.Utilities;

namespace Dissonance.Engine
{
	public class GameObjectManager : EngineModule
	{
		internal static InstanceLists<GameObject> gameObjects;

		protected override void Init()
		{
			gameObjects = new InstanceLists<GameObject>();
		}

		//Instantiation
		public T Instantiate<T>(Action<T> preinitializer = null, bool enable = true) where T : GameObject
		{
			var obj = (T)Activator.CreateInstance(typeof(T), true);

			preinitializer?.Invoke(obj);

			if(enable) {
				obj.EnabledLocal = true;
			}

			return obj;
		}
		public GameObject Instantiate(Type type, Action<GameObject> preinitializer = null, bool enable = true)
		{
			AssertionUtils.TypeIsAssignableFrom(typeof(GameObject), type ?? throw new ArgumentNullException(nameof(type)));

			var obj = (GameObject)Activator.CreateInstance(type, true);

			preinitializer?.Invoke(obj);

			if(enable) {
				obj.EnabledLocal = true;
			}

			return obj;
		}
		//Enumeration
		public IEnumerable<GameObject> EnumerateGameObjects(bool? enabled = true)
		{
			if(gameObjects == null) {
				yield break;
			}

			lock(gameObjects) {
				foreach(var entry in GetInstanceList(enabled)) {
					yield return entry;
				}
			}
		}

		internal void ModifyInstanceLists(Action<InstanceLists<GameObject>> action)
		{
			lock(gameObjects) {
				action(gameObjects);
			}
		}

		private List<GameObject> GetInstanceList(bool? enabled) => enabled switch {
			true => gameObjects.enabled,
			false => gameObjects.disabled,
			_ => gameObjects.all
		};
	}
}
