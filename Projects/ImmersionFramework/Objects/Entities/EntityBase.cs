using System;
using GameEngine;
using GameEngine.Physics;

namespace ImmersionFramework
{
	//A GameObject that's bind to a World and is possibly controllable by players (possessing tables is ok!)
	//Normal entities don't get to read inputs from brains though! Only Mobs can do that.
	public abstract class EntityBase : GameObject
	{
		public abstract bool IsPlayer { get; }
		public abstract bool IsLocalPlayer { get; }

		public virtual Type CameraControllerType => typeof(BasicThirdPersonCamera);
		
		public virtual void UpdateIsPlayer(bool isPlayer) {}
		public virtual void UpdateController(InputProxy prevProxy,InputProxy newProxy) {}

		public override void OnInit()
		{
			layer = Layers.GetLayerIndex("Entity");
		}

		/*public static T Instantiate<T>(TWorld world,string name = default,Vector3 position = default,Quaternion rotation = default,bool init = true) where T : TEntity
		{
			var entity = Instantiate<T>(name,position,rotation,init:false);

			entity.world = world;

			if(init) {
				entity.Init();
			}

			return entity;
		}*/
	}
}