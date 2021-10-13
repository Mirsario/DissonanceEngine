using Dissonance.Engine.Graphics;
using Dissonance.Engine.Physics;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Dissonance.Engine.IO
{
	public class ConvexCollisionMeshManager : IAssetReader<ConvexCollisionMesh>
	{
		public string[] Extensions { get; } = { ".obj" };

		public async ValueTask<ConvexCollisionMesh> ReadFromStream(Stream stream, string assetPath, MainThreadCreationContext switchToMainThread)
		{
			var mesh = Assets.Get<Mesh>(assetPath, AssetRequestMode.ImmediateLoad).Value;
			var collisionMesh = new ConvexCollisionMesh();

			collisionMesh.SetupFromMesh(mesh);

			return collisionMesh;
		}
	}
}
