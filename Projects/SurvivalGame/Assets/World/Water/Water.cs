using GameEngine;
using GameEngine.Graphics;
using GameEngine.Physics;

namespace SurvivalGame
{
	public class Water : Entity
	{
		public override void OnInit()
		{
			layer = Layers.GetLayerIndex("World");

			AddComponent<MeshRenderer>(c => {
				c.Mesh = PrimitiveMeshes.GeneratePlane(world.Size,world.SizeInUnits,true,uvSize:(Vector2)world.Size);
				c.Material = Resources.Find<Material>("Water");
			});

			//Transform.LocalScale = new Vector3(world.xSizeInUnits,world.ySizeInUnits,1f);
		}
	}
}
