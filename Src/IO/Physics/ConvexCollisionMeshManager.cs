using Dissonance.Engine.IO;
using Dissonance.Engine.Physics;
using System;
using System.IO;

namespace Dissonance.Engine.IO.Physics
{
	public class ConvexCollisionMeshManager : AssetManager<ConvexCollisionMesh>
	{
		public override string[] Extensions => new string[] { ".obj" };

		public override ConvexCollisionMesh Import(Stream stream,string fileName)
		{
			var mesh = Resources.ImportFromStream<Mesh>(stream,fileName: fileName);

			var collisionMesh = new ConvexCollisionMesh();

			collisionMesh.SetupFromMesh(mesh);

			return collisionMesh;
		}
		public override void Export(ConvexCollisionMesh mesh,Stream stream) => throw new NotImplementedException();
	}
}
