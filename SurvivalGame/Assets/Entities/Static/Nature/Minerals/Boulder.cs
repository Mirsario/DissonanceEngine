using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine;

namespace Game
{
	public class Boulder : StaticEntity, IHasMaterial
	{
		public MeshRenderer renderer;
		public MeshCollider collider;
		
		public override void OnInit()
		{
			base.OnInit();
			
			var mesh = Resources.Get<Mesh>($"{GetType().Name}.obj");

			renderer = AddComponent<MeshRenderer>();
			renderer.Mesh = mesh;
			renderer.Material = Resources.Find<Material>($"{GetType().Name}");

			collider = AddComponent<MeshCollider>();
			collider.Mesh = mesh;
			collider.Convex = false;
		}

		PhysicMaterial IHasMaterial.GetMaterial(Vector3? atPoint) => PhysicMaterial.GetMaterial<StonePhysicMaterial>();
	}
}
