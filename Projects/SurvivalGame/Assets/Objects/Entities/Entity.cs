using System;
using GameEngine;

namespace SurvivalGame
{
	//A GameObject that's bind to a World and is possibly controllable by players (possessing tables is ok!)
	//Normal entities don't get to read inputs from brains though! Only Mobs can do that.
	public abstract class Entity : GameObject
	{
		public World world;

		public bool IsLocal => Main.LocalEntity==this;

		public virtual Type CameraControllerType => typeof(BasicThirdPersonCamera);
		
		public virtual void UpdateIsPlayer(bool isPlayer) {}

		public override void OnInit()
		{
			layer = Layers.GetLayerIndex("Entity");
		}

		public static T Instantiate<T>(World world,string name = default,Vector3 position = default,Quaternion rotation = default,bool init = true) where T : Entity
		{
			var entity = Instantiate<T>(name,position,rotation,init:false);
			entity.world = world;
			if(init) {
				entity.Init();
			}
			return entity;
		}
	}
}