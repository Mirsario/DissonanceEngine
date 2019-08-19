using BulletSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Physics
{
	public abstract class CollisionMesh : Asset<CollisionMesh>
	{
		internal CollisionShape collShape;

		public abstract void SetupFromMesh(Mesh mesh);
	}
}
