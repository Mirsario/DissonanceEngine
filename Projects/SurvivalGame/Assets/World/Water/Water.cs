﻿using GameEngine;
using GameEngine.Graphics;

namespace SurvivalGame
{
	public class Water : Entity
	{
		public MeshRenderer renderer;
		public Rigidbody rigidbody;

		public override void OnInit()
		{
			layer = Layers.GetLayerIndex("World");

			renderer = AddComponent<MeshRenderer>();
			renderer.Mesh = PrimitiveMeshes.GeneratePlane(world.Size,world.SizeInUnits,true,uvSize:(Vector2)world.Size);
			renderer.Material = Resources.Find<Material>("Water");

			//Transform.LocalScale = new Vector3(world.xSizeInUnits,world.ySizeInUnits,1f);
		}
	}
}
