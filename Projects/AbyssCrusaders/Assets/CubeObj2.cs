using GameEngine;
using GameEngine.Graphics;

namespace AbyssCrusaders
{
	public class CubeObj2 : GameObject
	{
		public MeshRenderer renderer;

		public override void OnInit()
		{
			layer = Layers.GetLayerIndex("Entity");

			renderer = AddComponent<MeshRenderer>();
			renderer.Mesh = PrimitiveMeshes.Cube;
			renderer.Material = new Material("CubeMaterial",Resources.Find<Shader>("Diffuse"));

			AddComponent<Box2DCollider>();
			AddComponent<Rigidbody2D>();
		}
	}
}