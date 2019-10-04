using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImmersionFramework;
using GameEngine;
using GameEngine.Physics;

namespace SurvivalGame
{
	public abstract class Entity : EntityBase
	{
		public World world;

		public override bool IsPlayer => false; //TODO:
		public override bool IsLocalPlayer => false; // LocalEntity==this;
		public override Type CameraControllerType => typeof(BasicThirdPersonCamera);

		public static T Instantiate<T>(World world,string name = default,Vector3 position = default,Quaternion rotation = default,bool init = true) where T : Entity
		{
			var entity = Instantiate<T>(name,position,rotation,init: false);

			entity.world = world;

			if(init) {
				entity.Init();
			}

			return entity;
		}
	}
}
