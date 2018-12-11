using System;
using System.Collections.Generic;
using GameEngine;

namespace Game2DTest
{
	public class CubeObj : GameObject
	{
		public MeshRenderer renderer;

		public override void OnInit()
		{
			layer = Layers.GetLayerIndex("Entity");

			renderer = AddComponent<MeshRenderer>();
			renderer.Mesh = PrimitiveMeshes.Cube;
			renderer.Material = new Material("CubeMaterial",Resources.Find<Shader>("Diffuse"));

			Transform.LocalScale = new Vector3(10f,1f,1f);

			AddComponent<Box2DCollider>();
			AddComponent<Rigidbody2D>().IsKinematic = true;
		}
		public override void FixedUpdate()
		{
			
		}
	}
}