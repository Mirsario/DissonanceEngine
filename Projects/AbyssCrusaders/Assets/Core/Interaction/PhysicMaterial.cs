using System;
using System.Collections.Generic;
using GameEngine;

namespace AbyssCrusaders
{
	public abstract class PhysicMaterial : IFootstepProvider
	{
		private static Dictionary<Type,PhysicMaterial> materials = new Dictionary<Type,PhysicMaterial>();

		public PhysicMaterial()
		{
			OnInit();
		}

		public abstract void GetFootstepInfo(Vector2 stepPosition,out string surfaceType,ref string actionType,out int numSoundVariants);

		public virtual void OnInit() { }
		public virtual string GetSound(string type) => null;

		public bool TryGetSound(string type,out string sound) => (sound = GetSound(type))!=null;

		public static PhysicMaterial GetMaterial<T>() where T : PhysicMaterial,new()
		{
			var type = typeof(T);

			if(materials.TryGetValue(type,out var material)) {
				return material;
			}

			return materials[type] = new T();
		}
		public static PhysicMaterial GetMaterial(Type type)
		{
			if(!typeof(PhysicMaterial).IsAssignableFrom(type)) {
				throw new ArgumentException("Type is not a PhysicMaterial",nameof(type));
			}

			if(!materials.TryGetValue(type,out var material)) {
				materials[type] = material = (PhysicMaterial)Activator.CreateInstance(type);
			}

			return material;
		}
	}
}
