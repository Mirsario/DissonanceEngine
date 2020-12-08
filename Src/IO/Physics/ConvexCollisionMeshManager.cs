using Dissonance.Engine.Graphics;
using Dissonance.Engine.Physics;
using System;
using System.IO;

namespace Dissonance.Engine.IO
{
	public class ConvexCollisionMeshManager : AssetManager<ConvexCollisionMesh>
	{
		public override string[] Extensions => new string[] { ".obj" };

		public override ConvexCollisionMesh Import(Stream stream, string filePath)
		{
			var mesh = Resources.ImportFromStream<Mesh>(stream, filePath: filePath);

			var collisionMesh = new ConvexCollisionMesh();

			collisionMesh.SetupFromMesh(mesh);

			return collisionMesh;
		}
		public override void Export(ConvexCollisionMesh mesh, Stream stream) => throw new NotImplementedException();
	}
}
