using BulletSharp;
using Dissonance.Engine.Graphics;
using Dissonance.Engine.IO;

namespace Dissonance.Engine.Physics
{
	public abstract class CollisionMesh : Asset
	{
		internal CollisionShape collShape;

		public abstract void SetupFromMesh(Mesh mesh);
	}
}
