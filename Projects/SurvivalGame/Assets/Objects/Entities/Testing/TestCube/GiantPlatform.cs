using System;
using GameEngine;
using GameEngine.Graphics;

namespace SurvivalGame
{
	public class GiantPlatform : Entity
	{
		public MeshRenderer renderer;
		public BoxCollider collider;
		public Rigidbody rigidbody;

		public override void OnInit()
		{
            layer = Layers.GetLayerIndex("World");

            renderer = AddComponent<MeshRenderer>();
            renderer.Mesh = PrimitiveMeshes.Cube;
            renderer.Material = Resources.Get<Material>("Entities/Testing/TestCube/TestCube.material");

            collider = AddComponent<BoxCollider>();
            collider.Size = new Vector3(100f,1f,100f);

            //Transform.LocalScale = collider.Size;
        }
    }
}