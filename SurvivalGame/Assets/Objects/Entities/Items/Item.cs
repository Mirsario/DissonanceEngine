using System;
using System.Collections.Generic;
using GameEngine;

namespace SurvivalGame
{
	public abstract class Item : Entity
	{
		public MeshRenderer renderer;
		public MeshCollider collider;
		public Rigidbody rigidbody;
		
		public override void OnInit()
		{
			base.OnInit();

			string typeName = GetType().Name;
			
			renderer = AddComponent<MeshRenderer>();
			renderer.Mesh = Resources.Get<Mesh>($"{typeName}.obj");
			renderer.Material = Resources.Find<Material>($"{typeName}");

			collider = AddComponent<MeshCollider>();
			collider.Mesh = Resources.Get<Mesh>($"{typeName}.obj");

			rigidbody = AddComponent<Rigidbody>();
			rigidbody.Mass = 1f;
		}
	}
}