using System;
using System.Collections.Generic;
using GameEngine;

namespace Game
{
	//A GameObject that's bind to a World
	public class Entity : GameObject
	{
		public World world;
		
		public override void OnInit()
		{
			layer = Layers.GetLayerIndex("Entity");
		}

		public static T Instantiate<T>(World world,string name = default,Vector3 position = default,Quaternion rotation = default,bool init = true) where T : Entity
		{
			var entity = Instantiate<T>(name,position,rotation,false);
			entity.world = world;
			if(init) {
				entity.Init();
			}
			return entity;
		}
	}
}