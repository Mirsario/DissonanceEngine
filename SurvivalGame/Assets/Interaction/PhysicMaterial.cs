using System;
using System.Collections.Generic;
using System.Linq;
using GameEngine;

namespace Game
{
	public interface IHasMaterial
	{
		PhysicMaterial GetMaterial(Vector3? atPoint = null);
	}

	public class PhysicMaterial : IFootstepProvider
	{
		private static Dictionary<Type,PhysicMaterial> materials = new Dictionary<Type,PhysicMaterial>();

		public PhysicMaterial()
		{
			OnInit();
		}
		public virtual void OnInit() { }

		public virtual void GetFootstepInfo(Vector3 stepPosition,out string surfaceType,ref string actionType,out int numSoundVariants)
		{
			surfaceType = null;
			numSoundVariants = 10;
		}
		
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
			if(materials.TryGetValue(type,out var material)) {
				return material;
			}
			return materials[type] = (PhysicMaterial)Activator.CreateInstance(type);
		}
	}
}
