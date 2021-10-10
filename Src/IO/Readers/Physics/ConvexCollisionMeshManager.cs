using Dissonance.Engine.Graphics;
using Dissonance.Engine.Physics;
using System;
using System.IO;

namespace Dissonance.Engine.IO
{
	public class ConvexCollisionMeshManager : IAssetReader<ConvexCollisionMesh>
	{
		public string[] Extensions { get; } = { ".obj" };

		public ConvexCollisionMesh ReadFromStream(Stream stream, string assetPath)
		{
			var mesh = Resources.Get<Mesh>(assetPath, AssetRequestMode.ImmediateLoad).Value;
			var collisionMesh = new ConvexCollisionMesh();

			collisionMesh.SetupFromMesh(mesh);

			return collisionMesh;
		}
	}
}
