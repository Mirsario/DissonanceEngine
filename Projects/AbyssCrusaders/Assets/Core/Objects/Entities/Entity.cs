using GameEngine;
using GameEngine.Physics;

namespace AbyssCrusaders.Core
{
	//A GameObject that's bind to a World
	public abstract class Entity : GameObject2D
	{
		public World world;

		public override void OnInit()
		{
			layer = Layers.GetLayerIndex("Entity");
		}

		public static T Instantiate<T>(World world,string name = default,Vector2 position = default,float depth = 0f,float rotation = default,Vector2? scale = null,bool init = true) where T : Entity
		{
			var entity = Instantiate<T>(name,position,depth,rotation,scale,init);
			entity.world = world;
			if(init) {
				entity.Init();
			}
			return entity;
		}
	}
}