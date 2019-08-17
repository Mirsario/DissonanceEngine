using GameEngine;
using GameEngine.Graphics;
using GameEngine.Extensions.Chains;
using GameEngine.Physics;

namespace AbyssCrusaders
{
	public class CubeObj : GameObject
	{
		public override void OnInit()
		{
			layer = Layers.GetLayerIndex("Entity");

			AddComponent<MeshRenderer>()
				.WithMesh(PrimitiveMeshes.Cube)
				.WithMaterial(new Material("CubeMaterial",Resources.Find<Shader>("Diffuse")));

			AddComponent<Box2DCollider>();

			AddComponent<Rigidbody2D>()
				.WithIsKinematic(true);

			Transform.LocalScale = new Vector3(10f,1f,1f);
		}
	}
}