using GameEngine;
using GameEngine.Graphics;
using GameEngine.Physics;
using ImmersionFramework;

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
			
			renderer = AddComponent<MeshRenderer>(c => {
				c.Mesh = Resources.Get<Mesh>($"{typeName}.obj");
				c.Material = Resources.Find<Material>($"{typeName}");
			});

			collider = AddComponent<MeshCollider>(c => c.Mesh = Resources.Get<ConvexCollisionMesh>($"{typeName}.obj"));
			rigidbody = AddComponent<Rigidbody>(c => c.Mass = 1f);
		}
	}
}