using GameEngine;
using GameEngine.Graphics;
using GameEngine.Physics;

namespace SurvivalGame
{
	public class GiantPlatform : Entity
	{
		public override void OnInit()
		{
            layer = Layers.GetLayerIndex("World");

            AddComponent<MeshRenderer>(c => {
				c.Mesh = PrimitiveMeshes.Cube;
				c.Material = Resources.Get<Material>("Entities/Testing/TestCube/TestCube.material");
			});

            AddComponent<BoxCollider>(c => c.Size = new Vector3(100f,1f,100f));
        }
    }
}