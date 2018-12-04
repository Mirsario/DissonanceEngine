using System;
using System.Collections.Generic;
using GameEngine;

namespace Game
{
	public class Skybox : GameObject
	{
		public override void OnInit()
		{
			var renderer = AddComponent<MeshRenderer>();
			renderer.Mesh = PrimitiveMeshes.InvertedCube;
			renderer.Material = Resources.Get<Material>("Skybox.material");
		}
		public override void RenderUpdate()
		{
			Transform.Position = Main.camera.Transform.Position;
			Transform.LocalScale = Vector3.one*1024;
		}
	}
}